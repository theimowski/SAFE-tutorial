module MusicStore.NewAlbum

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let formLbl label = div [ ClassName "editor-label" ] [ str label ]
let formFld field = div [ ClassName "editor-field" ] [ field ]
let selectInput name options = 
  let options =
    options
    |> List.map (fun (v,txt) -> option [Value v] [str txt])
  select [Name name] options

let view model = 
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
      formFld (selectInput "Genre" genres)
      formLbl "Artist"
      formFld (selectInput "Artist" artists)
      formLbl "Title"
      formFld (input [Name "Title"; Type "text"])
      formLbl "Price"
      formFld (input [Name "Price"; Type "number"])
    ]
  ]
  button [ ClassName "button" ] [ str "Create" ]
  br []
  br []
  div [] [ aHref "Back to list" Manage ]
]