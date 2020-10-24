namespace Stryker.Core.Options
{
    public abstract class ComplexStrykerInput<T, Y>
    {
        public abstract StrykerOption Type { get; }
        public static string HelpText { get; protected set; } = string.Empty;
        public static T DefaultValue { get; protected set; } = default;

        private static Y _defaultInput = default;
        public static Y DefaultInput
        {
            get
            {
                if (_defaultInput is null && DefaultValue is Y defaultValueAsY)
                {
                    return defaultValueAsY;
                }
                return _defaultInput;
            }
            protected set
            {
                _defaultInput = value;
            }
        }

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
