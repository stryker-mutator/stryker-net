open System

[<EntryPoint>]
let main _ =
    let key:ConsoleKeyInfo = Console.ReadKey()

    let testing = 
        match key.KeyChar.CompareTo('1') with
            | 0 -> true
            | _ -> false

    let testingResults = 
        match testing with
            | true -> "true"
            | false -> "false"

    Console.WriteLine testingResults
    0 // return an integer exit code
