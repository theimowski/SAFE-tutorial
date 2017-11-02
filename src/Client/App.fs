module App

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.React

open Fable.PowerPack

open Shared.DTO

module Map =
  let join (p:Map<'a,'b>) (q:Map<'a,'b>) = 
      Map(Seq.concat [ (Map.toSeq p) ; (Map.toSeq q) ])

type Route =
| Home
| Genres
| Genre of string
| Album of int

type Model = 
  { Route  : Route
    Genres : Genre list
    Albums : Set<Album> }

type Msg =
| GenresFetched of Result<Genre[], exn>
| AlbumFetched  of Result<Album, exn>
| AlbumsFetched of Result<Album[], exn>

let genres () =
  Fetch.fetchAs<Genre[]> "/api/genres" []

let album id =
  Fetch.fetchAs<Album> (sprintf "/api/album/%d" id) []

let albumsFor genre =
  Fetch.fetchAs<Album[]> (sprintf "/api/genre/%s/albums" genre) []

let fetch req args f = 
  Cmd.ofPromise req args (Ok >> f) (Error >> f)

let hash = function
| Home     -> sprintf "#"
| Genre g  -> sprintf "#genre/%s" g
| Genres   -> sprintf "#genres"
| Album a  -> sprintf "#album/%d" a

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

let init route =
  let route = defaultArg route Home
  let model =
    { Route  = route
      Genres = []
      Albums = Set.empty }
  
  model, routeCmd route

let update msg (model : Model) =
  match msg with
  | GenresFetched (Ok genres) ->
    { model with Genres = List.ofArray genres }, Cmd.none
  | AlbumFetched (Ok album) ->
    { model with Albums = Set.add album model.Albums }, Cmd.none
  | AlbumsFetched (Ok albums) ->
    let albums' =
      albums
      |> Set.ofArray
      |> Set.union model.Albums
    { model with Albums = albums' }, Cmd.none
  | _ ->
    model, Navigation.modifyUrl "#"

let urlUpdate (result:Option<Route>) model =
  match result with
  | Some route ->
    { model with Route = route }, routeCmd route
  | None ->
    model, Navigation.modifyUrl "#"

open Fable.Helpers.React
open Fable.Helpers.React.Props

let aHref txt route = a [ Href (hash route) ] [ str txt ]

let list xs = ul [] [ for (txt, route) in xs -> aHref txt route ]

let viewHome = [ 
  str "Home"
  br []
  aHref "Genres" Genres
]

let viewGenre genre model = [
  str ("Genre: " + genre)
  
  model.Albums
  |> Seq.filter (fun a -> a.Genre.Name = genre)
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
  | Some album ->
    [ h2 [] [ str (sprintf "%s - %s" album.Artist.Name album.Title) ]
      p [] [ img [ Src album.ArtUrl ] ]
      div [ Id "album-details" ] [
        labeled "Artist: " (str album.Artist.Name)
        labeled "Title: " (str album.Title)
        labeled "Genre: " (aHref album.Genre.Name (Genre album.Genre.Name))
        labeled "Price: " ((str (album.Price.ToString())))
      ]
    ]
  | None ->
    [ str "Loading..." ]

let viewMain model dispatch =
  match model.Route with 
  | Home        -> viewHome
  | Genre genre -> viewGenre genre model
  | Genres      -> viewGenres model
  | Album id    -> viewAlbum id model

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