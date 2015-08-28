module TapTypes
open System

type CallEventDetailsChoiceItems = 
    | MobileOriginatedCall of (DateTime * string)
    | GprsCall of (DateTime * string * int)

type CallEventDetailChoice = 
    |CallEventDetail of CallEventDetailsChoiceItems

type CallEventDetailSequence = 
    |CallEventDetails of CallEventDetailChoice list

type UtcTimeOffsetInfo = 
    UtcTimeOffsetInfoList

type RecEntityInfo =
    RecEntityInfoList

type NetworkInfoItems = (UtcTimeOffsetInfo * RecEntityInfo)

type NetworkInfoSequence = 
    |NetworkInfo of NetworkInfoItems list

type BatchControlInfoSequence =
    |BatchControlInfo
    
type AccountingInfoSequence =
    |AccountingInfo

type MessageDescriptionInfoSequenceOf =
    |MessageDescriptionInfo

type AuditControlInfoSequence =
    |AuditControlInfo

type DataInterchangeChoiceItems = 
    | TransferBatch of BatchControlInfoSequence * AccountingInfoSequence * NetworkInfoSequence * MessageDescriptionInfoSequenceOf * CallEventDetailSequence * AuditControlInfoSequence
    | Notification of string

type DataInterchangeChoice =
    |DataInterchange of DataInterchangeChoiceItems

type ApplicationTags = 
    | BatchControlInfoTag = 4
    | NetworkInfoTag = 6