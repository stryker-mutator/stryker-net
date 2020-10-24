namespace Stryker.Core.Options
{
    public abstract class BaseStrykerOption<T> : IStrykerOption<T>
    {
        public abstract StrykerOption Type { get; }
        public static string HelpText { get; protected set; } = string.Empty;
        public static T DefaultValue { get; protected set; } = default;

        private T _value = default;
        /// <summary>
        /// The value of the option. Returns <see cref="DefaultValue"/> if current value equals <code>default(T)</code>
        /// </summary>
        public T Value
        {
            get
            {
                return _value?.Equals(default) ?? false ? DefaultValue : _value;
            }
            protected set
            {
                _value = value;
            }
        }
    }
}
