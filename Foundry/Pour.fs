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
    

    // FIXME: we're losing information about the header level right now!!
    //        probably the parser will need to be context-aware to know how many #'s it witnessed
    //        unless we limit the grammar up to 6 marks
    //
    //        we will not guarantee accurracy if user wants to have two disjoint
    //        trees within one snippet, that should be discouraged

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
