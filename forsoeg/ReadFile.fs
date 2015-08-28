module ReadFile

open System
open System.IO

//let readValue (reader:BinaryReader) cellIndex = 
//    reader.BaseStream.Seek(int64 (cellIndex*4), SeekOrigin.Begin) |> ignore
//    match reader.ReadInt32() with
//    | Int32.MinValue -> None
//    | v -> Some(v)
//
//        
// Use list or array to force creation of values (otherwise reader gets disposed before the values are read)
//let readValues indices fileName = 
//    use reader = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
//    let values = Array.map (readValue reader) indices
//    values
//

let readAllBytesFromGivenFile fileNameWithPath =
    use reader = new BinaryReader(File.Open(fileNameWithPath, FileMode.Open, FileAccess.Read, FileShare.Read))
    let allValues = File.ReadAllBytes(fileNameWithPath)
    allValues

