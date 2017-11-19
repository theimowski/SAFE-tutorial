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

let webpart = Common.mkWebpart Api2.Genres.get get