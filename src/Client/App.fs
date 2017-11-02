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
    Albums : Set<Album> }

type FetchError<'a> =
| NotFound of 'a
| FetchExn of exn

type Msg =
| GenresFetched of Result<Genre[], FetchError<unit>>
| AlbumFetched  of Result<Album, FetchError<int>>
| AlbumsFetched of Result<Album[], FetchError<string>>

let genres () =
  Fetch.fetchAs<Genre[]> "/api/genres" []

let album id =
  Fetch.fetchAs<Album> (sprintf "/api/album/%d" id) []

let albumsFor genre =
  Fetch.fetchAs<Album[]> (sprintf "/api/genre/%s/albums" genre) []

let fetchErr (id : 'a) (e : exn) =
  if e.Message.ToLower().Contains("not found") then
    NotFound id
  else
    FetchExn e

let fetch req args f = 
  Cmd.ofPromise req args (Ok >> f) (fetchErr args >> Error >> f)

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

let routeCmd = function
| Home 
| Genres      -> fetch genres () GenresFetched 
| Genre genre -> fetch albumsFor genre AlbumsFetched
| Album id    -> fetch album id AlbumFetched
| Woops       -> Cmd.none

let init route =
  let route = defaultArg route Home
  let model =
    { Route  = route
      Genres = []
      Albums = Set.empty }
  
  model, routeCmd route

let urlUpdate (result:Option<Route>) model =
  match result with
  | Some route ->
    { model with Route = route }, routeCmd route
  | None ->
    { model with Route = Woops }, Navigation.modifyUrl (hash Woops)

let update msg (model : Model) =
  match msg with
  | GenresFetched (Ok genres) ->
    { model with Genres = List.ofArray genres }, Cmd.none
  | AlbumFetched (Ok album) ->
    { model with Albums = Set.add album model.Albums }, Cmd.none
  | AlbumFetched (Error (NotFound id)) when model.Route = Album id -> 
    urlUpdate None model
  | AlbumsFetched (Ok xs) ->
    { model with Albums = Set.union model.Albums (Set.ofArray xs) }, Cmd.none
  | AlbumsFetched (Error (NotFound g)) when model.Route = Genre g ->
    urlUpdate None model
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

let viewGenre genre model = 
  let albums = 
    model.Albums 
    |> Seq.filter (fun a -> a.Genre.Name = genre) 
    |> Seq.toList
    
  match albums with
  | [] -> [ str "Loading..." ]
  | albums -> 
    [ str ("Genre: " + genre)
      
      albums
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

let viewAlbum id model =
  match Seq.tryFind (fun a -> a.Id = id) model.Albums with
  | Some a ->
    [ h2 [] [ str (sprintf "%s - %s" a.Artist.Name a.Title) ]
      p [] [ img [ Src a.ArtUrl ] ]
      div [ Id "album-details" ] [
        labeled "Artist: " (str a.Artist.Name)
        labeled "Title: " (str a.Title)
        labeled "Genre: " (aHref a.Genre.Name (Genre a.Genre.Name))
        labeled "Price: " ((str (a.Price.ToString())))
      ]
    ]
  | None ->
    [ str "Loading..." ]

let viewNotFound = [
  str "Woops... requested resource was not found."
]

let viewMain model dispatch =
  match model.Route with 
  | Home        -> viewHome
  | Genre genre -> viewGenre genre model
  | Genres      -> viewGenres model
  | Album id    -> viewAlbum id model
  | Woops    -> viewNotFound

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