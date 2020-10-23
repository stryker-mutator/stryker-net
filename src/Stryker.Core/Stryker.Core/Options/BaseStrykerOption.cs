namespace Stryker.Core.Options
{
	public abstract class BaseStrykerOption<T> : IStrykerOption<T>
	{
		public const string ErrorMessage = "The value for one of your settings is not correct. Try correcting or removing them.";
		public abstract StrykerOption Type { get; }
		public string Name => GetType().Name;
		public abstract string HelpText { get; }
		public T Value { get; protected set; }
		public abstract T DefaultValue { get; }
	}
}
