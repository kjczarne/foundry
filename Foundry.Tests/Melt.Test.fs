namespace Foundry.Tests

open NUnit.Framework
open Foundry.Melt
open Foundry.Mold

[<TestFixture>]
type MeltTest () =

    let mdCSpec =
        ()

    /// The testing Markdown consists of three
    /// sections that cover:
    /// 1. A standard case with title and single Q&A pair
    /// 2. A standard case but with no title
    /// 3. A case with multiple answer fields clearly separated
    let mdToParse = """
# Lorem Ipsum ⚗️

- Ave Caesar

    Morituri te salutant.

- Hic

    Mortui vivunt.

⚗️

⚗️

- Death is only an illusion

    And life is a simulation.

- The only correct answer is

    42.

⚗️

# Ich bin ⚗️

- Wer bist du

    Ich bin ich.

    Ich bin der Teufel.

- Wer ist sie

    Sie ist ich.

    Sie isst mich.

⚗️
"""

    /// <summary>
    /// Should serialize Markdown to Record according to default template
    /// </summary>
    [<Test>]
    member this.TestMdToRecord () =
        let (expected: List<Batch>) = [ 
            { Id = "Lorem Ipsum"
              Records = [ { Id = ""
                            Fields = ["Ave Caesar"; "Morituri te salutant."]
                            TreeCategories = []
                            Tags = [] }
                          { Id = ""
                            Fields = ["Hic"; "Mortui vivunt."]
                            TreeCategories = []
                            Tags = [] } ] }
            { Id = ""
              Records = [ { Id = ""
                            Fields = ["Death is only an illusion"; "And life is a simulation."]
                            TreeCategories = []
                            Tags = [] }
                          { Id = ""
                            Fields = ["The only correct answer is"; "42."]
                            TreeCategories = []
                            Tags = [] } ] }
            { Id = "Ich bin"
              Records = [ { Id = ""
                            Fields = ["Wer bist du"; "Ich bin ich."; "Ich bin der Teufel."]
                            TreeCategories = []
                            Tags = [] }
                          { Id = ""
                            Fields = ["Wer ist sie"; "Sie ist ich."; "Sie isst mich."]
                            TreeCategories = []
                            Tags = [] } ] } ]
        let (actual: List<Batch>) = melt defaultMarkdownMold mdToParse
        Assert.That(actual, Is.EqualTo(expected))

    /// <summary>
    /// Should serialize Markdown to Record according to **custom** template
    /// </summary>
    [<Test>]
    member this.TestMdToRecordCSpec () =
        let expected = ()
        let actual = ()
        Assert.That(actual, Is.EqualTo(expected))

    /// <summary>
    /// Should serialize iBooks highlights to Record
    /// </summary>
    [<Test>]
    member this.TestIBooksToRecord () =
        let expected = ()
        let actual = ()
        Assert.That(actual, Is.EqualTo(expected))