namespace CancellationTarget;

public static class Subject
{
    public static bool IsEnabled() => true;

    public static int Add(int value) => value + 1;

    public static bool Unobserved() => true;
}
