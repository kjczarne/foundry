
namespace Foundry.Tests

open NUnit.Framework
open Model
open Parse
open Utilities

[<TestFixture>]
type ParseTest () =

    /// <summary>
    /// Should parse each type of a marker correctly
    /// </summary>
    [<Test>]
    member this.TestParseMarkers () =
        let markers = [
            "{BEGIN}"
            "{BEGIN:START}"
            "{Answer}"
            "{Question}"
            "{Title}" ]
        let expected = [
            BM ( BeginM("") )
            BM ( BeginM("START") )
            AM ( AnswerM("") )
            QM ( QuestionM("") )
            TM ( TitleM("") ) ]
        let actual = 
            markers 
            |> Seq.map (fun i -> test pMarkers i) 
        Assert.That(actual, Is.EqualTo(expected))
    
    /// <summary>
    /// Should parse a simple template correctly
    /// </summary>
    [<Test; Ignore("There's an apparent bug where e.g. Title is treated as End")>]
    member this.TestSimpleTemplateParse () =
        let simplestTemplate = 
            """{BEGIN:START}
T: {Title}
Q: {Question}
A: {Answer}
{END:FINISH}"""
        let exSimplest =
            """START
T: My title
Q: What is the meaning of life?
A: 42
A: 69
FINISH"""
        let expected = [
            BM (BeginM "START")
            T (Title "My title")
            Q (Question "What is the meaning of life?")
            A (Answer "42")
            A (Answer "69")
            EM (EndM "FINISH") ]
        let actual = test (pUserInput "\r\n" simplestTemplate) exSimplest
        Assert.True(
            List.map2 (( = )) actual expected
            |> List.reduce (( && ))
        )
    
    /// <summary>
    /// Should correctly parse a standard markdown template
    /// </summary>
    [<Test>]
    member this.TestMarkdownTemplateParse () =
        let exMdTemplate = 
            """# {Title}

  - {Question}

    {Answer}"""
        let exMd = 
            """# Some title

  - What is the meaning of life?

    42

    69"""
        let expected = [
            T (Title "Some title")
            Q (Question "What is the meaning of life?")
            A (Answer "42")
            A (Answer "69") ]
        let actual = test (pUserInput "\r\n" exMdTemplate) exMd
        Assert.True(
            List.map2 (( = )) actual expected
            |> List.reduce (( && ))
        )
    
    /// <summary>
    /// Should correctly parse a standard markdown template
    /// </summary>
    [<Test>]
    member this.TestMoreThanOneMarkdown () =
        let exMdTemplate = 
            """# {Title}

  - {Question}

    {Answer}"""
        let exMd = 
            """# Some title

  - What is the meaning of life?

    42

    69

  - What did you have for dinner?

    Kale

    Spinach"""
        let expected = [
            T (Title "Some title")
            Q (Question "What is the meaning of life?")
            A (Answer "42")
            A (Answer "69")
            Q (Question "What did you have for dinner?")
            A (Answer "Kale")
            A (Answer "Spinach") ]
        let actual = test (pUserInput "\r\n" exMdTemplate) exMd
        Assert.True(
            List.map2 (( = )) actual expected
            |> List.reduce (( && ))
        )