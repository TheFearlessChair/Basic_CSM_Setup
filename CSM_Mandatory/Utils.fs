namespace Utils

open System
open System.IO
open System.Text
open FSharp.Text.Lexing
open FSharp.Text.Parsing

open GCLSettings
open TypesAST

module ParserUtil = 
  /// Parse a program from a string.
  let parseString input =
    let lexbuf = LexBuffer<char>.FromString input
    lexbuf.EndPos <- {pos_bol = 0; pos_fname = input; pos_cnum = 0; pos_lnum = 1}
    try
      let ast = Parser.start Lexer.tokenize lexbuf
      if DEBUG then printfn "\nTokens generated:\n----------------------------\n%s\n" ((string Lexer.sbLog).TrimEnd()) else ()
      ast
    with e -> let pos = lexbuf.EndPos
              printfn "Error near line %d, at column %d\nThe matched string was \"%s\"" pos.Line pos.Column (new string (lexbuf.Lexeme))
              if DEBUG then printfn "\nTokens at time of error:\n----------------------------\n%s\n" ((string Lexer.sbLog).TrimEnd()) else ()
              failwith "parser termination"
  
  /// Parse a program from a file.
  let parseFromFile filename =
    if File.Exists(filename)
    then parseString(File.ReadAllText(filename))
    else invalidArg "filename" (sprintf "File not found at %s" filename)


module CompilerUtil =
  open ParserUtil
  open FSharp.Reflection

  let mutable (detEntryFunc    : Option<Command -> int>) = None
  let mutable (nonDetEntryFunc : Option<Command -> int>) = None

  type ExecSettings =
    | StringSource of string
    | FileSource of string
    | Deterministic
    | Nope // Provides a similar role as Option.None.

  let getNumberOfExecSettingsCases = FSharpType.GetUnionCases(typeof<ExecSettings>).Length

  let exec opts =
    // You can only specify one Source option and you can't specify the Nope option, so the max
    // amount of options that can be specified is 2 less than there are union cases in ExecSettings.
    let maxOpts = getNumberOfExecSettingsCases - 2
    let optsCount = List.length opts
    if optsCount > maxOpts then invalidArg "exec: opts" (sprintf "Too many options were given, exec only accepts up to \'%d\' options, but were given \'%d\'." maxOpts optsCount)
    
    let sortSettings l =
      let rec doMatch out =
        function
        | [] -> out
        | Nope::ol -> doMatch out ol
        | o::ol ->
          match o with
          | StringSource s ->
            let (src, det) = out
            if src <> Nope then failwith "Multiple source options were given to exec."
            else doMatch (StringSource s, det) ol
          | FileSource s ->
            let (src, det) = out
            if src <> Nope then failwith"Multiple source options were given to exec."
            else doMatch (FileSource s, det) ol
          | _ -> failwith "This should not be possible: exec received an option that wasn't an option!"
        
      doMatch (Nope, Nope) l

    let (src, det) = sortSettings opts
    
    let prog = match src with
               | Nope -> invalidArg "exec: opts" "No source code was given!"
               | StringSource s -> parseString s
               | FileSource s   -> parseFromFile s
               | _ -> failwith "Something went wrong!"
    
    match det with 
    | Deterministic -> match detEntryFunc with
                       | Some(f) -> f prog
                       | None    -> failwith "No entry function was given for deterministic compilations."

    | Nope ->          match nonDetEntryFunc with
                       | Some(f) -> f prog
                       | None    -> failwith "No entry function was given for non-deterministic compilations."

    | _ -> failwith "Something went wrong!"