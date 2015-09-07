module main
open System
open ReadFile
open BerParser
open TapTypes

//type CallEventDetailsChoiceItems = 
//    | MobileOriginatedCall of (DateTime * string)
//    | GprsCall of (DateTime * string * int)
//
//type CallEventDetailChoice = 
//    |CallEventDetail of CallEventDetailsChoiceItems
//
//type CallEventDetailSequence = 
//    |CallEventDetails of CallEventDetailChoice list
//
//type NetworkInforChoiceItems =
//    |UtcTimeOffsetInfo
//    |RecEntityInfo
//
//type NetworkInfoChoice = 
//    |NetworkInfo of NetworkInforChoiceItems
//
//
//type DataInterchangeChoiceItems = 
//    | TransferBatch of NetworkInfoChoice * CallEventDetailSequence
//    | Notification of string
//
//type DataInterchangeChoice =
//    |DataInterchange of DataInterchangeChoiceItems
//
//

//let example = DataInterchange(
//                    TransferBatch(
//                        NetworkInfo(UtcTimeOffsetInfo),
//                        CallEventDetails(
//                            [CallEventDetail(MobileOriginatedCall(DateTime.UtcNow, "MobileOriginatedCall")); CallEventDetail(GprsCall(DateTime.Now, "GPRS call", 42))])))



[<EntryPoint>]
let main argv = 
    printfn "%A" argv

//    let test1 = calculateNumberFromPartials [(byte)1] 0 0;
//    let test2 = calculateNumberFromPartials [(byte)1;(byte)1] 0 1;
//    let test3 = calculateNumberFromPartials [(byte)1;(byte)1;(byte)1] 0 2;

    let allBytesInFile = Array.toList(readAllBytesFromGivenFile @"C:\Users\dlar\Desktop\TAP\TAPFiler\CbbTap\CDDNKDM6SPPZ33493")
    //let allBytesInFile = Array.toList(readAllBytesFromGivenFile @"C:\Users\dlar\Desktop\TAP\TAPFiler\CbbTap\CDDNKDM6SPPZ33490")
    //let allBytesInFile = Array.toList(readAllBytesFromGivenFile @"C:\Users\dlar\Desktop\TAP\TAPFiler\EksemplerFraBrands\CDDNKDM9SPBR44831")
    let test4 = getBytes allBytesInFile 4580 4592

    let firstByte = allBytesInFile.Item(0)
    
    let theClass = parseClass (firstByte)
    let thePc = parsePC(firstByte)
    let theTagNumber = getApplicationTag(allBytesInFile)

    let testBytes = [(byte)97;(byte)54]

    //let result = parseDataInterchange allBytesInFile

    let theClassIsUniversal = parseClass((byte)63) = Universal
    let theClassIsApplication = parseClass((byte)64) = Application
    let theClassIsContextSpecific = parseClass((byte)128) = ContextSpecific
    let theClassIsPrivate = parseClass((byte)192) = Private
    
    let thePcIsPrimitive = parsePC((byte)32) = Primitive
    let thePcIsPrimitive1 = parsePC((byte)192) = Primitive
    let thePcIsConstructed1 = parsePC((byte)63) = Primitive
    let thePcIsConstructed = parsePC((byte)32) = Constructed
    
    let startIndex = 0

    let theApplicationTagFromFirstByte = getApplicationTag [(byte)25] startIndex = (25,1)
    let theApplicationTagFromTwoBytes1 = getApplicationTag [(byte)31; (byte)1] startIndex = (1,2) //NB. Bit 8 in the second bit is not set to indicate that this is the last tag octet
    let theApplicationTagFromTwoBytes2 = getApplicationTag [(byte)31; (byte)2] startIndex = (2,2)
    let theApplicationTagFromTwoBytes3 = getApplicationTag [(byte)31; (byte)3] startIndex = (3,2) 
    let theApplicationTagFromTwoBytes128 = getApplicationTag [(byte)31; (byte)129; (byte)0] startIndex = (128,3)
    let actualValue = getApplicationTag [(byte)31; (byte)130; (byte)0] startIndex  
    let theApplicationTagFromTwoBytes256 = actualValue = (256 ,3) 
    let theApplicationTagFromTwoBytes129 = getApplicationTag [(byte)31; (byte)129; (byte)1] startIndex = (129,3)
    let theApplicationTagFromTwoBytes257 = getApplicationTag [(byte)31; (byte)130; (byte)1] startIndex = (257,3)  
    let theApplicationTagFromTwoBytes258 = getApplicationTag [(byte)31; (byte)130; (byte)2] startIndex = (258,3)
    let actualValue = getApplicationTag [(byte)31; (byte)129; (byte)128; (byte)0] startIndex
    let theApplicationTagFromTwoBytes258 = actualValue = (16384, 4)  
//    let carsten = parseDataInterChange 5
//    let bois = carsten
 
    let blaa = decode allBytesInFile 0
    
    0 // return an integer exit code

