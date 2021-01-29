open System

let private key:ConsoleKeyInfo = Console.ReadKey()
    
let keybool =
    key.KeyChar.CompareTo('1')

let testing = 
    match keybool with
        | 0 -> true
        | _ -> false
    
let testingResults = 
    match testing with
        | true -> "true"
        | false -> "false"
            
let fortesting key = 
    match key with
        | true -> true
        | false -> false
        
[<EntryPoint>]
let main _ =
    Console.WriteLine (fortesting true)
    0   // return an integer exit code
