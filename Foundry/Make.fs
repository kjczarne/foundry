module Make
open Model
open FParsec
open Parse
open System

let make separator parsedList outputTemplate =
    let templateTree = match run (pFull separator) outputTemplate with
                       | Success(result, _, _) -> result
                       | Failure(errorMessage, _, _) -> raise (Exception($"The template {outputTemplate} could not be parsed"))
    let walk func expr =
        match expr with
        | T s ->  match s with
                  | Title s2 -> func s2
        | TM s -> match s with
                  | TitleM s2 -> func s2
        | Q s ->  match s with
                  | Question s2 -> func s2
        | A s ->  match s with
                  | Answer s2 -> func s2
        | _ -> raise (Exception("not supported yet"))

    let extractStr = walk id
    let prependFromTemplate prependToken =
        walk (fun i -> (walk id prependToken) + i)
        // prependFromTemplate (T(Title(","))) (T(Title("TITITI"))) --> ",TITITI"
    let findCorrespondingTokenInTemplate tokenToMatch =
        templateTree |> Seq.tryFind (fun i -> i.GetType() = tokenToMatch.GetType())
    
    let prepend i =
        match (findCorrespondingTokenInTemplate i) with
        | Some t -> prependFromTemplate t i
        | None -> extractStr i
    String.Join("", [ for i in parsedList do
                      prepend i ])