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
    /// <typeparam name="Tags">Linear list of string categories.
    /// Specified by the user when parsing and shouldn't
    /// rely on the contents of the file, at least not
    /// in the implementation details of this application.</typeparam>
    /// </summary>
    type Record =
        { Id : string
          Fields : string list
          TreeCategories : string list
          Tags : string list }

    /// <summary>
    /// This function takes in a `Config` object
    /// and a *Mold* string as arguments and produces
    /// a tuple of patterns that will be used
    /// at each respective step of the parsing tree
    /// traversal.
    /// <param name="config">A `Config` object</param>
    /// <param name="mold">*Mold* string template</param>
    /// <returns>`string * string seq * string * string`
    /// This tuple is decomposable as follows:
    /// * the first pattern matches batches
    /// * the following patterns match the tree hierarchy
    ///   within a batch
    /// * the next-to-last pattern matches questions
    ///   (it's expected to have **only one** question
    ///   per Record)
    /// * the last pattern matches answers (it's
    ///   expected to have **one or more** answers
    ///   per Record).</returns>
    /// </summary>
    let meltingPatterns config mold = 
        let interpolator template = interpolateMarkers
                                        (regexMoldInterpolationMap config.MagicMarker) 
                                        template
        carveMoldMelt mold 
        |> fun (i, j, k, l) -> seq { seq { interpolator i }
                                     (Seq.map interpolator j)
                                     seq { interpolator k
                                           interpolator l } } |> Seq.concat

    // /// <summary>
    // /// Melts recursively traversing only one edge of the tree.
    // /// </summary>
    // let rec preMelt (patterns : seq<string>) (resource : string) =

    // TODO: try FParsec with full Markdown Spec
    let rec melt (patterns : seq<string>) resource accumulator =
        let p =
            match Seq.tryHead patterns with
            | None -> ""
            | Some p -> p
        let m = 
            parseRegex p resource |> Seq.toList
        let updatedAcc = Seq.append m accumulator
        match Seq.length m with
        | 0 -> updatedAcc
        | _ -> melt (Seq.tail patterns) (Seq.last updatedAcc) updatedAcc