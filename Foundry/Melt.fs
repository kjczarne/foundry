namespace Foundry

open System.Text.RegularExpressions
open Mold
open Config
open Regex

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


    let rec meltBatches pattern resource =
        ()

    let rec meltRecords patternQuestion patternAnswer resource =
        ()


    let melt config mold resource =
        // Convert tuple to seq, map interpolation to each element:
        let pBatch, pQuestion, pAnswer = 
            carveMoldMelt mold 
            |> fun (i, j, k) -> seq { i; j; k; }
            |> Seq.map (interpolateMarkers <| 
                        regexMoldInterpolationMap config.MagicMarker)
            |> fun i -> Seq.item 0 i, Seq.item 1 i, Seq.item 2 i  // back to tuple
        // Melt walking through the parse tree:
        // TODO: recursively parse records until there are no more,
        //       same for batches, i.e. `meltBatches` and `meltRecords`
        //       should be recursive
        meltBatches pBatch resource |> meltRecords pQuestion pAnswer