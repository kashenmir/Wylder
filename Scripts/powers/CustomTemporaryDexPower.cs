using BaseLib.Abstracts;
using BaseLib.Hooks;
using MegaCrit.Sts2.Core.Models.Powers;

namespace wylder.Scripts.powers;

public abstract class CustomTemporaryDexPower : TemporaryDexterityPower,
ICustomPower,
ICustomModel,
ILocalizationProvider,
IHealthBarForecastSource
{
    public virtual string? CustomPackedIconPath => (string) null;

    public virtual string? CustomBigIconPath => (string) null;

    public virtual string? CustomBigBetaIconPath => (string) null;

    public virtual IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context)
    {
        return (IEnumerable<HealthBarForecastSegment>) Array.Empty<HealthBarForecastSegment>();
    }

    public virtual List<(string, string)>? Localization => (List<(string, string)>) null;
}