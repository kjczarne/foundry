namespace Foundry.Tests

open NUnit.Framework
open Foundry.Mold

[<TestFixture>]
type MoldTest () =

    /// <summary>
    /// Should succesfully interpolate a Markdown Mold with respective Regex patterns.
    /// </summary>
    [<Test>]
    member this.TestMoldRegexInterpolation () =
        let expected = """
# (.+) ⚗️

${q}-(.+)${q} 
${a}
    (.+)
${a}
⚗️
"""
        let actual = interpolateMarkers (regexMoldInterpolationMap "⚗️") defaultMarkdownMold
        Assert.That(actual, Is.EqualTo(expected))