
namespace ExampleProject
{
    public class NullCoalescing
    {
        public string? DoIt(string? a, string? b)
        {
            return a ?? b;
        }
    }
}
