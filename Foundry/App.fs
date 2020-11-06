namespace Foundry

open Regex
open Mold

module App =

    [<EntryPoint>]
    let main args =
        printfn "%s" "hello world!"
        let str = """
# Ich bin ⚗️

- Wer bist du

    Ich bin ich.

    Ich bin der Teufel.

- Wer ist sie

    Sie ist ich.

    Sie isst mich.

⚗️   
"""
        carveMoldMelt defaultMarkdownMold
        |> printfn "%A"

        interpolateMarkers (regexMoldInterpolationMap "⚗️") defaultMarkdownMold
        |> printfn "%A"

        let k, v = Map.toSeq (regexMoldInterpolationMap "⚗️") |> Seq.item 0
        replaceRegex k v defaultMarkdownMold
        |> printfn "%A"
        0