module Common

open Suave
open Suave.Successful
open Suave.Filters
open Suave.Operators
open Suave.ServerErrors

open MusicStore.DTO

let meth = function
| Api2.Get  -> GET
| Api2.Post -> POST

let resolve 
    (endpoint : Api2.Endpoint<'uri, 'req, 'res>) 
    (resultF  : 'req -> Api2.Response<'res>) ctx =
  async {
    let args =
      match endpoint.Method with
      | Api2.Get -> box () :?> 'req
      | _ ->
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
    (endpoint : Api2.Endpoint<'uri, 'req, 'res>) 
    (resultF  : 'req -> Api2.Response<'res>) =

  let pathWebpart, resultF =
    if typeof<'uri> = typeof<unit> then path (endpoint.Uri ()), resultF
    else pathScan endpoint.Uri (fun x -> resultF x)

  path endpoint.Uri 
  >=> meth endpoint.Method 
  >=> resolve endpoint resultF
