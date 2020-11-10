namespace Foundry

open Regex
open Mold

module App =

    [<EntryPoint>]
    let main args =
        printfn "%s" "hello world!"
        let str = """
# Arbitrarily deep ⚗️

## Multiple levels

### Of parsing

- Sounds fun

    Undoubtedly

### Another batch

- Should be

    Melted separately

⚗️
"""
        // let simplerMeltingPatterns =
        //     [ @"\w";  ]
    
        carveMoldMelt defaultMultilevelMarkdownMold
        |> printfn "%A"

        // interpolateMarkers (regexMoldInterpolationMap "⚗️") defaultMultilevelMarkdownMold
        // |> printfn "%A"

        // let k, v = Map.toSeq (regexMoldInterpolationMap "⚗️") |> Seq.item 0
        // replaceRegex k v defaultMultilevelMarkdownMold
        // |> printfn "%A"

        Melt.melt (Melt.meltingPatterns Config.defaultConfig defaultMultilevelMarkdownMold) str []
        |> printfn "%A"

        0