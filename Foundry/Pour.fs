namespace Foundry

open Mold
open Melt

/// <summary>
/// This module represents the *pouring* logic, i.e. how the
/// intermediate Record representation will be injected into
/// a template (*mold*) and how the output should be interpreted
/// (e.g. is it a file, is it a REST API request, is it
/// supposed to end up in a database, etc.)
/// </summary>
module Pour =

    let pourText mold (outputPath: string) =
        ()

    let pourBinary mold (outputPath: string) =
        ()

    /// <summary>
    /// Describes the expected raw output from a parser, where the
    /// following conformation is expected:
    /// `[ ( TreeCategories list, Records list ), ... ]`, where the
    /// `Records list` should be understood as:
    /// `[ ( Question, Answer list ) ]`.
    /// A visual representation of this sort of structure would be:
    /// ```
    /// [ ( TreeCategories list, Records list ) ]
    ///                                |
    ///                                |
    ///                    [ ( Question, Answer list ) ]
    /// ```
    /// 
    /// Each parser output should conform to this structure to be
    /// able to inject it into the `pour` funtion that converts
    /// this intermediate representation into a more human-readable
    /// intermediate representation of a sequence of `Record` instances.
    /// </summary>
    type ParserRawOutput = List<List<string> * List<string * string list>>

    let example =[(["Arbitrarily deep "; "Multiple levels"; "Of parsing"],
                   [("Sounds fun", ["Undoubtedly"])]);
                  (["Another batch"], [("Should be", ["Melted separately"])])]

    /// <summary>
    /// Takes in a `ParserRawOutput` and spews out
    /// a `seq<Record>`.
    /// <param name="parsingOutput">`ParserRawOutput`, whatever
    /// is returned from the melting funtions that conforms
    /// to this type </param>
    /// </summary>
    let pour (parsingOutput : ParserRawOutput) =
        seq { for tree in parsingOutput do
              let categories, records = tree
              for record in records do
                  let question, answers = record
                  let questionList = [ question ]
                  let r = { TreeCategories = categories 
                            Id = ""
                            Tags = [ ]
                            Fields = (List.append questionList answers) }
                  yield r}