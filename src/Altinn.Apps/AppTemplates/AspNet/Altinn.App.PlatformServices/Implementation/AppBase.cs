using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Altinn.App.Common.Enums;
using Altinn.App.Common.Models;
using Altinn.App.Services.Configuration;
using Altinn.App.Services.Helpers;
using Altinn.App.Services.Interface;
using Altinn.App.Services.Models;
using Altinn.App.Services.Models.Validation;
using Altinn.Common.EFormidlingClient;
using Altinn.Common.EFormidlingClient.Models.SBD;
using Altinn.Platform.Profile.Models;
using Altinn.Platform.Storage.Interface.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Altinn.App.Services.Implementation
{
    /// <summary>
    /// Default implementation of the core Altinn App interface.
    /// </summary>
    public abstract class AppBase : IAltinnApp
    {
        private readonly Application _appMetadata;
        private readonly IAppResources _resourceService;
        private readonly ILogger<AppBase> _logger;
        private readonly IData _dataService;
        private readonly IProcess _processService;
        private readonly IPDF _pdfService;
        private readonly IPrefill _prefillService;
        private readonly IInstance _instanceService;
        private readonly IRegister _registerService;
        private readonly string pdfElementType = "ref-data-as-pdf";
        private readonly UserHelper _userHelper;
        private readonly IProfile _profileService;
        private readonly IText _textService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEFormidlingClient _eFormidlingClient;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Initialize a new instance of <see cref="AppBase"/> class with the given services.
        /// </summary>
        /// <param name="resourceService">The service giving access to local resources.</param>
        /// <param name="logger">A logging service.</param>
        /// <param name="dataService">The service giving access to data.</param>
        /// <param name="processService">The service giving access the App process.</param>
        /// <param name="pdfService">The service giving access to the PDF generator.</param>
        /// <param name="prefillService">The service giving access to prefill mechanisms.</param>
        /// <param name="instanceService">The service giving access to instance data</param>
        /// <param name="registerService">The service giving access to register data</param>
        /// <param name="settings">the general settings</param>
        /// <param name="profileService">the profile service</param>
        /// <param name="textService">The text service</param>
        /// <param name="httpContextAccessor">the httpContextAccessor</param>
        /// <param name="eFormidlingClient">The eFormidling client</param>
        /// <param name="appSettings">The appsettings</param>
        protected AppBase(
            IAppResources resourceService,
            ILogger<AppBase> logger,
            IData dataService,
            IProcess processService,
            IPDF pdfService,
            IPrefill prefillService,
            IInstance instanceService,
            IRegister registerService,
            IOptions<GeneralSettings> settings,
            IProfile profileService,
            IText textService,
            IHttpContextAccessor httpContextAccessor,
            IEFormidlingClient eFormidlingClient = null,
            IOptions<AppSettings> appSettings = null)
        {
            _appMetadata = resourceService.GetApplication();
            _resourceService = resourceService;
            _logger = logger;
            _dataService = dataService;
            _processService = processService;
            _pdfService = pdfService;
            _prefillService = prefillService;
            _instanceService = instanceService;
            _registerService = registerService;
            _userHelper = new UserHelper(profileService, registerService, settings);
            _profileService = profileService;
            _textService = textService;
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings.Value;
            _eFormidlingClient = eFormidlingClient;
        }

        /// <inheritdoc />
        public abstract Type GetAppModelType(string dataType);

        /// <inheritdoc />
        public abstract object CreateNewAppModel(string dataType);

        /// <inheritdoc />
        public abstract Task<bool> RunAppEvent(AppEventType appEvent, object model, ModelStateDictionary modelState = null);

        /// <inheritdoc />
        public abstract Task RunDataValidation(object data, ModelStateDictionary validationResults);

        /// <inheritdoc />
        public abstract Task RunTaskValidation(Instance instance, string taskId, ModelStateDictionary validationResults);

        /// <inheritdoc />
        public abstract Task<bool> RunCalculation(object data);

        /// <inheritdoc />
        public abstract Task<InstantiationValidationResult> RunInstantiationValidation(Instance instance);

        /// <inheritdoc />
        public abstract Task RunDataCreation(Instance instance, object data);

        /// <inheritdoc />
        public abstract Task<AppOptions> GetOptions(string id, AppOptions options);

        /// <inheritdoc />
        public abstract Task RunProcessTaskEnd(string taskId, Instance instance);

        /// <inheritdoc />
        public abstract Task<LayoutSettings> FormatPdf(LayoutSettings layoutSettings, object data);

        /// <inheritdoc />
        public Task<string> OnInstantiateGetStartEvent()
        {
            _logger.LogInformation("OnInstantiate: GetStartEvent");

            // return start event
            return Task.FromResult("StartEvent_1");
        }

        /// <inheritdoc />
        public async Task OnStartProcess(string startEvent, Instance instance)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"OnStartProcess for {instance.Id}");
        }

        /// <inheritdoc />
        public async Task OnEndProcess(string endEvent, Instance instance)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"OnEndProcess for {instance.Id}, endEvent: {endEvent}");
        }

        /// <inheritdoc />
        public async Task OnStartProcessTask(string taskId, Instance instance)
        {
            _logger.LogInformation($"OnStartProcessTask for {instance.Id}");

            foreach (DataType dataType in _appMetadata.DataTypes.Where(dt => dt.TaskId == taskId && dt.AppLogic?.AutoCreate == true))
            {
                _logger.LogInformation($"Auto create data element: {dataType.Id}");

                DataElement dataElement = instance.Data.Find(d => d.DataType == dataType.Id);

                if (dataElement == null)
                {
                    dynamic data = CreateNewAppModel(dataType.AppLogic.ClassRef);

                    // runs prefill from repo configuration if config exists
                    await _prefillService.PrefillDataModel(instance.InstanceOwner.PartyId, dataType.Id, data);
                    await RunDataCreation(instance, data);
                    Type type = GetAppModelType(dataType.AppLogic.ClassRef);

                    DataElement createdDataElement = await _dataService.InsertFormData(instance, dataType.Id, data, type);
                    instance.Data.Add(createdDataElement);

                    _logger.LogInformation($"Created data element: {createdDataElement.Id}");
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> CanEndProcessTask(string taskId, Instance instance, List<ValidationIssue> validationIssues)
        {
            // check if the task is validated
            if (instance.Process?.CurrentTask?.Validated != null)
            {
                ValidationStatus validationStatus = instance.Process.CurrentTask.Validated;

                if (validationStatus.CanCompleteTask)
                {
                    return true;
                }
            }
            else
            {
                if (validationIssues.Count == 0)
                {
                    return true;
                }
            }

            return await Task.FromResult(false);
        }

        /// <inheritdoc />
        public async Task OnEndProcessTask(string taskId, Instance instance)
        {
            await RunProcessTaskEnd(taskId, instance);

            _logger.LogInformation($"OnEndProcessTask for {instance.Id}. Locking data elements connected to {taskId}");

            List<DataType> dataTypesToLock = _appMetadata.DataTypes.FindAll(dt => dt.TaskId == taskId);

            foreach (DataType dataType in dataTypesToLock)
            {
                bool generatePdf = dataType.AppLogic != null;

                foreach (DataElement dataElement in instance.Data.FindAll(de => de.DataType == dataType.Id))
                {
                    dataElement.Locked = true;
                    _logger.LogInformation($"Locking data element {dataElement.Id} of dataType {dataType.Id}.");
                    Task updateData = _dataService.Update(instance, dataElement);

                    if (generatePdf)
                    {
                        Type dataElementType = GetAppModelType(dataType.AppLogic.ClassRef);
                        Task createPdf = GenerateAndStoreReceiptPDF(instance, taskId, dataElement, dataElementType);
                        await Task.WhenAll(updateData, createPdf);
                    }
                    else
                    {
                        await updateData;
                    }
                }
            }

            if (_appSettings.EnableEFormidling) // TODO: add check for taskID from appmetadata.
            {
                SendEFormidlingShipment(instance, taskId);
            }

            if (_appMetadata.AutoDeleteOnProcessEnd)
            {
                int instanceOwnerPartyId = int.Parse(instance.InstanceOwner.PartyId);
                Guid instanceGuid = Guid.Parse(instance.Id.Split("/")[1]);

                await _instanceService.DeleteInstance(instanceOwnerPartyId, instanceGuid, true);
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual async Task<List<string>> GetPageOrder(string org, string app, int instanceOwnerId, Guid instanceGuid, string layoutSetId, string currentPage, string dataTypeId, object formData)
        {
            LayoutSettings layoutSettings = null;

            if (string.IsNullOrEmpty(layoutSetId))
            {
                layoutSettings = _resourceService.GetLayoutSettings();
            }
            else
            {
                layoutSettings = _resourceService.GetLayoutSettingsForSet(layoutSetId);
            }

            return await Task.FromResult(layoutSettings.Pages.Order);
        }

        /// <inheritdoc />
        public virtual async Task<(string, Arkivmelding)> GetEFormidlingArkivmelding(Instance instance)
        {
            await Task.CompletedTask;
            return (null, null);
        }

        /// <inheritdoc />
        public virtual async Task<List<Receiver>> GetEFormidlingReceivers()
        {
            await Task.CompletedTask;
            Identifier identifier = new Identifier(_appMetadata.EFormidlingContract.Receiver, "iso6523-actorid-upis");
            Receiver receiver = new Receiver(identifier, null);

            return new List<Receiver> { receiver };
        }



        private async Task GenerateAndStoreReceiptPDF(Instance instance, string taskId, DataElement dataElement, Type dataElementModelType)
        {
            string app = instance.AppId.Split("/")[1];
            string org = instance.Org;
            int instanceOwnerId = int.Parse(instance.InstanceOwner.PartyId);
            Guid instanceGuid = Guid.Parse(instance.Id.Split("/")[1]);

            string layoutSetsString = _resourceService.GetLayoutSets();
            LayoutSets layoutSets = null;
            LayoutSet layoutSet = null;
            if (!string.IsNullOrEmpty(layoutSetsString))
            {
                layoutSets = JsonConvert.DeserializeObject<LayoutSets>(layoutSetsString);
                layoutSet = layoutSets.Sets.FirstOrDefault(t => t.DataType.Equals(dataElement.DataType) && t.Tasks.Contains(taskId));
            }

            string layoutSettingsFileContent = layoutSet == null ? _resourceService.GetLayoutSettingsString() : _resourceService.GetLayoutSettingsStringForSet(layoutSet.Id);

            LayoutSettings layoutSettings = null;
            if (!string.IsNullOrEmpty(layoutSettingsFileContent))
            {
                layoutSettings = JsonConvert.DeserializeObject<LayoutSettings>(layoutSettingsFileContent);
            }

            object data = await _dataService.GetFormData(instanceGuid, dataElementModelType, org, app, instanceOwnerId, new Guid(dataElement.Id));

            layoutSettings = await FormatPdf(layoutSettings, data);
            XmlSerializer serializer = new XmlSerializer(dataElementModelType);
            using MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, data);
            stream.Position = 0;

            byte[] dataAsBytes = new byte[stream.Length];
            await stream.ReadAsync(dataAsBytes);
            string encodedXml = Convert.ToBase64String(dataAsBytes);

            UserContext userContext = await _userHelper.GetUserContext(_httpContextAccessor.HttpContext);
            UserProfile userProfile = await _profileService.GetUserProfile(userContext.UserId);

            // If layoutst exist pick correctr layotFiles
            string formLayoutsFileContent = layoutSet == null ? _resourceService.GetLayouts() : _resourceService.GetLayoutsForSet(layoutSet.Id);

            TextResource textResource = await _textService.GetText(org, app, userProfile.ProfileSettingPreference.Language);
            if (textResource == null && !userProfile.ProfileSettingPreference.Equals("nb"))
            {
                // fallback to norwegian if texts does not exist
                textResource = await _textService.GetText(org, app, "nb");
            }

            string textResourcesString = JsonConvert.SerializeObject(textResource);
            Dictionary<string, Dictionary<string, string>> optionsDictionary = await GetOptionsDictionary(formLayoutsFileContent);

            PDFContext pdfContext = new PDFContext
            {
                Data = encodedXml,
                FormLayouts = JsonConvert.DeserializeObject<Dictionary<string, object>>(formLayoutsFileContent),
                LayoutSettings = layoutSettings,
                TextResources = JsonConvert.DeserializeObject(textResourcesString),
                OptionsDictionary = optionsDictionary,
                Party = await _registerService.GetParty(instanceOwnerId),
                Instance = instance,
                UserProfile = userProfile,
                UserParty = userProfile.Party
            };

            Stream pdfContent = await _pdfService.GeneratePDF(pdfContext);
            await StorePDF(pdfContent, instance, textResource);
            pdfContent.Dispose();
        }

        private async Task<DataElement> StorePDF(Stream pdfStream, Instance instance, TextResource textResource)
        {
            string fileName = null;
            string app = instance.AppId.Split("/")[1];

            TextResourceElement titleText = textResource.Resources.Find(textResourceElement => textResourceElement.Id.Equals("ServiceName"));

            if (titleText != null && !string.IsNullOrEmpty(titleText.Value))
            {
                fileName = titleText.Value + ".pdf";
            }
            else
            {
                fileName = app + ".pdf";
            }

            fileName = GetValidFileName(fileName);

            return await _dataService.InsertBinaryData(
                instance.Id,
                pdfElementType,
                "application/pdf",
                fileName,
                pdfStream);
        }

        private string GetValidFileName(string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            fileName = Uri.EscapeDataString(fileName);
            return fileName;
        }

        private List<string> GetOptionIdsFromFormLayout(string formLayout)
        {
            List<string> optionsIds = new List<string>();
            string matchString = "\"optionsId\":\"";

            string[] formLayoutSubstrings = formLayout.Replace(" ", string.Empty).Split(new string[] { matchString }, StringSplitOptions.None);

            for (int i = 1; i < formLayoutSubstrings.Length; i++)
            {
                string[] workingSet = formLayoutSubstrings[i].Split('\"');
                string optionsId = workingSet[0];
                optionsIds.Add(optionsId);
            }

            return optionsIds;
        }

        private async Task<Dictionary<string, Dictionary<string, string>>> GetOptionsDictionary(string formLayout)
        {
            Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
            List<string> optionsIdsList = GetOptionIdsFromFormLayout(formLayout);

            foreach (string optionsId in optionsIdsList)
            {
                AppOptions appOptions = new AppOptions();

                appOptions.Options = _resourceService.GetOptions(optionsId);
                appOptions = await GetOptions(optionsId, appOptions);

                if (appOptions.Options != null && !dictionary.ContainsKey(optionsId))
                {
                    Dictionary<string, string> options = new Dictionary<string, string>();
                    foreach (AppOption item in appOptions.Options)
                    {
                        options.Add(item.Label, item.Value);
                    }

                    dictionary.Add(optionsId, options);
                }
            }

            return dictionary;
        }

        private async void RetrieveSchemaData(Instance instance, string taskId)
        {
            Guid instanceGuid = Guid.Parse(instance.Id.Split("/")[1]);
            int instanceOwnerPartyId = int.Parse(instance.InstanceOwner.PartyId);

            foreach (DataType dataType in _appMetadata.DataTypes.Where(dt => dt.TaskId == taskId && dt.AppLogic?.AutoCreate == true))
            {
                foreach (DataElement dataElement in instance.Data)
                {
                    if (dataElement.Filename == null)
                    {
                        Type dataElementType = GetAppModelType(dataType.AppLogic.ClassRef);
                        object data = await _dataService.GetFormData(instanceGuid, dataElementType, instance.Org, instance.AppId, instanceOwnerPartyId, new Guid(dataElement.Id));
                    }
                }
            }
        }

        private async void RetrieveAndSendBinaryAttachments(Instance instance)
        {
            Guid instanceGuid = Guid.Parse(instance.Id.Split("/")[1]);
            int instanceOwnerPartyId = int.Parse(instance.InstanceOwner.PartyId);

            // Filter by datatypes
            foreach (DataElement dataElement in instance.Data)
            {
                string fileName = dataElement.Id;

                if (dataElement.Filename != null)
                {
                    fileName = dataElement.Filename;
                }

                using (Stream stream = _dataService.GetBinaryData(instance.Org, instance.AppId, instanceOwnerPartyId, instanceGuid, new Guid(dataElement.Id)).Result)
                {
                    var sendBinary = await _eFormidlingClient.UploadAttachment(stream, instanceGuid.ToString(), fileName);
                }
            }
        }

        private async Task<StandardBusinessDocument> ConstructStandardBusinessDocument(string documentHeader, Arkivmelding arkivmelding, Instance instance)
        {
            string headerVersion = "1.0";
            List<Receiver> receivers = await GetEFormidlingReceivers();
            Sender digdirSender = new Sender(new Identifier(_appSettings.EFormidlingSender, "iso6523-actorid-upis"), null);
            List<Sender> sender = new List<Sender> { digdirSender };
            DocumentIdentification documentIdentification = null;
            BusinessScope businessScope = null;

            StandardBusinessDocumentHeader sbdHeader = new StandardBusinessDocumentHeader(headerVersion, sender, receivers, documentIdentification, businessScope);

            StandardBusinessDocument sbd = new StandardBusinessDocument(sbdHeader, arkivmelding);


            return null;
        }

        private StandardBusinessDocument ConfigureStandardBusinessDocument(Instance instance)
        {
            // File is used to test with - replace with "ConstructStandardBusinessDocument"
           / JObject sbdJson = JObject.Parse(File.ReadAllText(@"<PATH_TO_SBD.json>"));
            StandardBusinessDocument sbd = JsonConvert.DeserializeObject<StandardBusinessDocument>(sbdJson.ToString());

            // StandardBusinessDocument sbd = ConstructStandardBusinessDocument(instance);
            DateTime currentCreationTime = DateTime.Now;
            DateTime currentCreationTime2HoursLater = currentCreationTime.AddHours(2);

            string instanceGuid = Guid.Parse(instance.Id.Split("/")[1]).ToString();

            sbd.StandardBusinessDocumentHeader.BusinessScope.Scope.First().Identifier = _appMetadata.EFormidlingContract.Process;
            sbd.StandardBusinessDocumentHeader.BusinessScope.Scope.First().InstanceIdentifier = instanceGuid;
            sbd.StandardBusinessDocumentHeader.BusinessScope.Scope.First().ScopeInformation.First().ExpectedResponseDateTime = currentCreationTime2HoursLater;
            sbd.StandardBusinessDocumentHeader.DocumentIdentification.Type = _appMetadata.EFormidlingContract.DataTypes.First();
            sbd.StandardBusinessDocumentHeader.DocumentIdentification.InstanceIdentifier = instanceGuid;
            sbd.StandardBusinessDocumentHeader.DocumentIdentification.CreationDateAndTime = currentCreationTime;

            return sbd;
        }

        private async void SendEFormidlingShipment(Instance instance, string taskId)
        {
            string instanceGuid = instance.Id.Split("/")[1];

            (string arkivMeldingName, Arkivmelding arkivmelding) = await GetEFormidlingArkivmelding(instance);

            StandardBusinessDocument sbd = ConstructStandardBusinessDocument(instance);

            StandardBusinessDocument sbdVerified = await _eFormidlingClient.CreateMessage(sbd);
            RetrieveAndSendBinaryAttachments(instance);

            // RetrieveSchemaData(instance, taskId);
            string filename = "arkivmelding.xml";

            // Retrieve constructed arkivmelding dto
            using (FileStream fs2 = File.OpenRead(@"<PATH_TO_ARKIVMELDING.xml>"))
            {
                if (fs2.Length > 3)
                {
                    await _eFormidlingClient.UploadAttachment(fs2, instanceGuid, filename);
                }
            }

            bool shiptmentResult = await _eFormidlingClient.SendMessage(instanceGuid);

            if (!shiptmentResult)
            {
                _logger.LogError("// AppBase // SendEFormidlingShipment // Shipment of instance {InstanceId} failed.", instance.Id);
            }
        }
    }
}
