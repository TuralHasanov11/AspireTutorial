using Microsoft.Extensions.Compliance.Classification;

namespace SharedKernel;

/// <summary>
/// Provides reusable <see cref="DataClassification"/> definitions for logging and compliance.
/// </summary>
/// <summary>
/// Provides taxonomy definitions for data classification used in logging and compliance.
/// </summary>
public static class LoggingTaxonomyDefinitions
{
    /// <summary>
    /// Data classification for End User Identifiable Information (EUII).
    /// </summary>
    public static DataClassification EUIIDataClassification => new("EUIIDataTaxonomy", "EUII");


    /// <summary>
    /// Data classification for End User Pseudonymous Data (EUP).
    /// </summary>
    public static DataClassification EUPDataClassification => new("EUPDataTaxonomy", "EUP");

    /// <summary>
    /// Data classification for Customer Data as defined by Microsoft Purview.
    /// Customer Data includes user content such as emails, files, Teams chat, and Copilot interactions.
    /// </summary>
    public static DataClassification CustomerDataClassification => new("CustomerDataTaxonomy", "CustomerData");

    /// <summary>
    /// Data classification for Administrator Data.
    /// Administrator Data includes tenant admin's email, UPN, IP address, username, and display name.
    /// </summary>
    public static DataClassification AdministratorDataClassification => new("AdministratorDataTaxonomy", "AdministratorData");

    /// <summary>
    /// Data classification for Feedback Data.
    /// Feedback Data is feedback about compliance solutions submitted by tenant administrators.
    /// </summary>
    public static DataClassification FeedbackDataClassification => new("FeedbackDataTaxonomy", "FeedbackData");
}

/// <summary>
/// Attribute to mark a parameter, field, or property as End User Identifiable Information (EUII).
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class EuiiDataAttribute : DataClassificationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EuiiDataAttribute"/> class.
    /// </summary>
    public EuiiDataAttribute()
        : base(LoggingTaxonomyDefinitions.EUIIDataClassification) { }
    /// <summary>
    /// Attribute to mark a parameter, field, or property as End User Identifiable Information (EUII).
    /// </summary>
}

/// <summary>
/// Attribute to mark a parameter, field, or property as End User Pseudonymous Data (EUP).
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class EupDataAttribute : DataClassificationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EupDataAttribute"/> class.
    /// </summary>
    public EupDataAttribute()
        : base(LoggingTaxonomyDefinitions.EUPDataClassification) { }
    /// <summary>
    /// Attribute to mark a parameter, field, or property as End User Pseudonymous Data (EUP).
    /// </summary>
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class CustomerDataAttribute : DataClassificationAttribute
{
    /// <summary>
    /// Marks a property, field, or parameter as Customer Data for privacy and compliance purposes.
    /// </summary>
    public CustomerDataAttribute()
        : base(LoggingTaxonomyDefinitions.CustomerDataClassification) { }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class AdministratorDataAttribute : DataClassificationAttribute
{
    /// <summary>
    /// Marks a property, field, or parameter as Administrator Data for privacy and compliance purposes.
    /// </summary>
    public AdministratorDataAttribute()
        : base(LoggingTaxonomyDefinitions.AdministratorDataClassification) { }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class FeedbackDataAttribute : DataClassificationAttribute
{
    /// <summary>
    /// Marks a property, field, or parameter as Feedback Data for privacy and compliance purposes.
    /// </summary>
    public FeedbackDataAttribute()
        : base(LoggingTaxonomyDefinitions.FeedbackDataClassification) { }
}