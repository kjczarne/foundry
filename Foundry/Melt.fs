namespace Foundry

open System.Text.RegularExpressions
open Mold

/// <summary>
/// This module contains the *melting* logic, i.e. how
/// the translating functions should implement transformations
/// from the source format into an intermediate Record
/// representation.
/// </summary>
module Melt =

    /// <summary>
    /// The intermediate representation of the transformed data.
    /// <typeparam name="Id"></typeparam>
    /// <typeparam name="Fields">List of data fields 
    /// (at least 2 fields are required to transform a Record 
    /// into a standard 2-dimensional flashcard)</typeparam>
    /// <typeparam name="Timestamp">Timestamp of record creation</typeparam>
    /// <typeparam name="TreeCategories">Hierarchical top-down 
    /// list of string categories.</typeparam>
    /// <typeparam name="Tags">Linear list of string categories.</typeparam>
    /// </summary>
    type Record =
        { Id : string
          Fields : List<string>
          TreeCategories : List<string>
          Tags : List<string> }

    type Batch =
        { Id : string
          Records : List<Record> }

    // let (|ParseRegex|_|) regex str =
    //     let mCol = Regex(regex).Matches(str)
    //     match mCol.Count with
    //     | 0 -> None
    //     | _ -> Some [for m in mCol do m.Value]

    // let regexSplit regex str =
    //     let s = Regex.Split(str, regex)
    //     if s.Length = 0
    //     then None
    //     else s |> Array.toList |> Some
    
    // module TextMelt =

    //     let matchPartition mold (str: string) =
    //         match str with
    //         | ParseRegex (makeRegex mold mold.HeaderSpec) m -> m
    //         | ParseRegex (makeRegex mold mold.QuestionUnitSpec) m -> m
    //         | ParseRegex (makeRegex mold mold.AnswerUnitSpec) m -> m
    //         | _ -> []

    //     let parseHeadings listOfBatchesAndHeadings =
    //         [ for idx, element in (listOfBatchesAndHeadings |> List.indexed) do 
    //           if idx % 2 = 1 then yield element ]

    //     let parseBatches listOfBatchesAndHeadings =
    //         [ for idx, element in (listOfBatchesAndHeadings |> List.indexed) do 
    //           if idx % 2 = 0 then yield element ]

    //         // [(0, "dasda")
    //         // (2, "dasdasd")]

    //     /// <summary>
    //     /// This function is used to transform a text
    //     /// resource into an intermediate `Record` representation.
    //     /// The pipeline it follows:
    //     /// 1. Find all matching structures delimited by the *Magic Marker*.
    //     /// 2. Look back from each odd match of the *Magic Marker* to try and
    //     ///    find the header that precedes a section. If a header is not found,
    //     ///    write an empty string instead of a header into the `Record` instance.
    //     /// 3. Compartmentalize the aforementioned structures into a `List<string>`,
    //     ///    with the headers stored in a separate transient `List<string>`.
    //     /// 4. For each string in the data list, apply `QuestionUnitRegex` to obtain
    //     ///    the spec of the first data field that by convention represents a
    //     ///    question unit. Compartmentalize underlying answer units on the question
    //     ///    unit, i.e. feed through all the potential answer units with the
    //     ///    question unit stripped out.
    //     /// 5. 
    //     let melt (mold: TextMold) resource =
    //         match regexSplit mold.MagicMarker resource with
    //         | Some listOfBatchesAndHeadings -> parseBatches
    //         | None -> List.empty
    //         // ["pre # title", "records", 
    //         // "pre # title", "records"]

    // let melt mold resource =
    //     match mold with
    //     | MarkdownMold mold -> TextMelt.melt mold resource

    