module Make
open Model
open FParsec
open Parse
open System
open Newtonsoft.Json

let makeCustom separator parsedList outputTemplate =
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
        | QM s -> match s with
                  | QuestionM s2 -> func s2
        | A s ->  match s with
                  | Answer s2 -> func s2
        | AM s -> match s with
                  | AnswerM s2 -> func s2
        | B s ->  match s with
                  | Begin s2 -> func s2
        | BM s -> match s with
                  | BeginM s2 -> func s2
        | E s ->  match s with
                  | End s2 -> func s2
        | EM s -> match s with
                  | EndM s2 -> func s2
        | Model.Error s ->  raise ( Exception(s) )
        | N -> func ""

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

type Outputs =
    | Csv
    | Custom of string
    | Json

let csv = """{Title}
{Question},{Answer}"""

let make ouptutType inputTemplate inputFileContents =
    let parsedList = parse "\r\n" inputTemplate inputFileContents

    match ouptutType with
    | Custom s -> makeCustom "\r\n" parsedList s
    | Csv -> makeCustom "\r\n" parsedList csv
    | Json -> JsonConvert.SerializeObject(parsedList, Formatting.Indented)