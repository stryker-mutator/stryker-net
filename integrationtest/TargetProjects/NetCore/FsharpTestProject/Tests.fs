module FsharpTestProject.Tests

open Xunit
open FsharpTargetProject

[<Fact>]
let ``describe scales and adds one`` () =
    Assert.Equal(7, B.describe 3)

[<Fact>]
let ``isLimit accepts the limit`` () =
    Assert.True(B.isLimit 10)

[<Fact>]
let ``describeName appends the greeting`` () =
    Assert.StartsWith("bob", B.describeName "bob")

[<Fact>]
let ``first returns the head`` () =
    Assert.Equal(1, B.first [ 1; 2; 3 ])

[<Fact>]
let ``ordered keeps every element`` () =
    Assert.Equal(3, List.length (B.ordered [ 3; 1; 2 ]))
