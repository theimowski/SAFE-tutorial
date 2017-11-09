module MusicStore.NewAlbum

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fable.PowerPack.Fetch

open Elmish

open MusicStore.Api
open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let init () : Form.NewAlbum =
  { Genre  = 0
    Artist = 0
    Title  = ""
    Price  = 0.0M }

type Msg =
| Genre        of int
| Artist       of int
| Title        of string
| Price        of decimal
| NewAlbum     of Form.NewAlbum
| AlbumCreated of Result<Album, exn>

let update msg model =
  let set newAlbum = { model with NewAlbum = newAlbum }
  match msg with 
  | Genre  id  -> set { model.NewAlbum with Genre = id }, Cmd.none
  | Artist id  -> set { model.NewAlbum with Artist = id }, Cmd.none
  | Title t    -> set { model.NewAlbum with Title = t }, Cmd.none
  | Price p    -> set { model.NewAlbum with Price = p }, Cmd.none
  | NewAlbum a ->
    let cmd =
      match model.State with
      | LoggedAsAdmin token ->
        Cmd.batch [ promise (create token) a AlbumCreated 
                    redirect Manage]
      | _ -> Cmd.none
    model, cmd
  | AlbumCreated (Ok album) -> 
    { model with Albums = album :: model.Albums }, Cmd.none
  | AlbumCreated _ -> model, Cmd.none

let view model dispatch = 
  let genres = 
    model.Genres 
    |> List.map (fun g -> string g.Id, g.Name)
    |> List.sortBy snd
  let artists = 
    model.Artists 
    |> List.map (fun a -> string a.Id, a.Name)
    |> List.sortBy snd
  [
  h2 [] [ str "Create" ]
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
  button [ ClassName "button"; onClick dispatch (NewAlbum model.NewAlbum) ] [ str "Create" ]
  br []
  br []
  div [] [ aHref "Back to list" Manage ]
]