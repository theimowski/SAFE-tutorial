#r @"packages/build/FAKE/tools/FakeLib.dll"

open System

open Fake

let serverPath = "./src/Server" |> FullName
let serverProj = serverPath </> "Server.fsproj"
let clientPath = "./src/Client" |> FullName

let platformTool tool winTool =
  let tool = if isUnix then tool else winTool
  tool
  |> ProcessHelper.tryFindFileOnPath
  |> function Some t -> t | _ -> failwithf "%s not found" tool

let nodeTool = platformTool "node" "node.exe"
let yarnTool = platformTool "yarn" "yarn.cmd"

let mutable dotnetCli = "dotnet"

let run cmd args workingDir =
  let result =
    ExecProcess (fun info ->
      info.FileName <- cmd
      info.WorkingDirectory <- workingDir
      info.Arguments <- args) TimeSpan.MaxValue
  if result <> 0 then failwithf "'%s %s' failed" cmd args

Target "Clean" DoNothing

Target "InstallDotNetCore" (fun _ ->
  dotnetCli <- DotNetCli.InstallDotNetSDK "2.0.0"
)

Target "InstallClient" (fun _ ->
  printfn "Node version:"
  run nodeTool "--version" __SOURCE_DIRECTORY__
  printfn "Yarn version:"
  run yarnTool "--version" __SOURCE_DIRECTORY__
  run yarnTool "install --frozen-lockfile" __SOURCE_DIRECTORY__
)

Target "Build" (fun () -> 
  run dotnetCli "build" serverPath
  run dotnetCli "restore" clientPath
  run dotnetCli "fable webpack -- -p" clientPath
)

Target "Run" (fun () -> 
  let server = async { 
    run dotnetCli ("watch run") serverPath 
  }
  let client = async { 
    run dotnetCli "fable webpack-dev-server" clientPath 
  }
  let browser = async { 
    Threading.Thread.Sleep 5000
    Diagnostics.Process.Start "http://localhost:8080" |> ignore 
  }
  
  [ server; client; browser]
  |> Async.Parallel
  |> Async.RunSynchronously
  |> ignore
)

Target "SqlProvider" (fun () ->
  let targetDir = serverPath </> "sqlprovider_dlls"
  CleanDir targetDir
  !! "packages/Npgsql/lib/netstandard2.0/Npgsql.dll"
  ++ "packages/System.Threading.Tasks.Extensions/lib/netstandard2.0/System.Threading.Tasks.Extensions.dll"
  ++ "packages/System.Data.Common/lib/netstandard1.2/System.Data.Common.dll"
  |> CopyTo targetDir
)

"Clean"
  ==> "InstallDotNetCore"
  =?> ("InstallClient", not <| hasBuildParam "skip")
  ==> "Build"
  ==> "Run"

RunTargetOrDefault "Build"