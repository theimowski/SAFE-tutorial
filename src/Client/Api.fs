module MusicStore.Api

open Fable.Core
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch

open Elmish

open MusicStore.DTO
open MusicStore.Model

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

let addToCart (cartId, albumId : int) =
  fetchAs<CartItem[]> (sprintf "/api/cart/%s" cartId) [
    Method HttpMethod.POST
    albumId |> toJson |> U3.Case3 |> Body
  ]

let promise req args f = 
  Cmd.ofPromise req args (Ok >> f) (Error >> f)

let (|LoggedAsAdmin|_|) = function
| LoggedIn { Role = Admin; Token = t } -> Some t
| _ -> None