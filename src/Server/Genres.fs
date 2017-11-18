module Genres

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful

open MusicStore.DTO
open Suave.ServerErrors

let get () =
  let ctx = Db.ctx ()
  ctx.Public.Genres
  |> Seq.map (fun g -> { Genre.Id = g.Genreid; Name = g.Name })
  |> Seq.toArray
  |> Api2.Response.Ok

let meth = function
| Api2.Get -> GET

let resolve resultF ctx =
  async {
    let args =
      ctx.request.rawForm
      |> System.Text.Encoding.UTF8.GetString
      |> ServerCode.FableJson.ofJson

    match resultF args with
    | Api2.Response.Ok x ->
      return! OK (ServerCode.FableJson.toJson x) ctx
    | Api2.Exception e ->
      return! INTERNAL_ERROR e ctx
  }

let mkWebpart 
    (endpoint : Api2.Endpoint<'req, 'res>) 
    (resultF  : 'req -> Api2.Response<'res>) =
  path endpoint.Uri 
  >=> meth endpoint.Method 
  >=> resolve resultF

let webpart = mkWebpart Api2.Genres.get get