module DetermineClassTest

open Xunit
open Decoder

[<Fact>]
let Given_0_DetermineClass_should_return_Universal () =
    Assert.Equal(Universal, DetermineClass 0 )

[<Fact>]
let Given_3_DetermineClass_should_return_Private () =
    Assert.Equal(Private, DetermineClass 3 )