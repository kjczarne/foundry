namespace Foundry

open System.Text.RegularExpressions

/// <summary>
/// This module wraps particular functions
/// from the `System.Text.RegularExpressions`
/// namespace to allow more functional approach
/// to Regular Expression matching in this
/// application.
/// </summary>
module Regex =
    
    /// <summary>
    /// Matches all occurences and returns a list
    /// of them.
    /// <param name="regex">Regular expression
    /// written with a `@""` string.</param>
    /// <param name="str"> String to match.
    /// </param>
    /// </summary>
    let parseRegex regex str =
        [for m in Regex(regex).Matches(str) do m.Value]

    /// <summary>
    /// Replaces all occurences of a matched
    /// Regex pattern.
    /// <param name="regex">Regular expression
    /// written with a `@""` string.</param>
    /// <param name="repl">Replacement string.
    /// </param>
    /// <param name="str"> String to match.
    /// </param>
    /// </summary>
    let replaceRegex regex (repl: string) str=
        Regex.Replace(str, regex, repl)

    /// <summary>
    /// Splits string into a list on each match
    /// of a Regex pattern.
    /// <param name="regex">Regular expression
    /// written with a `@""` string.</param>
    /// <param name="str"> String to match.
    /// </param>
    /// </summary>
    let splitRegex regex str =
        Regex.Split(str, regex) |> Array.toList

    /// <summary>
    /// Deprecated.
    /// Splits on a magic template pattern and
    /// returns a 3-tuple with elements before
    /// first magic pattern, between the two and
    /// after the second *Magic Marker*.
    /// <param name="str"> String to match.
    /// </param>
    /// </summary>
    let splitOnMagic str =
        let s = splitRegex @"\$\{MAGIC\}" str
        s |> List.head, s |> List.item 1, s |> List.item 2