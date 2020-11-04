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
                   @"\$\{ANSWER\}", @"(.+)" } )


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
        | 0 -> repl
        | _ -> interpolateMarkers (Map.remove k markerMap) repl


    /// <summary>
    /// Example, default Markdown spec:
    /// ```fsharp
    /// """
    /// # ${HEADER} ${MAGIC}
    /// 
    /// ${q}-${QUESTION}${q} 
    /// ${a}
    ///     ${ANSWER}
    /// ${a}
    /// ${MAGIC}
    /// """
    /// ```
    /// The pipeline acts as follows:
    /// 1. The first pattern to be returned is a pattern
    ///    that matches a complete batch, i.e.
    ///    `${HEADER}${MAGIC}[\s\S]+${MAGIC}`
    /// 2. The second pattern returned is used to match questions
    ///    and is extracted from between the `${q}` tags.
    /// 3. The third pattern returned is used to match answers
    ///    and is extracted from between the `${a}` tags.
    /// 4. `${HEADER}`, `${QUESTION}` and `${ANSWER}` are interpolated
    ///    with `.+`
    /// 5. `${MAGIC}` is interpolated with the *Magic Marker*
    /// 6. All interpolated patterns are returned
    let carveMoldMelt magicMarker mold =
        (parseRegex @"[\s\S]+?\$\{MAGIC\}" mold |> List.head) +
        @"[\s\S]+\$\{MAGIC\}" + (splitRegex @"\$\{MAGIC\}" mold |> List.last),
        parseRegex @"\$\{q\}[\s\S]+\$\{q\}" mold |> List.head,
        parseRegex @"\$\{a\}[\s\S]+\$\{a\}" mold |> List.head
