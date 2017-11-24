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
| EditAlbum    of Form.EditAlbum
| AlbumUpdated of Result<Album, exn>

let init (album : Album) : Form.EditAlbum =
  { Id     = album.Id 
    Genre  = album.Genre.Id
    Artist = album.Artist.Id
    Title  = album.Title
    Price  = album.Price }

let initEmpty () : Form.EditAlbum =
  { Id     = 0
    Genre  = 0
    Artist = 0
    Title  = ""
    Price  = 0.0M }


let update msg model =
  let set editAlbum = { model with EditAlbum = editAlbum }
  match msg with 
  | Genre  id   -> set { model.EditAlbum with Genre = id }, Cmd.none
  | Artist id   -> set { model.EditAlbum with Artist = id }, Cmd.none
  | Title t     -> set { model.EditAlbum with Title = t }, Cmd.none
  | Price p     -> set { model.EditAlbum with Price = p }, Cmd.none
  | EditAlbum a -> 
    let cmd =
      match model.User with
      | LoggedAsAdmin token ->
        Cmd.batch [ promise (edit token) a AlbumUpdated 
                    redirect Manage]
      | _ -> Cmd.none
    model, cmd
  //| AlbumUpdated (Ok album) ->
  //  let albums' = model.Albums |> List.filter (fun a -> a.Id <> album.Id) 
  //  { model with Albums = album :: albums' }, Cmd.none
  | AlbumUpdated _ -> model, Cmd.none


let view album model dispatch = 
  let genres = []
    
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
                (string model.EditAlbum.Genre))
      formLbl "Artist"
      formFld 
        (select [Name "Artist"
                 onInput (int >> Artist >> dispatch)]
                artists
                (string model.EditAlbum.Artist))
      formLbl "Title"
      formFld 
        (input [Name "Title"
                Value model.EditAlbum.Title
                Type "text"
                onInput (Title >> dispatch)])
      formLbl "Price"
      formFld 
        (input [Name "Price"
                Value (string model.EditAlbum.Price)
                Type "number"
                onInput (decimal >> Price >> dispatch)])
    ]
  ]
  button [ ClassName "button"; onClick dispatch (EditAlbum model.EditAlbum) ] [ str "Save" ]
  br []
  br []
  div [] [ aHref "Back to list" Manage ]
]
