#r @"packages/build/FAKE/tools/FakeLib.dll"

open System

open Fake

let runDotnet workingDir args =
    let result =
        ExecProcess (fun info ->
            info.FileName <- "dotnet"
            info.WorkingDirectory <- workingDir
            info.Arguments <- args) TimeSpan.MaxValue
    if result <> 0 then failwithf "dotnet %s failed" args

Target "Build" (fun () -> 
  runDotnet "." "build"
)

RunTargetOrDefault "Build"