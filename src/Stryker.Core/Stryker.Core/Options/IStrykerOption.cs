namespace Stryker.Core.Options
{
    public interface IStrykerOption<T>
    {
        StrykerOption Type { get; }
        T Value { get; }
        T DefaultValue { get; }
        string HelpText { get; }
    }
}
