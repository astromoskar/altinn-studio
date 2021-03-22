using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Altinn.Common.EFormidlingClient.Models
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Arkivmelding"/> class. This class represents the actual arkivmelding.
    /// This class is autogenerated. XSD definition: https://github.com/difi/felleslosninger/blob/gh-pages/resources/arkivmelding/arkivmelding.xsd
    /// </summary>
    [ExcludeFromCodeCoverage]
    [XmlRoot(ElementName = "arkivmelding", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
    public class Arkivmelding
    {
        /// <summary>
        ///  Gets or sets the AntallFiler
        /// </summary>
        [XmlElement(ElementName = "antallFiler", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public int AntallFiler { get; set; }

        /// <summary>
        ///  Gets or sets the Mappe
        /// </summary>
        [XmlElement(ElementName = "mappe")]
        public List<Mappe> Mappe { get; set; }

        /// <summary>
        ///  Gets or sets the MeldingId
        /// </summary>
        [XmlElement(ElementName = "meldingId", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string MeldingId { get; set; }

        /// <summary>
        ///  Gets or sets the SchemaLocation
        /// </summary>
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }

        /// <summary>
        ///  Gets or sets the System
        /// </summary>
        [XmlElement(ElementName = "system", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string System { get; set; }

        /// <summary>
        ///  Gets or sets the Tidspunkt
        /// </summary>
        [XmlElement(ElementName = "tidspunkt", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string Tidspunkt { get; set; }

        /// <summary>
        ///  Gets or sets the Xmlns
        /// </summary>
        [XmlAttribute(AttributeName = "xmlns", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string Xmlns { get; set; }

        /// <summary>
        ///  Gets or sets the Xsi
        /// </summary>
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dokumentobjekt"/> class.
    /// </summary>
    [XmlRoot(ElementName = "dokumentobjekt")]
    public class Dokumentobjekt
    {
        /// <summary>
        ///  Gets or sets the Versjonsnummer
        /// </summary>
        [XmlElement(ElementName = "versjonsnummer")]
        public int Versjonsnummer { get; set; }

        /// <summary>
        ///  Gets or sets the Variantformat
        /// </summary>
        [XmlElement(ElementName = "variantformat")]
        public string Variantformat { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetDato
        /// </summary>
        [XmlElement(ElementName = "opprettetDato")]
        public DateTime OpprettetDato { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetAv
        /// </summary>
        [XmlElement(ElementName = "opprettetAv")]
        public string OpprettetAv { get; set; }

        /// <summary>
        ///  Gets or sets the ReferanseDokumentfil
        /// </summary>
        [XmlElement(ElementName = "referanseDokumentfil")]
        public string ReferanseDokumentfil { get; set; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dokumentbeskrivelse"/> class.
    /// </summary>
    [XmlRoot(ElementName = "dokumentbeskrivelse")]
    public class Dokumentbeskrivelse
    {
        /// <summary>
        ///  Gets or sets the SystemID
        /// </summary>
        [XmlElement(ElementName = "systemID")]
        public string SystemID { get; set; }

        /// <summary>
        ///  Gets or sets the Dokumenttype
        /// </summary>
        [XmlElement(ElementName = "dokumenttype")]
        public string Dokumenttype { get; set; }

        /// <summary>
        ///  Gets or sets the Dokumentstatus
        /// </summary>
        [XmlElement(ElementName = "dokumentstatus")]
        public string Dokumentstatus { get; set; }

        /// <summary>
        ///  Gets or sets the Tittel
        /// </summary>
        [XmlElement(ElementName = "tittel")]
        public string Tittel { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetDato
        /// </summary>
        [XmlElement(ElementName = "opprettetDato")]
        public DateTime OpprettetDato { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetAv
        /// </summary>
        [XmlElement(ElementName = "opprettetAv")]
        public string OpprettetAv { get; set; }

        /// <summary>
        ///  Gets or sets the TilknyttetRegistreringSom
        /// </summary>
        [XmlElement(ElementName = "tilknyttetRegistreringSom")]
        public string TilknyttetRegistreringSom { get; set; }

        /// <summary>
        ///  Gets or sets the Dokumentnummer
        /// </summary>
        [XmlElement(ElementName = "dokumentnummer")]
        public int Dokumentnummer { get; set; }

        /// <summary>
        ///  Gets or sets the TilknyttetDato
        /// </summary>
        [XmlElement(ElementName = "tilknyttetDato")]
        public DateTime TilknyttetDato { get; set; }

        /// <summary>
        ///  Gets or sets the TilknyttetAv
        /// </summary>
        [XmlElement(ElementName = "tilknyttetAv")]
        public string TilknyttetAv { get; set; }

        /// <summary>
        ///  Gets or sets the Dokumentobjekt
        /// </summary>
        [XmlElement(ElementName = "dokumentobjekt")]
        public Dokumentobjekt Dokumentobjekt { get; set; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Korrespondansepart"/> class.
    /// </summary>
    [XmlRoot(ElementName = "korrespondansepart")]
    public class Korrespondansepart
    {
        /// <summary>
        ///  Gets or sets the Korrespondanseparttype
        /// </summary>
        [XmlElement(ElementName = "korrespondanseparttype")]
        public string Korrespondanseparttype { get; set; }

        /// <summary>
        ///  Gets or sets the KorrespondansepartNavn
        /// </summary>
        [XmlElement(ElementName = "korrespondansepartNavn")]
        public string KorrespondansepartNavn { get; set; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Basisregistrering"/> class.
    /// </summary>
    [XmlTypeAttribute(TypeName = "journalpost")]
    public class Basisregistrering
    {
        /// <summary>
        ///  Gets or sets the SystemID
        /// </summary>
        [XmlElement(ElementName = "systemID")]
        public string SystemID { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetDato
        /// </summary>
        [XmlElement(ElementName = "opprettetDato")]
        public DateTime OpprettetDato { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetAv
        /// </summary>
        [XmlElement(ElementName = "opprettetAv")]
        public string OpprettetAv { get; set; }

        /// <summary>
        ///  Gets or sets the ArkivertDato
        /// </summary>
        [XmlElement(ElementName = "arkivertDato")]
        public DateTime ArkivertDato { get; set; }

        /// <summary>
        ///  Gets or sets the ArkivertAv
        /// </summary>
        [XmlElement(ElementName = "arkivertAv")]
        public string ArkivertAv { get; set; }

        /// <summary>
        ///  Gets or sets the ReferanseForelderMappe
        /// </summary>
        [XmlElement(ElementName = "referanseForelderMappe")]
        public string ReferanseForelderMappe { get; set; }

        /// <summary>
        ///  Gets or sets the Dokumentbeskrivelse
        /// </summary>
        [XmlElement(ElementName = "dokumentbeskrivelse")]
        public Dokumentbeskrivelse Dokumentbeskrivelse { get; set; }

        /// <summary>
        ///  Gets or sets the Tittel
        /// </summary>
        [XmlElement(ElementName = "tittel")]
        public string Tittel { get; set; }

        /// <summary>
        ///  Gets or sets the OffentligTittel
        /// </summary>
        [XmlElement(ElementName = "offentligTittel")]
        public string OffentligTittel { get; set; }

        /// <summary>
        ///  Gets or sets the Journalposttype
        /// </summary>
        [XmlElement(ElementName = "journalposttype")]
        public string Journalposttype { get; set; }

        /// <summary>
        ///  Gets or sets the Journalstatus
        /// </summary>
        [XmlElement(ElementName = "journalstatus")]
        public string Journalstatus { get; set; }

        /// <summary>
        ///  Gets or sets the Journaldato
        /// </summary>
        [XmlElement(ElementName = "journaldato")]
        public DateTime Journaldato { get; set; }

        /// <summary>
        ///  Gets or sets the Korrespondansepart
        /// </summary>
        [XmlElement(ElementName = "korrespondansepart")]
        public Korrespondansepart Korrespondansepart { get; set; }

        /// <summary>
        ///  Gets or sets the Type
        /// </summary>
        //[XmlAttribute(AttributeName = "type")]
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }

        /// <summary>
        ///  Gets or sets the Text
        /// </summary>
        [XmlText]
        public string Text { get; set; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mappe"/> class.
    /// </summary>
    [XmlTypeAttribute(TypeName = "saksmappe")]
    public class Mappe
    {
        /// <summary>
        ///  Gets or sets the AdministrativEnhet
        /// </summary>
        [XmlElement(ElementName = "administrativEnhet", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string AdministrativEnhet { get; set; }

        /// <summary>
        ///  Gets or sets the Basisregistrering
        /// </summary>
        [XmlElement(ElementName = "basisregistrering", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public Basisregistrering Basisregistrering { get; set; }

        /// <summary>
        ///  Gets or sets the Klassifikasjon
        /// </summary>
        [XmlElement(ElementName = "klassifikasjon", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public List<Klassifikasjon> Klassifikasjon { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetAv
        /// </summary>
        [XmlElement(ElementName = "opprettetAv", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string OpprettetAv { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetDato
        /// </summary>
        [XmlElement(ElementName = "opprettetDato", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string OpprettetDato { get; set; }

        /// <summary>
        ///  Gets or sets the Saksansvarlig
        /// </summary>
        [XmlElement(ElementName = "saksansvarlig", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string Saksansvarlig { get; set; }

        /// <summary>
        ///  Gets or sets the Saksdato
        /// </summary>
        [XmlElement(ElementName = "saksdato", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string Saksdato { get; set; }

        /// <summary>
        ///  Gets or sets the Saksstatus
        /// </summary>
        [XmlElement(ElementName = "saksstatus", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string Saksstatus { get; set; }

        /// <summary>
        ///  Gets or sets the SystemID
        /// </summary>
        [XmlElement(ElementName = "systemID", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string SystemID { get; set; }

        /// <summary>
        ///  Gets or sets the Tittel
        /// </summary>
        [XmlElement(ElementName = "tittel", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string Tittel { get; set; }

        /// <summary>
        ///  Gets or sets the Type
        /// </summary>
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mappe"/> class.
    /// </summary>
    [XmlRoot(ElementName = "klassifikasjon", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
    public class Klassifikasjon
    {
        /// <summary>
        ///  Gets or sets the KlasseID
        /// </summary>
        [XmlElement(ElementName = "klasseID", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string KlasseID { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetAv
        /// </summary>
        [XmlElement(ElementName = "opprettetAv", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string OpprettetAv { get; set; }

        /// <summary>
        ///  Gets or sets the OpprettetDato
        /// </summary>
        [XmlElement(ElementName = "opprettetDato", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string OpprettetDato { get; set; }

        /// <summary>
        ///  Gets or sets the ReferanseKlassifikasjonssystem
        /// </summary>
        [XmlElement(ElementName = "referanseKlassifikasjonssystem", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string ReferanseKlassifikasjonssystem { get; set; }

        /// <summary>
        ///  Gets or sets the Tittel
        /// </summary>
        [XmlElement(ElementName = "tittel", Namespace = "http://www.arkivverket.no/standarder/noark5/arkivmelding")]
        public string Tittel { get; set; }
    }
}
