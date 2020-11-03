namespace Foundry

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
        Mold.carveMoldToRegex "⚗️" defaultMarkdownMold
        |> printfn "%A"
        0