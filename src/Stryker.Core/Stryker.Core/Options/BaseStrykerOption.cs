namespace Stryker.Core.Options
{
	public abstract class BaseStrykerOption<T> : IStrykerOption<T>
	{
		protected BaseStrykerOption(params string[] parameters)
		{
			Validate(parameters);
		}

		public abstract StrykerOption Type { get; }
		public abstract string Name { get; }
		public abstract string HelpText { get; }
		public T Value { get; protected set; }
		public abstract T DefaultValue { get; }

		protected abstract void Validate(params string[] parameters);
	}
}
