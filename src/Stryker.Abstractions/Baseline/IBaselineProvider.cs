using Stryker.Configuration.Reporting;

namespace Stryker.Configuration.Baseline
{
    public interface IBaselineProvider
    {
        Task<IJsonReport> Load(string version);
        Task Save(IJsonReport report, string version);
    }
}
