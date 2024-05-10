using System.Collections;
using System.Collections.Generic;

namespace TargetProject;

public class Csharp2
{
    // delegate operator
    public static void DelegateOperator()
    {
        Func<int, int, int> sum = delegate (int a, int b) { return a + b; };
        Console.WriteLine(sum(3, 4));  // output: 7

        Action greet = delegate { Console.WriteLine("Hello!"); };
        greet();

        Action<int, double> introduce = delegate { Console.WriteLine("This is world!"); };
        introduce(42, 2.7);

        Func<int, int, int> constant = delegate (int _, int _) { return 42; };
        Console.WriteLine(constant(3, 4));  // output: 42

        Func<int, int, int> sum2 = static delegate (int a, int b) { return a + b; };
        Console.WriteLine(sum2(10, 4));  // output: 14
    }

    // itarators
    static void Iter()
    {
        Zoo theZoo = new Zoo();

        theZoo.AddMammal("Whale");
        theZoo.AddMammal("Rhinoceros");
        theZoo.AddBird("Penguin");
        theZoo.AddBird("Warbler");

        foreach (string name in theZoo)
        {
            Console.Write(name + " ");
        }
        Console.WriteLine();
        // Output: Whale Rhinoceros Penguin Warbler

        foreach (string name in theZoo.Birds)
        {
            Console.Write(name + " ");
        }
        Console.WriteLine();
        // Output: Penguin Warbler

        foreach (string name in theZoo.Mammals)
        {
            Console.Write(name + " ");
        }
        Console.WriteLine();
        // Output: Whale Rhinoceros

        Console.ReadKey();
    }

    public class Zoo : IEnumerable
    {
        // Private members.
        private List<Animal> animals = new List<Animal>();

        // Public methods.
        public void AddMammal(string name)
        {
            animals.Add(new Animal { Name = name, Type = Animal.TypeEnum.Mammal });
        }

        public void AddBird(string name)
        {
            animals.Add(new Animal { Name = name, Type = Animal.TypeEnum.Bird });
        }

        public IEnumerator GetEnumerator()
        {
            foreach (Animal theAnimal in animals)
            {
                yield return theAnimal.Name;
            }
        }

        // Public members.
        public IEnumerable Mammals
        {
            get { return AnimalsForType(Animal.TypeEnum.Mammal); }
        }

        public IEnumerable Birds
        {
            get { return AnimalsForType(Animal.TypeEnum.Bird); }
        }

        // Private methods.
        private IEnumerable AnimalsForType(Animal.TypeEnum type)
        {
            foreach (Animal theAnimal in animals)
            {
                if (theAnimal.Type == type)
                {
                    yield return theAnimal.Name;
                }
            }
        }

        // Private class.
        private class Animal
        {
            public enum TypeEnum { Bird, Mammal }

            public string Name { get; set; }
            public TypeEnum Type { get; set; }
        }
    }
    // partial types
    public partial class Employee
    {
        public void DoWork()
        {
            Console.WriteLine(1 + 1);
        }
    }

    public partial class Employee
    {
        public void GoToLunch()
        {
            Console.WriteLine(1 + 1);
        }
    }
}
