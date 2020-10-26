namespace Stryker.Core.Options
{
    public abstract class SimpleStrykerInput<T> : ComplexStrykerInput<T, T>
    {
        public override T DefaultInput => DefaultValue;
    }
}
