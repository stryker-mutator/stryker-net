// See https://aka.ms/new-console-template for more information

using ConsoleApp1;

Console.WriteLine("Hello world!");

var calculator = new Calculator();

Console.WriteLine($"1 + 2 = {calculator.Add(1, 2)}");
Console.WriteLine($"1 - 2 = {calculator.Subtract(1, 2)}");
Console.WriteLine($"1 * 2 = {calculator.Multiply(1, 2)}");
Console.WriteLine($"1 / 2 = {calculator.Divide(1, 2)}");

