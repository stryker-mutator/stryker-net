module Tests

open Xunit

open Program

[<Fact>]
let ``My test`` () =
    Assert.True(true)

[<Fact>]
let ``inputTest`` () =
    Assert.Equal(true, fortesting(true))
