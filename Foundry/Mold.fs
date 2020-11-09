namespace Foundry

open Regex

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
/// 
/// Also remember that the mold specifications should be written
/// precisely, *to the tee*, that is e.g. introducing a space
/// after the final marker will mean that the application
/// will always try to match that space. In such case
/// batches that don't have that trailing space,
/// won't be detected. This is a mistake that we made
/// ourselves initially.
/// </summary>
module Mold =

    /// <summary>
    /// Default Markdown Mold consistent with
    /// what Notion returns when using their
    /// Toggle construct.
    /// </summary>
    let defaultMarkdownMold = """
# ${HEADER} ${MAGIC}

${q}-${QUESTION}${q} 
${a}
    ${ANSWER}
${a}
${MAGIC}
"""

    /// <summary>
    /// Default Markdown Mold that takes
    /// into account two possible steps in
    /// the tree hierarchy.
    /// </summary>
    let defaultMultilevelMarkdownMold = """
# ${HEADER} ${MAGIC}

${t1}## ${TREE1}

${t2}### ${TREE2}


${q}-${QUESTION}${q} 
${a}
    ${ANSWER}
${a}
${t2}${t1}
${MAGIC}
"""


    /// <summary>
    /// Default Regex interpolation map for the
    /// interpolable fields. Since the *Magic Marker*
    /// is configurable, this is a function that takes
    /// a *Magic Marker* as a parameter.
    /// <param name="magicMarker">The configured *Magic Marker*
    /// </param>
    /// <returns>Map {Regex pattern to match interpolated field
    /// : Regex pattern to be placed in the interpolated field}
    /// </returns>
    /// </summary>
    let regexMoldInterpolationMap magicMarker =
        Map( seq { @"\$\{MAGIC\}", magicMarker
                   @"\$\{HEADER\}", @"(.+)"
                   @"\$\{QUESTION\}", @"(.+)"
                   @"\$\{ANSWER\}", @"(.+)" 
                   @"\$\{TREE\d+\}", @"(.+)" } )

    /// <summary>
    /// Replaces `${t}`, `${q}` and `${a}` tags with
    /// an empty string.
    /// <param name="s">String template with the tags
    /// to be stripped away</param>
    /// <returns>String, same template but without
    /// the enclosing tags</returns>
    /// </summary>
    let cleanUpEnclosingTags s = replaceRegex @"\$\{([qar]|t\d)\}" "" s

    /// <summary>
    /// Recursively interpolates markers with values
    /// taken from an interpolation map. This can be used to
    /// interpolate regex patterns for parsing or actual
    /// values for pouring into a file
    /// <param name="markerMap">Map {Regex pattern : 
    /// Value to interpolate}</param>
    /// <param name="str">Template string with fields
    /// to interpolate</param>
    /// 
    /// The implementation might seem convoluted but is
    /// actually fairly simple:
    /// 1. Unpack the first element of a Key, Value pair
    ///    *if it exists* into `k` and `v` variables. If
    ///    it doesn't exist just return empty strings.
    /// 2. Use the `k` to match a template marker and
    ///    use the `v` to inject into the `str`.
    /// 3. Match on the count of elements in the map,
    ///    if the map has no more elements, return `repl`.
    ///    If it still has elements, call the 
    ///    `interpolateMarkers` function again with the
    ///    same map but with `k` element removed and with
    ///    `repl` instead of the original string to keep
    ///    interpolating within the *already interpolated*
    ///    string.
    /// </summary>
    let rec interpolateMarkers (markerMap: Map<string, string>) str =
        let s = Map.toSeq markerMap
        let k, v = 
            match Seq.tryHead s with
            | None -> "", ""
            | Some s -> s
        let repl = replaceRegex k v str
        match markerMap.Count with
        | 0 -> repl |> cleanUpEnclosingTags
        | _ -> interpolateMarkers (Map.remove k markerMap) repl

    /// <summary>
    /// This function finds `${t}` tags
    /// and assesses how many tree levels
    /// should be expected from the mold.
    /// <param name="mold">String *Mold*</param>
    /// <returns>Integer count of tree levels</returns>
    /// </summary>
    let treeMarkerCount mold = 
        parseRegex @"\$\{t\d\}" mold
        |> Seq.collect (parseRegex @"\d")
        |> Seq.distinct
        |> Seq.length

    /// <summary>
    /// This function returns a list of Regex patterns
    /// that match increasing levels of the hierarchy tree
    /// in a *Mold*
    /// <param name="mold">*Mold* string</param>
    /// <returns>List of string Regex patterns
    /// matching increasing tree levels</returns>
    /// </summary>
    let treeMarkerPatterns mold =
        [for i in 1..(treeMarkerCount mold) do 
            @"\$\{t" + i.ToString() + @"\}[\s\S]+?\$\{t" + i.ToString() + @"\}"]

    /// <summary>
    /// This function prepares melting Regex patterns.
    /// 
    /// Example, default Markdown spec:
    /// ```fsharp
    /// """
    /// # ${HEADER} ${MAGIC}
    /// 
    /// ${t1}## ${TREE1}
    /// 
    /// ${t2}### ${TREE2}
    /// 
    /// ${r}${q}-${QUESTION}${q} 
    /// ${a}
    ///     ${ANSWER}
    /// ${a}${r}
    /// ${t2}${t1}
    /// ${MAGIC}
    /// """
    /// ```
    /// The pipeline acts as follows:
    /// 1. The first pattern to be returned is a pattern
    ///    that matches a complete batch, i.e.
    ///    `${HEADER}${MAGIC}[\s\S]+${MAGIC}`
    /// 2. The second pattern matches tree hierarchy elements
    ///    within a single batch: `\$\{t\}[\s\S]+?\$\{t\}`
    ///    and trims them to a matching pattern
    /// 2. The second pattern returned is used to match questions
    ///    and is extracted from between the `${q}` tags.
    /// 3. The third pattern returned is used to match answers
    ///    and is extracted from between the `${a}` tags.
    /// <param name="mold">A string *Mold* adherent
    /// to the specification</param>
    /// <returns>`string * string seq * string * string`
    /// The tuple constists of regex patterns used in
    /// the `melt` function:
    /// 1. Pattern matching the *Magic-Marker*-demarcated
    ///    batches.
    /// 2. Sequence of patterns that matches tree hierarchy
    ///    above a record.
    /// 3. Pattern matching questions.
    /// 4. Pattern matching answers.
    /// </returns>
    /// </summary>
    let carveMoldMelt mold =
        
        (parseRegex @"[\s\S]+?\$\{MAGIC\}" mold |> Seq.head) 
            + @"[\s\S]+?${MAGIC}" 
            + (splitRegex @"\$\{MAGIC\}" mold |> Seq.last) |> cleanUpEnclosingTags,
        treeMarkerPatterns mold,
        parseRegex @"\$\{q\}[\s\S]+\$\{q\}" mold |> Seq.head |> cleanUpEnclosingTags,
        parseRegex @"\$\{a\}[\s\S]+\$\{a\}" mold |> Seq.head |> cleanUpEnclosingTags
