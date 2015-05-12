module DecoderTest

open Xunit
open Decoder

[<Fact>]
let Given_0_DeterminePC_should_return_Primitive () =
    Assert.Equal(Primitive, DeterminePC 0 )

[<Fact>]
let Given_1_DeterminePC_should_return_Primitive () =
    Assert.Equal(Constructed, DeterminePC 1 )

