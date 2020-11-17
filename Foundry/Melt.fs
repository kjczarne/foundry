namespace Foundry

open System.Text.RegularExpressions
open Mold
open Config
open Regex

/// <summary>
/// This module contains the *melting* logic, i.e. how
/// the translating functions should implement transformations
/// from the source format into an intermediate Record
/// representation.
/// </summary>
module Melt =

    ()