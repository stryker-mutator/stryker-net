namespace Stryker.Core.Options
{
    public abstract class BaseStrykerOption<T> : IStrykerOption<T>
    {
        public abstract StrykerOption Type { get; }
        public abstract string HelpText { get; }
        public virtual T DefaultValue { get; } = default;

        private T _value = default;
        /// <summary>
        /// The value of the option. Returns <see cref="DefaultValue"/> if current value equals <code>default(T)</code>
        /// </summary>
        public T Value
        {
            get
            {
                return _value.Equals(default) ? DefaultValue : _value;
            }
            protected set
            {
                _value = value;
            }
        }
    }
}
