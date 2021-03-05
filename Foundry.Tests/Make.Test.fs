namespace Foundry.Tests

open Make
open Model
open Parse
open FParsec
open System
open NUnit.Framework

        

[<TestFixture>]
type MakeTest () =

    /// <summary>
    /// Should produce a valid ouptut from a parsed list of identifiers
    /// </summary>
    [<Test>]
    member this.TestMakeSimpleOutput () =
        let parsedList = [
            T (Title "Some title")
            Q (Question "What is the meaning of life?")
            A (Answer "42")
            A (Answer "69") ]
        let expected = 
            """Some title,What is the meaning of life?,42,69"""
        let outputTemplate = """{Title},{Question},{Answer}"""
        let actual = Make.makeCustom "\r\n" parsedList outputTemplate
        Assert.That(actual, Is.EqualTo(expected))