module Decoder

type PC =
    | Primitive
    | Constructed


type UniversalClassTag = 
    | Eoc
    | Boolean of bool
    | Integer of int
    | BitString
    | OctetString
    | Null
    | ObjectIdentifier
    | ObjectDescriptor
    | External
    | Real
    | Enumerated
    | EmbeddedPvd
    | Utf8String
    | RelatuiveOID
      //| (reserved)	-	14
       //| (reserved)	-	15
    | SequenceAndSequenceOf
    | SetAndSetOf
    | NumericString
    | PrintableString
    | T61String
    | VideotexString
    | Ia5String
    | UtcTime
    | GeneralizedTime
    | GraphicString
    | VisibleString
    | GeneralString
    | UniversalString
    | CharacterString
    | BmpString
    | UseLongForm

type TapApplicationTag =
    |TransferBatch
    

type Class =
    |Universal //of UniversalClassTag
    |Application //of TapApplicationTag
    |ContextSpecific
    |Private


let GetBoolean x =
    true

let GetInteger x =
    5

let DetermineUniversalClassTag fiveBits =
    match fiveBits with
    | 0 -> Eoc
    | 1 -> Boolean(GetBoolean 5)
    | 2 -> Integer(GetInteger 5)
    | 3 -> BitString
    | 4 -> OctetString
    | 5 -> Null
    | 6 -> ObjectIdentifier
    | 7 -> ObjectDescriptor
    | 8 -> External
    | 9 -> Real
    | 10 -> Enumerated
    | 11 -> EmbeddedPvd
    | 12 -> Utf8String
    | 13 -> RelatuiveOID
    //| (reserved)	-	14
    //| (reserved)	-	15
    | 16 -> SequenceAndSequenceOf
    | 17 -> SetAndSetOf
    | 18 -> NumericString
    | 19 -> PrintableString
    | 20 -> T61String
    | 21 -> VideotexString
    | 22 -> Ia5String
    | 23 -> UtcTime
    | 24 -> GeneralizedTime
    | 25 -> GraphicString
    | 26 -> VisibleString
    | 27 -> GeneralString
    | 28 -> UniversalString
    | 29 -> CharacterString
    | 30 -> BmpString
    | 31 -> UseLongForm
    | _ -> raise (System.ArgumentException("Value for Universal Class Tag was not in the interval 0-31 "))
//
//let DetermineClass byte =
//    let twoLargestBits = byte >>> 6
//    if twoLargestBits =  0
//    then Universal
//    else if twoLargestBits = 1
//         then Application
//         else if twoLargestBits = 2
//              then  ContextSpecific
//              else Private
    
let DetermineClass twoBits =
    match twoBits with
    | 0 -> Universal
    | 1 -> Application
    | 2 -> ContextSpecific
    | 3 -> Private
    | _ -> raise (System.ArgumentException("Value for Class was not in the interval 0-3 "))

let DeterminePC bit = 
    match bit with
    | 0 -> Primitive
    | 1 -> Constructed
    | _ -> raise (System.ArgumentException("Value for PC was not either 0 or 1"))


[<EntryPoint>]
let main argv = 
    let byte = 97
    
    let twoMostSignificantBits = byte >>> 6
    let Class = DetermineClass twoMostSignificantBits
    
    let bit678 = byte >>> 5 
    let bit6 = 
    let PC = DeterminePC bit6
    
    let bit5To0 = byte

    printfn "%A" Class
    System.Console.ReadKey() |> ignore
    0 

