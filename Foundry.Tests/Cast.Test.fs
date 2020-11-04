namespace Foundry.Tests

open NUnit.Framework

[<TestFixture>]
type CastTest () =

    /// <summary>
    /// Should transform an MD document under default spec into an AndevCsv File
    /// </summary>
    [<Test>]
    member this.TestMdToAndev () =
        ()

    /// <summary>
    /// Should transform and MD document under **custom** spec into an AndevCsv File
    /// </summary>
    [<Test>]
    member this.TestMdToAndevCSpec () =
        ()

    /// <summary>
    /// Should transform an iBooks highlight into an AndevCsv File
    /// </summary>
    [<Test>]
    member this.TestIBooksToAndev () =
        ()

    /// <summary>
    /// Should transform an iBooks highlight into a MD summary under default spec
    /// </summary>
    [<Test>]
    member this.TestIBooksToMd () =
        ()

    /// <summary>
    /// Should transfrom an iBooks highlight into an MD summary under a **custom** spec
    /// </summary>
    [<Test>]
    member this.TestIBooksToMdCSpec () =
        ()