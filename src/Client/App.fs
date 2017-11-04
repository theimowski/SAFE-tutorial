module App

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.React

open Fable.PowerPack

open Shared.DTO

type Route =
| Home
| Genres
| Genre of string
| Album of int
| Woops

type Model = 
  { Route  : Route
    Genres : Genre list
    Albums : Album list }

type Msg =
| AlbumsFetched of Result<Album[], exn>

let albums () =
  Fetch.fetchAs<Album[]> "/api/albums" []

let fetch req args f = 
  Cmd.ofPromise req args (Ok >> f) (Error >> f)

let hash = function
| Home     -> sprintf "#"
| Genre g  -> sprintf "#genre/%s" g
| Genres   -> sprintf "#genres"
| Album a  -> sprintf "#album/%d" a
| Woops    -> sprintf "#notfound"

let route : Parser<Route -> Route, _> =
  oneOf [
    map Home (top)
    map Genres (s "genres")
    map Genre  (s "genre" </> str)
    map Album  (s "album" </> i32)
  ]

let init route =
  let route = defaultArg route Home
  let model =
    { Route  = route
      Genres = []
      Albums = [] }
  
  model, fetch albums () AlbumsFetched

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
    let model =
      { model with 
          Albums = albums
          Genres = genres }
    model, Cmd.none
  | _ ->
    model, Cmd.none

open Fable.Helpers.React
open Fable.Helpers.React.Props

let aHref txt route = a [ Href (hash route) ] [ str txt ]

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
|> Program.toNavigable (parseHash route) urlUpdate
|> Program.withReact "elmish-app"
|> Program.run