module MusicStore.Manage

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Elmish

open MusicStore.Api
open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

type Msg =
| DeleteAlbum of AlbumDetails
| EditAlbumMsg of AlbumDetails
| AlbumDeleted of Result<int, exn>

let update msg model =
  match msg with
  //| DeleteAlbum album ->
  //  match model.User with
  //  | LoggedAsAdmin token -> 
  //    model, promise (delete token) album AlbumDeleted
  //  | _ ->
  //    model, Cmd.none
  //| EditAlbumMsg album ->
  //  { model with EditAlbum = EditAlbum.init album } , Cmd.none
  | AlbumDeleted  (Error _) ->
    model, Cmd.none
  //| AlbumDeleted (Ok id) ->
  //  let albums = model.Albums |> List.filter (fun a -> a.Id <> id) 
  //  { model with Albums = albums }, Cmd.none

let truncate k (s : string) =
  if s.Length > k then
    s.Substring(0, k - 3) + "..."
  else s

let editAlbum album dispatch =
  dispatch (EditAlbumMsg album)

let deleteAlbum (album : AlbumDetails) dispatch =
  let msg = sprintf "Confirm delete album '%s'?" album.Title
  if Fable.Import.Browser.window.confirm msg then
    dispatch (DeleteAlbum album)

let view model dispatch = [
  yield h2 [] [ str "Index" ]
  yield p [] [ aHref "Create new" NewAlbum ]
  match model.Albums with
  | Loading ->
    yield gear ""
  | NotAsked
  | Failed _ ->
    yield str "Failed to fetch albums"
  | Ready albums ->
    yield table [] [
      thead [] [
        tr [] [
          thStr "Artist"
          thStr "Title"
          thStr "Genre"
          thStr "Price"
          thStr "Action"
        ]
      ]
      
      tbody [] [
        for album in 
          albums 
          |> List.sortBy (fun a -> a.Artist + a.Title) do
        yield tr [] [
          tdStr (truncate 25 album.Artist)
          tdStr (truncate 25 album.Title)
          tdStr album.Genre
          tdStr (string album.Price)
          td [ ] [ 
            a [ Href (hash (EdAlbum album.Id))
                OnClick (fun _ -> editAlbum album dispatch) ] [ 
              str "Edit"
            ]
            str " | "
            aHref "Details" (Album album.Id)
            str " | "          
            a [ Href (hash Manage)
                OnClick (fun _ -> deleteAlbum album dispatch) ] [ 
              str "Delete"
            ]
          ]
        ]
      ]
  ]
]
