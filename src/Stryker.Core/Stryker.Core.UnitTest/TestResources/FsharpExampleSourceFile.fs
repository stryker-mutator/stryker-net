// Learn more about F# at http://fsharp.org

module ExampleProject

open System

let rec private Fibonacci(a : int) (b : int) (counter : int) (len : int) :int =        
            match counter <= len with
            | true ->
                Console.Write("{0}", a)
                Fibonacci b (a + b) (counter + 1) len
            | _ -> 0;  

let public Fibinacci(len : int) :int =
            Fibonacci 0 1 1 len     

let LoremIpsum : string =
             @"Lorem Ipsum
                    Dolor Sit Amet
                    Lorem Dolor Sit"

let private StringSplit =
            let testString = ""
            testString.Split("\n");
