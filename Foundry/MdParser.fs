namespace Foundry

open FParsec
open Mold

module MdParser =
    type UserState = unit
    type Parser<'t> = Parser<'t, UserState>

    let (doubleNewline : Parser<_>) = newline .>> newline
    let pUntilDoubleNewline pPreceding = pPreceding >>. manyCharsTill anyChar doubleNewline

    let pHeading = choice [
        pstring "###### " |> pUntilDoubleNewline |>> Heading6
        pstring "##### " |> pUntilDoubleNewline |>> Heading5
        pstring "#### " |> pUntilDoubleNewline |>> Heading4
        pstring "### " |> pUntilDoubleNewline |>> Heading3
        pstring "## " |> pUntilDoubleNewline |>> Heading2
        pstring "# " |> pUntilDoubleNewline |>> Heading1 ]

    let pQuestion = pstring "- " |> pUntilDoubleNewline |>> Question
    let pAnswer = pstring "    " |> pUntilDoubleNewline |>> Answer

    let pRecord = pQuestion .>>. many1 pAnswer |>> Record

    let pRecords = many1 pRecord

    let pBranch = manyTill pHeading (lookAhead pRecords) .>>. pRecords |>> Branch

    let pTree = many1 pBranch |>> Tree

    let pMagic = pstring "⚗️"
    
    let pP = pMagic >>. newline >>. pTree .>> pMagic