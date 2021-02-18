module Utilities
open FParsec
open System

let test (p : Parser<'a, unit>) str =
    match run p str with
    | Success(result, _, _)   -> result
    | Failure(errorMsg, _, _) -> raise ( Exception("Parsing failed: " + errorMsg))