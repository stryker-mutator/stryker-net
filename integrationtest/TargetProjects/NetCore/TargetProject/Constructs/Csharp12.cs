using System.Collections.Generic;

namespace TargetProject.Constructs;

public class Csharp12
{
    // collection expressions
    public static void CollectionExpressions()
    {
        // Create an array:
        int[] a = [1, 2, 3, 4, 5, 6, 7, 8];

        // Create a list:
        List<string> b = ["one", "two", "three"];

        // Create a span
        Span<char> c = ['a', 'b', 'c', 'd', 'e', 'f', 'h', 'i'];

        // Create a jagged 2D array:
        int[][] twoD = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];

        // Create a jagged 2D array from variables:
        int[] row0 = [1, 2, 3];
        int[] row1 = [4, 5, 6];
        int[] row2 = [7, 8, 9];
        int[][] twoDFromVariables = [row0, row1, row2];
    }

    //ref parameters
    delegate void DIn(in int p);
    delegate void DRef(ref int p);
    delegate void DRR(ref readonly int p);

    public static void RefParameters()
    {
        var dRR = (in int p) => { };
        var dIn = (ref readonly int p) => { };
        var dRef = (ref readonly int p) => { };
    }

    // lambda improvements
    public static void DefaultValueLambda()
    {
        var IncrementBy = (int source, int increment = 1) => source + increment;
        Console.WriteLine(IncrementBy(5)); // 6
        Console.WriteLine(IncrementBy(5, 2)); // 7

        var sum = (params int[] values) =>
        {
            var sum = 0;
            foreach (var value in values)
                sum += value;

            return sum;
        };

        var empty = sum();
        Console.WriteLine(empty); // 0

        var sequence = new[] { 1, 2, 3, 4, 5 };
        var total = sum(sequence);
        Console.WriteLine(total); // 15
    }

    public static void TuplesInLambda()
    {
        Func<(int, int, int), (int, int, int)> doubleThem = ns => (2 * ns.Item1, 2 * ns.Item2, 2 * ns.Item3);
        var numbers = (2, 3, 4);
        var doubledNumbers = doubleThem(numbers);
        Console.WriteLine($"The set {numbers} doubled: {doubledNumbers}");
        // Output:
        // The set (2, 3, 4) doubled: (4, 6, 8)
    }

    #if NET8_0_OR_GREATER
    // inline array
    [System.Runtime.CompilerServices.InlineArray(10)]
    public struct Buffer
    {
        private int _element0;
    }

    public static void InlineArray()
    {
        var buffer = new Buffer();
        for (var i = 0; i < 10; i++)
        {
            buffer[i] = i;
        }

        foreach (var i in buffer)
        {
            Console.WriteLine(i);
        }
    }
    #endif
}
