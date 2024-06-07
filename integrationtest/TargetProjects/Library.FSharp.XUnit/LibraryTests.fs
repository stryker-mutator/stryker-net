module Library.FSharp.Tests.LibraryTests

open Xunit
open Library.FSharp

[<Fact>]
let ``True is true`` () =
    let expected = "You passed true."
    let actual = Library.matchBool true
    Assert.Equal(expected, actual)

[<Fact>]
let ``False is false`` () =
    let expected = "You passed false."
    let actual = Library.matchBool false
    Assert.Equal(expected, actual)
