using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetProject.Constructs;

public class Csharp3
{
    // anonymous types
    public class Product
    {
        public string Color { get; set; }
        public double Price { get; set; }
    }

    public static void AnonymousTypes()
    {
        var person = new { Name = "John", Age = 42 + 1 };
        Console.WriteLine(person.Name);  // output: John
        Console.WriteLine(person.Age);  // output: 42

        var products = new List<Product>
        {
            new Product { Color = "Red", Price = 100 },
            new Product { Color = "Green", Price = 200 },
            new Product { Color = "Blue", Price = 300 }
        };

        var productQuery =
            from prod in products
            select new { prod.Color, prod.Price };
    }

    // linq
    public void FirstExample()
    {
        var dynamicList = new List<dynamic>
        {
            new { Name = "John", Age = 42 },
            new { Name = "Jane", Age = 21 }
        };
        var allHaveLongNames = dynamicList.Where(s => s.Age > 18)
                          .Select(s => s)
                          .Where(st => st.StandardID > 0)
                          .Select(s => s.StudentName)
                          .All(x => x.Length > 5);
    }

    // query expressions
    public static void QueryExpressions()
    {
        var numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        var numQuery =
            from num in numbers
            where num % 2 == 0
            select num;

        foreach (var num in numQuery)
        {
            Console.Write("{0,1} ", num);
        }
        Console.WriteLine();
        // Output: 2 4 6 8 10
    }

    // initializers
    public static void Initializers()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var colors = new Dictionary<string, string>
        {
            ["red"] = "FF0000",
            ["green"] = "00FF00",
            ["blue"] = "0000FF"
        };
        var thing = new IndexersExample
        {
            name = "object one",
            [1] = '1',
            [2] = '4',
            [3] = '9',
            Size = Math.PI
        };
    }

    private class IndexersExample
    {
        public string name;
        public double Size { set; get; }
        public char this[int i] { set => name.Remove(i, 1).Insert(i, value.ToString()); get => name.ElementAt(i); }
    }

    // dynamic binding
    public static void DynamicBinding()
    {
        dynamic d = 1 + 1;
        Console.WriteLine(d.GetType());  // output: System.Int32

        d = "string" + 1;
        Console.WriteLine(d.GetType());  // output: System.String
    }

    static dynamic _field;
    dynamic Prop { get => 1 + 1; set => _field = value; }

    public dynamic ExampleMethod(dynamic d)
    {
        dynamic local = "Local variable";
        var two = 2;

        if (d is int)
        {
            return local;
        }
        else
        {
            return two;
        }
    }
}
