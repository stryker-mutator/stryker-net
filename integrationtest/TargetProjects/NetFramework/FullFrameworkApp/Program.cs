using Newtonsoft.Json;
using System;

namespace FullFrameworkApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new Person()));
            Console.ReadKey();
        }
    }
}
