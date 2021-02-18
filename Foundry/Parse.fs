module Parse
open FParsec
open Config
open Model
open System


/// <summary>
/// Creates a marker parser and returns a wrapped value
/// within the parser if the value was given. Otherwise
/// it returns the domain model component with an empty
/// string
/// </summary>
/// <param name="tagStr">identifying string of the tag,
/// e.g. BEGIN, END, Answer, Question, etc.
/// </param>
let pMarker tagStr = between (pstring <| fst markerBrackets) 
                             (pstring <| snd markerBrackets) <|
                             (pstring (tagStr + ":") 
                                 >>. (noneOf <| snd markerBrackets |> many1Chars ) 
                             <|> (pstring tagStr >>. pstring ""))

let pBeginMarker'    = pMarker markerIds.Begin            |>> BeginM    |>> BM
let pEndMarker'      = pMarker markerIds.End              |>> EndM      |>> EM
let pTitleMarker'    = pMarker markerIds.Title            |>> TitleM   |>> TM
let pQuestionMarker' = pMarker markerIds.Question         |>> QuestionM |>> QM
let pAnswerMarker'   = pMarker markerIds.Answer           |>> AnswerM  |>> AM

let pMarkers = choice [
    attempt pBeginMarker'
    attempt pTitleMarker'
    attempt pQuestionMarker'
    attempt pAnswerMarker'
    attempt pEndMarker'
]

let pTupleMarker pSeparator pMarker tCast tCast2 = 
    (((noneOf <| (fst markerBrackets + snd markerBrackets)) 
        |> many1Chars) |>> tCast |>> tCast2) .>> pMarker .>> opt (regex pSeparator)
    <|> pMarker .>> opt (regex pSeparator)

let pTupleBeginMarker pSeparator = pTupleMarker pSeparator pBeginMarker' Begin B
let pTupleEndMarker pSeparator = pTupleMarker pSeparator pEndMarker' End E
let pTupleTitleMarker pSeparator = pTupleMarker pSeparator pTitleMarker' Title T
let pTupleQuestionMarker pSeparator = pTupleMarker pSeparator pQuestionMarker' Question Q
let pTupleAnswerMarker pSeparator = pTupleMarker pSeparator pAnswerMarker' Answer A

let pFull pSeparator = (many1 <| choice [
    attempt <| pTupleBeginMarker pSeparator
    attempt <| pTupleTitleMarker pSeparator
    attempt <| pTupleQuestionMarker pSeparator
    attempt <| pTupleAnswerMarker pSeparator
    attempt <| pTupleEndMarker pSeparator
    // attempt <| (newline <|> doubleNewline) >>% N
    // attempt <| pSeparator >>% N
    // attempt doubleNewline >>. preturn N
])

let tokenize (input : string) =
    input.Split(
        [|"\r\n"; "\r"; "\n"|], 
        StringSplitOptions.None)

let sampleParser pSeparator v tCast tCast2 =
    attempt (regex v >>. manyChars (noneOf pSeparator) |>> tCast |>> tCast2)

let processTemplate pSeparator template =
    match run (pFull pSeparator) template with
    | Success(result, _, _) -> 
        [ for i in result do
            match i with
            | BM v -> 
                match v with
                | BeginM v2    -> yield "pBeginMarker",    sampleParser pSeparator v2 BeginM BM
            | EM v -> 
                match v with
                | EndM v2      -> yield "pEndMarker",      sampleParser pSeparator v2 EndM EM
            | QM v ->
                match v with
                | QuestionM v2 -> yield "pQuestionMarker", sampleParser pSeparator v2 QuestionM QM
            | AM v ->
                match v with
                | AnswerM v2   -> yield "pAnswerMarker",   sampleParser pSeparator v2 AnswerM AM
            | TM v -> 
                match v with
                | TitleM v2    -> yield "pTitleMarker",    sampleParser pSeparator v2 TitleM TM
            | B v -> 
                match v with
                | Begin v2     -> yield "pBegin",          sampleParser pSeparator v2 Begin B
            | E v ->
                match v with
                | End v2       -> yield "pEnd",            sampleParser pSeparator v2 End E
            | Q v ->
                match v with
                | Question v2  -> yield "pQuestion",       sampleParser pSeparator v2 Question Q
            | A v ->
                match v with
                | Answer v2    -> yield "pAnswer",         sampleParser pSeparator v2 Answer A
            | T v ->
                match v with
                | Title v2     -> yield "pTitle",          sampleParser pSeparator v2 Title T
            | N _              -> yield "pN",              preturn N
            | Error _ -> 
                raise (Exception("Invalid pattern matching!")) ]
    | Failure(errorMsg, _, _) -> 
        raise (Exception(errorMsg))
        // ["pN", preturn N]

let combinators pSeparator template = 
    processTemplate pSeparator template |> Map.ofSeq
// let combinators : Map<string,Parser<obj,unit>> = 
//     getTemplateValues () |> Map.ofSeq
// let pBeginMarker template = (combinators template) |> Map.tryFind "pBeginMarker"
// let pEndMarker template = (combinators template) |> Map.tryFind "pEndMarker"
// let pQuestionMarker pSeparator template = (combinators pSeparator template) |> Map.find "pQuestionMarker"
// let pAnswerMarker pSeparator template = (combinators pSeparator template) |> Map.find "pAnswerMarker"
// let pTitleMarker pSeparator template = (combinators pSeparator template) |> Map.find "pTitleMarker"
// let pBegin pSeparator template = (combinators pSeparator template) |> Map.find "pBegin"
// let pEnd pSeparator template = (combinators pSeparator template) |> Map.find "pEnd"
let pQuestion pSeparator template = (combinators pSeparator template) |> Map.find "pQuestion"
let pAnswer pSeparator template = (combinators pSeparator template) |> Map.find "pAnswer"
let pTitle pSeparator template = (combinators pSeparator template) |> Map.find "pTitle"

let pParseQualifier pSeparator template =
    ((combinators pSeparator template) 
    |> Map.toSeq 
    |> Seq.map (fun (i, j) -> j))
    |> choice 
    |> many1
    // many1 <| choice [
    // //     pBeginMarker template <!> "BM"
    // //     pEndMarker template <!> "EM"
    //     // pQuestionMarker pSeparator template <!> "QM"
    //     // pAnswerMarker pSeparator template <!> "AM"
    //     // pTitleMarker pSeparator template <!> "TM"
    //     // pBegin pSeparator template <!> "B"
    //     // pEnd pSeparator template <!> "E"
    //     pTitle pSeparator template //<!> "T"
    //     pQuestion pSeparator template //<!> "Q"
    //     pAnswer pSeparator template //<!> "A"
        
    // ]
