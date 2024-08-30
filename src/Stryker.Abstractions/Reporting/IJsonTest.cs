namespace Stryker.Abstractions.Reporting;

public interface IJsonTest
{
    string Id { get; }
    ILocation Location { get; set; }
    string Name { get; set; }

    bool Equals(IJsonTest other);
    bool Equals(object obj);
    int GetHashCode();
}
