module MusicStore.Api

open Fable.Core
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch

open Elmish

open MusicStore.DTO
open MusicStore.Model
open System.Net.Http

let authHeader (token : string) = Authorization ("Bearer " + token) 

let delete token (album : Album) =
  fetchAs<int> 
    (sprintf "/api/album/%d" album.Id) 
    [ Method HttpMethod.DELETE
      requestHeaders [ authHeader token ]]

let edit token (album : Form.EditAlbum) =
  fetchAs<Album> (sprintf "/api/album/%d" album.Id) [
    Method HttpMethod.PATCH
    requestHeaders [ authHeader token ]
    album |> toJson |> U3.Case3 |> Body]

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

module Remoting =
  open Fable.Remoting.Client
  
  let albums = 
    Proxy.createWithBuilder<ApiRemoting.Albums> (sprintf "/api/%s/%s")
  let genres = 
    Proxy.createWithBuilder<ApiRemoting.Genres> (sprintf "/api/%s/%s")

  let artists = 
    Proxy.createWithBuilder<ApiRemoting.Artists> (sprintf "/api/%s/%s")

  let bestsellers = 
    Proxy.createWithBuilder<ApiRemoting.Bestsellers> (sprintf "/api/%s/%s")
  let account = 
    Proxy.createWithBuilder<ApiRemoting.Account> (sprintf "/api/%s/%s")

  let promise req args resF =
    Cmd.ofAsync req args (Ok >> resF) (Error >> resF)
  
  let promiseWD req args resF =
    Cmd.ofAsync req args (Ready >> resF) (Failed >> resF)