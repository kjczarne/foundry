namespace Foundry

module Config =

    type Config =
        { MagicMarker : string }

    let defaultConfig =
        { MagicMarker = "⚗️" }