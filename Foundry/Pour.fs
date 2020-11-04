namespace Foundry

open Mold

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