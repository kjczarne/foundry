module Config
type MarkerIds = {
    Begin    : string
    End      : string
    Title    : string
    Question : string
    Answer   : string
}

/// <summary>
/// Can be overridden to define own marker IDs 
/// (strings that go inside a marker).
/// </summary>
let markerIds = {
    Begin    = "BEGIN"
    End      = "END"
    Title    = "Title"
    Question = "Question"
    Answer   = "Answer"
}

let markerBrackets = "{", "}"