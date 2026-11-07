module FsharpTargetProject.B

open FsharpTargetProject

let limit = 10

let describe (x: int) =
    if A.isEnabled then A.scale x + 1 else 0

let isLimit (x: int) = x = limit

let describeName (name: string) = A.label name + A.greeting

let first (xs: int list) = List.head xs

let ordered (xs: int list) = List.sort xs

let quoted = <@ 1 + 2 @>

let untested (x: int) = x - 1
