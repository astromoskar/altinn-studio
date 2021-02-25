using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Altinn.Platform.Events.Models
{
    /// <summary>
    /// Class that describes a events subscriptions
    /// </summary>
    public class EventsSubscription
    {
        /// <summary>
        /// Endpoint to receive matching events
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// Subscription Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Filter on source
        /// </summary>
        public string SourceFilter { get; set; }

        /// <summary>
        /// Filter on subject
        /// </summary>
        public string SubjectFilter { get; set; }

        /// <summary>
        /// Filter on alternative subject
        /// </summary>
        public string AlternativeSubjectFilter { get; set; }

        /// <summary>
        /// Filter for type. Starts with
        /// </summary>
        public string TypeFilter { get; set; }

        /// <summary>
        /// The events consumer
        /// </summary>
        public string Consumer { get; set; }

        /// <summary>
        /// Who created this subscription
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// When subscription was created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Serializes the cloud event to a JSON string.
        /// </summary>
        /// <returns>Serialized cloud event</returns>
        public string Serialize()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { IgnoreNullValues = true });
        }

        /// <summary>
        /// Deserializes the cloud event to a JSON string.
        /// </summary>
        /// <returns>Cloud event</returns>
        public static CloudEvent Deserialize(string jsonString)
        {
            return JsonSerializer.Deserialize<CloudEvent>(jsonString, new JsonSerializerOptions { });
        }
    }
}
