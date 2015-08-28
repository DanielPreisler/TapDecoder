module BerParser
open TapTypes
open System

type IdentifierClass =
    |Universal
    |Application
    |ContextSpecific
    |Private
    |Error


type IdentifierPC =
    |Primitive
    |Constructed


type UniversalTag =
    |Eoc
    |Boolean
    |Integer
    |Sequence
    |UseLongForm  
    

let parseClass (identifierByte : byte) = 
    let shiftedForClass = (int)(identifierByte >>> 6) 
    match shiftedForClass with
    | 0 -> Universal
    | 1 -> Application
    | 2 -> ContextSpecific
    | 3 -> Private
    | _ -> Error

  
let parsePC (identifierByte : byte) =
    let shiftedToRemoveTwoMostSignificantBits = (int)(identifierByte <<< 2)
    let shiftedForPC = (int)(shiftedToRemoveTwoMostSignificantBits >>> 7) 
    match shiftedForPC with
    | 0 -> Primitive
    | 1 -> Constructed



//    match shiftedBackToOriginalPosition with
//    | 0 -> Eoc
//    | 1 -> Boolean
//    | 2 -> Integer
//    | 16 -> Sequence
//    | 31 -> UseLongForm

    
let parseUniversalContent bytes =
    (123 ,345) 



let rec getBytes (bytes : byte list) startIndex endIndex = 
    let firstByte = bytes.Item startIndex
    if startIndex = endIndex
    then [firstByte]
    else firstByte :: getBytes bytes (startIndex + 1) endIndex







let rec calculateTagNumberFromPartials (partialTagNumbers : byte list)  =
    match partialTagNumbers with
    | [] -> 0
    | firstPartialNumber::theRest ->
            let exponent = (float) ((partialTagNumbers.Length - 1) * 7)
            let factor = (int)(2.0 ** exponent)
            (int)firstPartialNumber * factor + calculateTagNumberFromPartials theRest



let bit8IsNotSet byte =
    (int)byte < 128

let rec getPartialTagNumbers (bytes : byte list) startIndex partialTagNumbers =
    let byteAtStartIndex = bytes.Item startIndex
    if bit8IsNotSet byteAtStartIndex
    then List.rev (byteAtStartIndex::partialTagNumbers)
    else getPartialTagNumbers bytes (startIndex + 1) (((byte)((int)byteAtStartIndex - 128))::partialTagNumbers)


let getLongApplicationTag (bytes : byte list) startIndex =
    let partialTagNumbers = getPartialTagNumbers bytes startIndex []
    let tagNumber = calculateTagNumberFromPartials partialTagNumbers
    (tagNumber, partialTagNumbers.Length)


let parseTagNumberFromByte (identifierByte : byte) = 
    let shiftedToRemoveThreeMostSignificantBits = identifierByte <<< 3
    (int)(shiftedToRemoveThreeMostSignificantBits >>> 3)

let getApplicationTag (bytes : byte list) startIndex =
    let tagValueFromFirstByte = parseTagNumberFromByte (bytes.Item startIndex)
    if tagValueFromFirstByte < 31 
    then
        let bytesUsedForTag = 1 
        (tagValueFromFirstByte, bytesUsedForTag)
    else 
        let applicationTagAndBytesUsed = getLongApplicationTag bytes (startIndex + 1)
        let bytesUsedForPartialTagNumbers = snd applicationTagAndBytesUsed
        (fst applicationTagAndBytesUsed, bytesUsedForPartialTagNumbers + 1)





let rec calculateNumberFromPartials (bytes : byte list) startIndex endIndex =
    let firstPartialNumber = (int) (bytes.Item startIndex)
    if startIndex = endIndex 
    then firstPartialNumber
    else
        let exponent = (float)((endIndex - startIndex) * 8)
        let factor = (int)(2.0 ** exponent)
        firstPartialNumber * factor + calculateNumberFromPartials bytes (startIndex + 1) endIndex

let getLongFormLength (bytes : byte list) numberOfOctetsEncodingTheLength startIndexForLength = 
    let endIndexForPartials = startIndexForLength + numberOfOctetsEncodingTheLength - 1
    let numberOfDataOctets = calculateNumberFromPartials bytes startIndexForLength endIndexForPartials
    (numberOfDataOctets, numberOfOctetsEncodingTheLength)


let getNumberOfDataOctets (bytes : byte list) startIndexForLenght =
    let firstOctet = bytes.Item startIndexForLenght
    let valueFromThe7Bits = (int)((firstOctet <<< 1) >>> 1)
    if (int)firstOctet < 128
    then
        let totalNumberOfBytesUsedForLength = 1
        (valueFromThe7Bits, totalNumberOfBytesUsedForLength)
    else 
        let (numberOfDataOctets, numberOfOctetsEncodingTheLength) = getLongFormLength bytes valueFromThe7Bits (startIndexForLenght + 1)
        let totalNumberOfBytesUsedForLength = numberOfOctetsEncodingTheLength + 1
        (numberOfDataOctets, totalNumberOfBytesUsedForLength)


let parseBerInfo bytes startIndex =
    let (applicationTag, bytesUsedForTag) = getApplicationTag bytes startIndex
    let startIndexForLenght = startIndex + bytesUsedForTag
    let (numberOfDataOctets, numberOfOctetsEncodingTheLength) = getNumberOfDataOctets bytes startIndexForLenght
    let startIndexOfDataOctets = startIndexForLenght + numberOfOctetsEncodingTheLength 
    let endIndexOfThisObject = startIndexOfDataOctets + numberOfDataOctets
    (applicationTag, startIndexOfDataOctets, endIndexOfThisObject) 

let parseBatchControlInfo bytes startIndex =
    let (applicationTag, senderDataOctetsStartIndex, senderEndIndex) = parseBerInfo bytes startIndex
    if applicationTag <> 196 //Sender
    then failwith "Wrong application tag in batch control info"
    else
        let theString = System.Text.Encoding.ASCII.GetString(List.toArray(getBytes bytes senderDataOctetsStartIndex senderEndIndex))
        let (applicationTag, recipientDataOctetStartIndex, recipientEndIndex) = parseBerInfo bytes senderDataOctetsStartIndex        
        (BatchControlInfo, senderEndIndex)

let parseAccountingInfo bytes startIndex  =
    let (applicationTag, accountingInfoDataOctetsStartIndex, accountingInfoEndIndex) = parseBerInfo bytes startIndex
    if applicationTag <> 5 //AccountingInfo
    then failwith "Wrong application tag in batch control info"
    else (AccountingInfo, accountingInfoEndIndex)

let parseNetworkInfo bytes startIndex =
    let (applicationTag, nextStartIndex, networkInfoEndIndex) = parseBerInfo bytes startIndex
    if  enum<ApplicationTags>(applicationTag) <> ApplicationTags.NetworkInfoTag 
    then failwith "Wrong application tag in networkInfo"
    else 
        let networkInfoChoiceList = [(UtcTimeOffsetInfoList, RecEntityInfoList)]
        (NetworkInfo(networkInfoChoiceList), networkInfoEndIndex)

let parseMessageDescriptionInfo bytes startIndex =
    let (applicationTag, nextStartIndex, messageDescriptionInfoEndIndex) = parseBerInfo bytes startIndex
    if applicationTag <> 8 //MessageDescriptionInfo
    then 
        failwith "Wrong application tag in MessageDescriptionInfo"
    else 
        (MessageDescriptionInfo, messageDescriptionInfoEndIndex)

let parseCallEventDetails bytes startIndex =
    let (applicationTag, nextStartIndex, callEventDetailsEndIndex) = parseBerInfo bytes startIndex
    (CallEventDetails([CallEventDetail(MobileOriginatedCall(DateTime.UtcNow, "MobileOriginatedCall"))]), callEventDetailsEndIndex)

let parseAuditControlInfo bytes startIndex =
    let (applicationTag, nextStartIndex, auditControlInfoEndIndex) = parseBerInfo bytes startIndex
    (AuditControlInfo, auditControlInfoEndIndex)

let parseTransferBatch bytes startIndex endIndex =
    let (applicationTag, batchControlDataOctetsStartIndex, accountingInfoStartIndex) = parseBerInfo bytes startIndex
    if applicationTag = BatchControlInfoTag
    then 
        let (batchControlInfo, _)                                = parseBatchControlInfo       bytes batchControlDataOctetsStartIndex 
    else
    let (accountingInfo, networkInfoStartIndex)              = parseAccountingInfo         bytes accountingInfoStartIndex 
    let (networkInfo, messageDescriptionInfoStartIndex)      = parseNetworkInfo            bytes networkInfoStartIndex 
    let (messageDescriptionInfo, callEventDetailsStartIndex) = parseMessageDescriptionInfo bytes messageDescriptionInfoStartIndex
    let (callEventDetails, auditControlInfoStartIndex)       = parseCallEventDetails       bytes callEventDetailsStartIndex 
    let (auditControlInfo, _)                                = parseAuditControlInfo       bytes auditControlInfoStartIndex
    TransferBatch(batchControlInfo, accountingInfo, networkInfo, messageDescriptionInfo, callEventDetails, auditControlInfo)

let parseNotification bytes startIndex endIndex = 
    let (applicationTag, nextStartIndex, nextEndIndex) = parseBerInfo bytes startIndex
    Notification("Notification")

let parseDataInterchange bytes =
    let startIndex = 0
    let (applicationTag, nextStartIndex, nextEndIndex) = parseBerInfo bytes startIndex
    match applicationTag with
    |1 -> DataInterchange (parseTransferBatch bytes nextStartIndex nextEndIndex)
    |2 -> DataInterchange (parseNotification bytes nextStartIndex nextEndIndex)
    |_ -> failwith "Error passing ApplicationContent"


//let parse (bytes : byte list) =
//    let identifierClass = parseClass bytes.Head
//    match identifierClass with
//        | Universal -> failwith "Parsing of Universal not supported yet (if ever...)" //parseUniversalContent bytes
//        | Application -> parseDataInterchange bytes
//        | _ -> failwith "Error parsing"
//   
//    