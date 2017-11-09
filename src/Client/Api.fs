module MusicStore.Api

open Fable.Core
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch

open Elmish

open MusicStore.DTO

let albums () =
  fetchAs<Album[]> "/api/albums" []

let delete album =
  fetchAs<int> (sprintf "/api/album/%d" album.Id) [Method HttpMethod.DELETE]

let create (album : Form.NewAlbum) =
  fetchAs<Album> "/api/albums" [
    Method HttpMethod.POST
    album |> toJson |> U3.Case3 |> Body]

let edit (album : Form.EditAlbum) =
  fetchAs<Album> (sprintf "/api/album/%d" album.Id) [
    Method HttpMethod.PATCH
    album |> toJson |> U3.Case3 |> Body]

let logon (form : Form.Logon) =
  fetchAs<Credentials> (sprintf "/api/account/logon") [
    Method HttpMethod.POST
    form |> toJson |> U3.Case3 |> Body
  ]

let promise req args f = 
  Cmd.ofPromise req args (Ok >> f) (Error >> f)