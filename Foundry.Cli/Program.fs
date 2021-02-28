module Foundry.Cli
open System
open System.IO
open Legivel.Serialization
open Argu

let Defaults = {|
    ConfigPath = "config.yaml"
|}

type Config = {
    Input: string
    Output: string
}

type Args =
    | [<AltCommandLine("-c")>] Config of string
    | [<AltCommandLine("-i")>] Input of string
    | [<AltCommandLine("-o")>] Output of string

    interface IArgParserTemplate with
        member s.Usage = 
            match s with
            | Config _ -> "Path to the configuration file"
            | Input _ -> "Path to the input file/directory"
            | Output _ -> "Path to the output file/directory"

[<EntryPoint>]
let main argv =
    let argParser = ArgumentParser.Create<Args>(programName = "foundry.exe")
    let args = argParser.Parse argv

    let configObject =

        let handleCfg path =
            match Deserialize<Config>(File.ReadAllText(path)).Head with
            | Success si -> si.Data
            | Error _ -> raise ( Exception ("The supplied config isn't properly formatted!") )
        
        match args.TryGetResult Config with
        | Some s -> handleCfg(s)
        | None -> handleCfg(Defaults.ConfigPath);
    
    let inputFileContents =
        match args.TryGetResult Input with
        | Some s -> File.ReadAllText(s)
        | None -> raise ( Exception ("Provide an input file to transform!") )

    let parsed = Parse.parse "\r\n" configObject.Input inputFileContents

    let made = Make.make "\r\n" parsed configObject.Output

    printfn "%A" made
    0