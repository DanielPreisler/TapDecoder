module DetermineUniversalClassTagTest

open Xunit
open Decoder

[<Fact>]
let Given_0_DetermineUniversalClassTag_should_return_Eoc () =
    Assert.Equal(Eoc, DetermineUniversalClassTag 0)

[<Fact>]
let Given_1_DetermineUniversalClassTag_should_return_Boolean () =
    Assert.Equal(Boolean(true), DetermineUniversalClassTag 1)

[<Fact>]
let Given_31_DetermineUniversalClassTag_should_return_UseLongForm () =
    Assert.Equal(UseLongForm, DetermineUniversalClassTag 31)