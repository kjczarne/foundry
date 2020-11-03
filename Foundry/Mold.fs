namespace Foundry

open System.Text.RegularExpressions

/// <summary>
/// This module contains shared *mold* logic, i.e.
/// a shape/template which should be followed by some
/// converters. For example a Markdown document can
/// have different formatting for two- or multi-dimensional
/// Records:
/// ```md
/// - What is the meaning of life?
///     
///     42
/// ```
/// or
/// ```md
/// * What is the meaning of life?
///     42
/// ```
/// Notice different formatting in both cases. Both units
/// of information are equivalent but the users may need one
/// or another formatting of the Record to plug it seamlessly
/// into their knowledge consumption systems. At the same time
/// some of the programs used by the users may use a particular
/// way of formatting a Markdown document on export, so a flexible
/// spec is needed to be translate both from and into a given format.
/// </summary>
module Mold =

    /// <summary>
    /// A Mold for textual data.
    /// 
    /// <typeparam name="MagicMarker">The marker placed
    /// in a text document to be detected by Foundry
    /// as a paren-like delimiter of data to be parsed.
    /// By convention, placed after the header text
    /// and at the end of a parse block. This is used
    /// to delimit what portion of the document will be
    /// actually parsed instead of always expecting
    /// a dedicated document. This way users can choose
    /// to have data conversible by Foundry embedded in
    /// any document of their choice.</typeparam>
    /// <typeparam name="RegexWrapper">
    /// A wrapper to transform a simple spec into a
    /// Regex pattern. It declares a `@@repl@@` tag in
    /// the place where substitution should occur to
    /// derive a valid Regular Expression.
    /// </typeparam>
    /// <typeparam name="HeaderSpec">
    /// A spec describing what a header of a batch should
    /// look like. A logical counterpart of `Batch` `Id`
    /// field.</typeparam>
    /// <typeparam name="QuestionUnitSpec">
    /// This Spec pattern is used to determine the
    /// logical counterpart of a flashcard's front
    /// face. It is used to determine arrays of Records
    /// in a text document.</typeparam>
    /// <typeparam name="AnswerUnitSpec">
    /// This Spec pattern is used to parse answer
    /// units which are expected to be consistently
    /// identical for a given question unit.</typeparam>
    type TextMold =
        { MagicMarker : string
          RegexWrapper : string
          HeaderSpec : string * string
          QuestionUnitSpec : string * string
          AnswerUnitSpec : string * string }

    type Mold =
        | MarkdownMold of TextMold

    let parseRegex regex str =
        [for m in Regex(regex).Matches(str) do m.Value]

    let replaceRegex regex (repl: string) str=
        Regex.Replace(str, regex, repl)

    let splitRegex regex str =
        Regex.Split(str, regex) |> Array.toList

    let makeRegex mold replTuple =
        let head, trail = replTuple
        mold.RegexWrapper.Replace("@@repl1@@", head).Replace("@@repl2@@", trail)


    let defaultMarkdownMold = """
# ${HEADER} ${MAGIC}

$|R:- ${QUESTION}
$|A:
    ${ANSWER}|:A$
|:R$
${MAGIC}
"""

    /// <summary>
    /// This function should take in a *Mold* spec
    /// and transform it into an intermediate representation
    /// that can be used from within the Regex Parser.
    /// 
    /// The *Mold* spec should be a string that obeys the 
    /// following rules:
    /// * It contains `${HEADER}`, `${MAGIC}`, `${QUESTION}`,
    ///   `${ANSWER}` interpolation tags for the title,
    ///   *Magic Marker*, question field and answer field spec.
    /// * It describes the array of Records and array of Answer
    ///   fields in the following manner:
    ///     * Record array is denoted by `$|R:` and `|:R$` tags.
    ///     * Answer array is denoted by `$|A:` and `|:A$` tags.
    /// 
    /// Example, default Markdown spec:
    /// ```fsharp
    /// """
    /// # ${HEADER} ${MAGIC}
    /// 
    /// $|R:- ${QUESTION}
    /// $|A:
    ///     ${ANSWER}|:A$
    /// |:R$
    /// ${MAGIC}
    /// """
    /// ```
    /// 
    /// The pipeline acts as follows:
    /// 1. The *Magic Marker* is interpolated in the Mold.
    /// 2. The Header, Question and Answer placeholders are
    ///    interpolated with ".+" pattern
    /// 3. Split the input on the *Magic Marker* -> this results
    ///    in a 3-part list: 
    ///    `[pre-header and header pattern; records; post-magic]`
    /// 4. Split each field in that list on the array tokens ->
    ///    this results in the following output list of Regex
    ///    patterns:
    ///    ```fsharp
    ///    [ [ "pre-header and header" ]
    ///      [ "between magic and record"
    ///        "question"
    ///        "between question and answer list" 
    ///        "answer"
    ///        "between answer and end of record list"
    ///        "between record list and magic" ]
    ///      [ "post-magic" ] ]
    ///    ```
    ///    Notice that the middle list is representative of a
    ///    `Record` structure in the file and that the first
    ///    element in the list matches the header of the file.
    /// <param name="mold">`string` mold</param>
    /// </summary>
    let carveMoldToRegex magicMarker mold =
        replaceRegex @"\$\{MAGIC\}" magicMarker mold
        |> replaceRegex @"\$\{HEADER\}" @"(.+)"
        |> replaceRegex @"\$\{QUESTION\}" @"(.+)"
        |> replaceRegex @"\$\{ANSWER\}" @"(.+)"
        |> splitRegex magicMarker
        |> List.map (splitRegex @"\$\|R\:|\$\|A\:|\|\:A\$|\|\:R\$")

    // /// <summary>
    // /// The default Mold for Markdown documents.
    // /// Consistent with the way `toggle`s are dumped
    // /// by Notion into Markdown.
    // /// </summary>
    // let defaultMarkdownMold =
    //     { MagicMarker = @"⚗️"
    //       RegexWrapper = @"(?<=@@repl1@@)\w+.*(?<!@@repl2@@)"
    //       HeaderSpec = @"# ", @"\n\n"
    //       QuestionUnitSpec = @"- ", @"\n\n"
    //       AnswerUnitSpec = @"    ", @"\n\n" }