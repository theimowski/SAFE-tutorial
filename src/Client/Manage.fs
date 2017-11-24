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
| DeleteAlbum of Album
| EditAlbumMsg of Album
| AlbumDeleted of Result<int, exn>

let update msg model =
  match msg with
  | DeleteAlbum album ->
    match model.User with
    | LoggedAsAdmin token -> 
      model, promise (delete token) album AlbumDeleted
    | _ ->
      model, Cmd.none
  | EditAlbumMsg album ->
    { model with EditAlbum = EditAlbum.init album } , Cmd.none
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

let deleteAblum (album : Album) dispatch =
  let msg = sprintf "Confirm delete album '%s'?" album.Title
  if Fable.Import.Browser.window.confirm msg then
    dispatch (DeleteAlbum album)

let view model dispatch = [
  h2 [] [ str "Index" ]
  p [] [ aHref "Create new" NewAlbum ]
  table [] [
    thead [] [
      tr [] [
        thStr "Artist"
        thStr "Title"
        thStr "Genre"
        thStr "Price"
        thStr "Action"
      ]
    ]
    
    //tbody [] [
    //  for album in 
    //    model.Albums 
    //    |> List.sortBy (fun a -> a.Artist.Name + a.Title) do
    //  yield tr [] [
    //    tdStr (truncate 25 album.Artist.Name)
    //    tdStr (truncate 25 album.Title)
    //    tdStr album.Genre.Name
    //    tdStr (string album.Price)
    //    td [ ] [ 
    //      a [ Href (hash (EdAlbum album.Id))
    //          OnClick (fun _ -> editAlbum album dispatch) ] [ 
    //        str "Edit"
    //      ]
    //      str " | "
    //      aHref "Details" (Album album.Id)
    //      str " | "          
    //      a [ Href (hash Manage)
    //          OnClick (fun _ -> deleteAblum album dispatch) ] [ 
    //        str "Delete"
    //      ]
    //    ]
    //  ]
    //]
  ]
]
