namespace Foundry

open FParsec

module MdParser =
    type UserState = unit
    type Parser<'t> = Parser<'t, UserState>

    let parseMagic : Parser<_> = pstring "⚗️"
    let doubleNewline : Parser<_> = newline .>> newline
    let parseAllUntilNewline pPreceding = pPreceding >>. manyCharsTill anyChar doubleNewline
    let parseHeadingSymbols : Parser<_> = many1 (pstring "#") >>. (pstring " ")
    let parseAnyHeading : Parser<_> = parseHeadingSymbols |> parseAllUntilNewline

    let parseQuestion : Parser<_> = pstring "- " |> parseAllUntilNewline
    let parseAnswer : Parser<_> = pstring "    " |> parseAllUntilNewline

    let parseRecord : Parser<_> = parseQuestion .>>. (many1 parseAnswer)

    let parseRecordWithHeading : Parser<_> = parseAnyHeading .>>. parseRecord
    let parseRecordWithOneOrMoreHeading = many1 parseAnyHeading .>>. parseRecord
        
    let parseRecordsWithOneOrMoreHeading = many1 parseAnyHeading .>>. (many1 parseRecord)

    let parseManyRecords = many1 parseRecordsWithOneOrMoreHeading
        
    let parseFoundrySnippet = skipManyTill anyChar (parseMagic .>> newline) >>. parseManyRecords
