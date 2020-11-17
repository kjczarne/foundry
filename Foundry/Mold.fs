namespace Foundry

open Regex

/// <summary>
/// </summary>
module Mold =

    type Question = Question of string
    type Answer = Answer of string
    type Record = Record of Question * Answer list
    type Heading =
        | Heading1 of string
        | Heading2 of string
        | Heading3 of string
        | Heading4 of string
        | Heading5 of string
        | Heading6 of string
    type Branch = Branch of Heading list * Record list
    type Tree = Tree of Branch list