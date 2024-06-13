using System.Diagnostics.CodeAnalysis;

namespace TargetProject.Constructs
{
    public class CSharp7
    {
        // default keyword
        public double GetDefault()
        {
            return default;
        }

        // tuples
        public (int, string) GetTuple()
        {
            return (1, "one");
        }

        // local functions
        public char LocalFunction()
        {
            return LocalFunctionImpl();

            char LocalFunctionImpl() => 'a';
        }

        // pattern matching lists

        public class Car
        {
            public int Passengers { get; set; }
        }

        public class DeliveryTruck
        {
            public int GrossWeightClass { get; set; }
        }

        public class Taxi
        {
            public int Fares { get; set; }
        }

        public class Bus
        {
            public int Capacity { get; set; }
            public int Riders { get; set; }
        }

        public void PatternMatching()
        {
            var vehicles = new object[] { new Car { Passengers = 2 }, new DeliveryTruck { GrossWeightClass = 4 }, new Taxi { Fares = 4 }, new Bus { Capacity = 30, Riders = 15 } };

            foreach (var vehicle in vehicles)
            {
                Console.WriteLine(vehicle switch
                {
                    Car { Passengers: 1 } => "A car with one passenger",
                    Car { Passengers: 2 } => "A car with two passengers",
                    DeliveryTruck t => $"A delivery truck with gross weight class {t.GrossWeightClass}",
                    Taxi t => $"A taxi with {t.Fares} fares",
                    Bus { Capacity: var c, Riders: var r } when r > c => $"A bus with {r} riders, over capacity by {r - c}",
                    Bus { Capacity: var c, Riders: var r } => $"A bus with {r} riders",
                    _ => "Some other vehicle"
                });
            }
        }
    }
}
