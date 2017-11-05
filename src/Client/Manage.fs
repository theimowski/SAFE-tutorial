module MusicStore.Manage

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

type Msg =
| DeleteAlbum of Album

let truncate k (s : string) =
  if s.Length > k then
    s.Substring(0, k - 3) + "..."
  else s

let deleteAblum album dispatch =
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
    
    tbody [] [
      for album in model.Albums |> List.sortBy (fun a -> a.Artist.Name) do
      yield tr [] [
        tdStr (truncate 25 album.Artist.Name)
        tdStr (truncate 25 album.Title)
        tdStr album.Genre.Name
        tdStr (string album.Price)
        td [ ] [ 
          a [ Href (hash Manage)
              OnClick (fun _ -> deleteAblum album dispatch) ] [ 
            str "Delete"
          ]
        ]
      ]
    ]
  ]
]
