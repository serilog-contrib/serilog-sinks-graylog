using System;
using System.Collections.Generic;

namespace Serilog.Sinks.Graylog.Tests.ComplexIntegrationTest
{
    public class Profile
    {
        public int Id { get; set; }
        public LocalizedString FirstName { get; set; }
        public LocalizedString MiddleName { get; set; }
        public LocalizedString LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Sex { get; set; }
        public Document[] Documents { get; set; }
        public Contacts Contacts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Version { get; set; }
        public Company CompanyInfo { get; set; }
        public Location WorkLocationCity { get; set; }
        public string PhotoFingerprint { get; set; }

        public Preference Preference { get; set; }

        /// <summary>Gets or sets the additional information.</summary>
        /// <value>The additional information.</value>
        public IList<AdditionalInfo> AdditionalInfo { get; set; }
    }

    public class LocalizedString
    {
        public string Ru { get; set; }
        public string En { get; set; }
    }

    public class Document
    {
        public string Type { get; set; }
        public string Series { get; set; }
        public string Number { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime? IssuedOn { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        /// <summary>
        /// В рамках виз это используется как "страна, на которую выдана виза" 
        /// В рамках остальных документов - пока никак.
        /// </summary>
        public Location CitizenshipCountry { get; set; }
    }

    public class Contacts
    {
        public EmailAddress[] EmailAddresses { get; set; }
        public PhoneNumberContact[] PhoneNumbers { get; set; }

    }

    public class EmailAddress
    {
        public string Type { get; set; }
        public string Address { get; set; }
    }

    public class PhoneNumberContact
    {
        public string Type { get; set; }
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public string PhoneNumber { get; set; }
        public string ExtensionNumber { get; set; }
    }

    public class Location
    {
        public string Type { get; set; }
        public LocalizedString Name { get; set; }
        public string Code { get; set; }

        public Location LocatedIn { get; set; }

    }

    public class Company
    {
        public LocalizedString CompanyName { get; set; }
        public LocalizedString HoldingName { get; set; }
    }

    public class Preference
    {
        public int Id { get; set; }
        public string Comments { get; set; }
        public string InternalComments { get; set; }
        public PreferenceInformation[] PreferenceInformations { get; set; }
    }

    public class PreferenceInformation
    {
        //public int Id { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public LocalizedString Name { get; set; }
    }

    public class AdditionalInfo
    {
        /// <summary>Gets or sets the name en.</summary>
        /// <value>The name en.</value>
        public string NameEn { get; set; }
        /// <summary>Gets or sets the name ru.</summary>
        /// <value>The name ru.</value>
        public string NameRu { get; set; }
        /// <summary>Gets or sets the value en.</summary>
        /// <value>The value en.</value>
        public string ValueEn { get; set; }
        /// <summary>Gets or sets the value ru.</summary>
        /// <value>The value ru.</value>
        public string ValueRu { get; set; }
    }

}