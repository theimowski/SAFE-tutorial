module MusicStore.App

open Elmish
open Elmish.Browser.Navigation
open Elmish.React

open Fable.PowerPack
open Fable.PowerPack.Fetch

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

type Msg =
| AlbumsFetched of Result<Album[], exn>
| AlbumDeleted of Result<int, exn>
| ManageMsg of Manage.Msg

let albums () =
  fetchAs<Album[]> "/api/albums" []

let delete album =
  fetchAs<int> (sprintf "/api/album/%d" album.Id) [Method HttpMethod.DELETE]

let promise req args f = 
  Cmd.ofPromise req args (Ok >> f) (Error >> f)

let init route =
  let route = defaultArg route Home
  let model =
    { Route   = route
      Artists = []
      Genres  = []
      Albums  = [] }
  
  model, promise albums () AlbumsFetched

let urlUpdate (result:Option<Route>) model =
  match result with
  | Some route ->
    { model with Route = route }, Cmd.none
  | None ->
    { model with Route = Woops }, Navigation.modifyUrl (hash Woops)

let update msg (model : Model) =
  match msg with
  | AlbumsFetched (Ok albums) ->
    let albums = List.ofArray albums
    let genres =
      albums
      |> List.map (fun a -> a.Genre)
      |> List.distinct
    let artists =
      albums
      |> List.map (fun a -> a.Artist)
      |> List.distinct
    let model =
      { model with 
          Albums  = albums
          Genres  = genres
          Artists = artists }
    model, Cmd.none
  | AlbumsFetched (Error _)
  | AlbumDeleted  (Error _) ->
    model, Cmd.none
  | AlbumDeleted (Ok id) ->
    let albums = List.filter (fun a -> a.Id <> id) model.Albums
    { model with Albums = albums }, Cmd.none
  | ManageMsg (Manage.DeleteAlbum album) ->
    model, promise delete album AlbumDeleted

open Fable.Helpers.React
open Fable.Helpers.React.Props


let list xs = 
  ul [] [ for (txt, route) in xs -> li [] [ aHref txt route ] ]

let viewHome = [ 
  str "Home"
  br []
  aHref "Genres" Genres
]

let viewLoading = [ str "Loading..." ]

let viewGenre (genre : Genre) model = [ 
  str ("Genre: " + genre.Name)
    
  model.Albums 
  |> Seq.filter (fun a -> a.Genre = genre) 
  |> Seq.toList
  |> Seq.map (fun a -> a.Title, (Album a.Id))
  |> list
]

let viewGenres model = [ 
  h2 [] [ str "Browse Genres" ]
  p [] [
    str (sprintf "Select from %d genres:" model.Genres.Length)
  ]

  model.Genres
  |> Seq.map (fun g -> g.Name, (Genre g.Name))
  |> list
]

let labeled caption elem =
  p [] [
    em [] [ str caption ]
    elem
  ]

let viewAlbum a model = [
  h2 [] [ str (sprintf "%s - %s" a.Artist.Name a.Title) ]
  p [] [ img [ Src a.ArtUrl ] ]
  div [ Id "album-details" ] [
    labeled "Artist: " (str a.Artist.Name)
    labeled "Title: " (str a.Title)
    labeled "Genre: " (aHref a.Genre.Name (Genre a.Genre.Name))
    labeled "Price: " ((str (a.Price.ToString())))
  ]
]



let onClick dispatch msg = OnClick (fun _ ->  dispatch msg)

let formLbl label = div [ ClassName "editor-label" ] [ str label ]
let formFld field = div [ ClassName "editor-field" ] [ field ]
let selectInput name options = 
  let options =
    options
    |> List.map (fun (v,txt) -> option [Value v] [str txt])
  select [Name name] options

let viewNewAlbum model = 
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

let viewNotFound = [
  str "Woops... requested resource was not found."
]

let viewMain model dispatch =
  if List.isEmpty model.Albums then 
    viewLoading
  else
    match model.Route with 
    | Home        -> viewHome
    | Genres      -> viewGenres model
    | Manage      -> Manage.view model (ManageMsg >> dispatch)
    | NewAlbum    -> viewNewAlbum model
    | Woops       -> viewNotFound
    | Genre genre ->
      match model.Genres |> List.tryFind (fun g -> g.Name = genre) with
      | Some genre -> viewGenre genre model
      | None       -> viewNotFound
    | Album id    ->
      match model.Albums |> List.tryFind (fun a -> a.Id = id) with
      | Some album -> viewAlbum album model
      | None       -> viewNotFound

let blank desc url =
  a [ Href url; Target "_blank" ] [ str desc ]

let view model dispatch =
  div [] [
    div [ Id "header" ] [ 
      h1 [] [
        aHref "SAFE Music Store" Home
      ]
    ]

    div [ Id "main" ] (viewMain model dispatch)

    div [ Id "footer"] [
      str "built with "
      blank "F#" "http://fsharp.org"
      str " and "
      blank "SAFE Stack" "http://SAFE-Stack.github.io"
    ]
  ]

Program.mkProgram init update view
|> Program.toNavigable parser urlUpdate
|> Program.withReact "elmish-app"
|> Program.run