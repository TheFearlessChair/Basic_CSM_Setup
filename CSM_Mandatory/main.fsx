// References and file loads.
#r "../packages/fslexyacc.runtime/10.0.0/lib/net46/FsLexYacc.Runtime.dll"
#load "GCLSettings.fs"
#load "TypesAST.fs"
#load "Parser.fs"
#load "Lexer.fs"
#load "Utils.fs"

open System
open System.Text
open System.IO

open GCLSettings
open TypesAST
open Utils.ParserUtil
open Utils.CompilerUtil

let path = Path.Combine(__SOURCE_DIRECTORY__, @"../data/")
//DEBUG <- true // Uncomment to enable debugging features.

parseFromFile (path + @"testProg.gc");;