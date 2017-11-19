module MusicStore.Api

open Fable.Core
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch

open Elmish

open MusicStore.DTO
open MusicStore.Model
open System.Net.Http
open Fable.PowerPack.Fetch.Fetch_types

let albums () =
  fetchAs<Album[]> "/api/albums" []

let authHeader (token : string) = Authorization ("Bearer " + token) 

let delete token album =
  fetchAs<int> 
    (sprintf "/api/album/%d" album.Id) 
    [ Method HttpMethod.DELETE
      requestHeaders [ authHeader token ]]

let create token (album : Form.NewAlbum) =
  fetchAs<Album> "/api/albums" [
    Method HttpMethod.POST
    requestHeaders [ authHeader token ]
    album |> toJson |> U3.Case3 |> Body]

let edit token (album : Form.EditAlbum) =
  fetchAs<Album> (sprintf "/api/album/%d" album.Id) [
    Method HttpMethod.PATCH
    requestHeaders [ authHeader token ]
    album |> toJson |> U3.Case3 |> Body]

let logon (form : Form.Logon) =
  fetchAs<Credentials> (sprintf "/api/account/logon") [
    Method HttpMethod.POST
    form |> toJson |> U3.Case3 |> Body
  ]

let cartItems cartId =
  fetchAs<CartItem[]> (sprintf "/api/cart/%s" cartId) [ ]

let upgradeCart (oldCartId, newCartId : string) =
  fetchAs<CartItem[]> (sprintf "/api/cart/%s" oldCartId) [
    Method HttpMethod.PATCH
    newCartId |> toJson |> U3.Case3 |> Body
  ]

let addToCart (cartId, albumId : int) =
  fetchAs<CartItem[]> (sprintf "/api/cart/%s" cartId) [
    Method HttpMethod.POST
    albumId |> toJson |> U3.Case3 |> Body
  ]

let removeFromCart (cartId, albumId : int) =
  fetchAs<CartItem[]> (sprintf "/api/cart/%s" cartId) [
    Method HttpMethod.DELETE
    albumId |> toJson |> U3.Case3 |> Body
  ]

let register (form : Form.Register) =
  fetchAs<Credentials> "/api/accounts/register" [
    Method HttpMethod.POST
    form |> toJson |> U3.Case3 |> Body
  ]

let promise req args f = 
  Cmd.ofPromise req args (Ok >> f) (Error >> f)

let (|LoggedAsAdmin|_|) = function
| LoggedIn { Role = Admin; Token = t } -> Some t
| _ -> None

let meth = function
| Api2.Get -> HttpMethod.GET
| Api2.Post -> HttpMethod.POST


let [<PassGenerics>] handleResp<'res> (res : Response) =
  if res.Status = 200 then
    res.text()
    |> Promise.map (ofJson<'res> >> Api2.Response.Ok)
  else
    let msg =
      if res.Status = 500 then
        res.text()
      else
        res.text()
        |> Promise.map (sprintf "unhandled code: %d, body: %s" res.Status)
    
    Promise.map Api2.Response.Exception msg
    
let [<PassGenerics>]  promise' 
  (endpoint : Api2.Endpoint<'req, 'res>) (arg : 'req) =
  fetch endpoint.Uri [ 
    yield Method (meth endpoint.Method)
    match box arg with
    | :? unit -> ()
    | _ ->
      yield arg |> toJson |> U3.Case3 |> Body
  ]
  |> Promise.bind handleResp<'res>

let [<PassGenerics>] promise2 
  (endpoint : Api2.Endpoint<'req, 'res>) args msgF =

  Cmd.ofPromise 
    (promise' endpoint)
    args 
    msgF 
    (fun e -> Api2.Response.Exception e.Message |> msgF)