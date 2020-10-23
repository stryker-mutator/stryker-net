namespace Stryker.Core.Options
{
	public interface IStrykerOption<T>
	{
		string Name { get; }
		string HelpText { get; }
		T Value { get; }
		T DefaultValue { get; }
		StrykerOption Type { get;}
	}
}
