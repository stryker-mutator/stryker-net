using System.Diagnostics.CodeAnalysis;

namespace ExampleProject
{
    public class NewCsharpFeatures
    {
        public double GetDefaultDoubleValue()
        {
            return default;
        }
    }

    public class GenericClass<T1>
    {
        public bool TryGet<T2>([NotNullWhen(true)] out T2? result) where T2 : class, T1
        {
            result = null;
            return false;
        }
    }
}
