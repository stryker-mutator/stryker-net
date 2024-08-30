using Stryker.Abstractions.Reporting;

namespace Stryker.Abstractions.Baseline
{
    public interface IBaselineProvider
    {
        Task<IJsonReport> Load(string version);
        Task Save(IJsonReport report, string version);
    }
}
