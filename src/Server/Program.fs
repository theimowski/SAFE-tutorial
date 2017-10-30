open System.IO

open System.Net

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let clientPath = Path.Combine("src","Client") |> Path.GetFullPath
let port = 8085us

let config =
  { defaultConfig with 
      homeFolder = Some clientPath
      bindings = [ HttpBinding.create HTTP (IPAddress.Parse "0.0.0.0") port ] }

let getGenres = 
  [ "Rock"; "Pop"; "Disco"; "Blues"]
  |> List.map (sprintf "\"%s\"")
  |> String.concat ","
  |> sprintf "[%s]"
  |> OK

let app =
  choose [
    path "/api/genres" >=> getGenres
    Files.browseHome
  ]

startWebServer config app