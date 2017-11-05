module MusicStore.EditAlbum

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fable.PowerPack.Fetch

open Elmish

open MusicStore.Api
open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

type Msg =
| Genre        of int
| Artist       of int
| Title        of string
| Price        of decimal

let init (album : Album) =
  { Form.EditAlbum.Id = album.Id 
    Form.EditAlbum.Genre = album.Genre.Id
    Form.EditAlbum.Artist = album.Artist.Id
    Form.EditAlbum.Title = album.Title
    Form.EditAlbum.Price = album.Price }

let update msg model =
  let set editAlbum = { model with EditAlbum = Some editAlbum }
  match msg with 
  | Genre  id  -> set { model.EditAlbum with Genre = id }, Cmd.none
  | Artist id  -> set { model.EditAlbum with Artist = id }, Cmd.none
  | Title t    -> set { model.EditAlbum with Title = t }, Cmd.none
  | Price p    -> set { model.EditAlbum with Price = p }, Cmd.none

let view album model dispatch = 
  let genres = 
    model.Genres 
    |> List.map (fun g -> string g.Id, g.Name)
    |> List.sortBy snd
  let artists = 
    model.Artists 
    |> List.map (fun a -> string a.Id, a.Name)
    |> List.sortBy snd
  [
  h2 [] [ str "Edit" ]
  form [ ] [
    fieldset [] [
      legend [] [ str "Album" ]
      formLbl "Genre"
      formFld 
        (select [Name "Genre"
                 onInput (int >> Genre >> dispatch)]  
                genres
                (string model.NewAlbum.Genre))
      formLbl "Artist"
      formFld 
        (select [Name "Artist"
                 onInput (int >> Artist >> dispatch)]
                artists
                (string model.NewAlbum.Artist))
      formLbl "Title"
      formFld 
        (input [Name "Title"
                Value model.NewAlbum.Title
                Type "text"
                onInput (Title >> dispatch)])
      formLbl "Price"
      formFld 
        (input [Name "Price"
                Value (string model.NewAlbum.Price)
                Type "number"
                onInput (decimal >> Price >> dispatch)])
    ]
  ]
  button [ ClassName "button"; onClick dispatch (EditAlbum model.NewAlbum) ] [ str "Create" ]
  br []
  br []
  div [] [ aHref "Back to list" Manage ]
]
