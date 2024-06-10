module Library.FSharp.Library

let matchBool value =
    match value with
    | true -> "You passed true."
    | false -> "You passed false."
