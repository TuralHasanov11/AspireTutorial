using Microsoft.Extensions.Compliance.Classification;

namespace AspireSample.SharedKernel;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class EUIIDataAttribute : DataClassificationAttribute
{
    public EUIIDataAttribute()
        : base(ApplicationLoggingTaxonomy.EUIIDataClassification) { }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class EUPDataAttribute : DataClassificationAttribute
{
    public EUPDataAttribute()
        : base(ApplicationLoggingTaxonomy.EUPDataClassification) { }
}