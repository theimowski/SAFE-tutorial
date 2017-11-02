module App

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.React

open Fable.Helpers.React.Props
open Fable.PowerPack
module R = Fable.Helpers.React

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

let href = hash >> Href

let viewHome = [ 
  R.str "Home"
  R.br []
  R.a [ href Genres ] [ R.str "Genres" ] 
]

let viewGenre genre model = [
  R.str ("Genre: " + genre)
  R.ul [] [
    let albums = Seq.filter (fun a -> a.Genre.Name = genre) model.Albums
    for album in albums do
      yield R.li [] [ R.a [ href (Album album.Id) ] [ R.str album.Title ] ]
  ]
]

let aHref route txt = R.a [ href route ] [ R.str txt ]

let viewGenres model = [ 
  R.h2 [] [ R.str "Browse Genres" ]
  R.p [] [
    R.str (sprintf "Select from %d genres:" model.Genres.Length)
  ]

  R.ul [] [
    for genre in model.Genres ->
      R.li [] [ R.a [ href (Genre genre.Name) ] [ R.str genre.Name ] ]
  ]
]

let labeled caption elem =
  R.p [] [
    R.em [] [ R.str caption ]
    elem
  ]

let viewAlbum id model =
  match Seq.tryFind (fun a -> a.Id = id) model.Albums with
  | Some album ->
    [ R.h2 [] [ R.str (sprintf "%s - %s" album.Artist.Name album.Title) ]
      R.p [] [ R.img [ Src album.ArtUrl ] ]
      R.div [ Id "album-details" ] [
        labeled "Artist: " (R.str album.Artist.Name)
        labeled "Title: " (R.str album.Title)
        labeled "Genre: " (aHref (Genre album.Genre.Name) album.Genre.Name)
        labeled "Price: " ((R.str (album.Price.ToString())))
      ]
    ]
  | None ->
    [ R.str "Loading..." ]

let viewMain model dispatch =
  match model.Route with 
  | Home        -> viewHome
  | Genre genre -> viewGenre genre model
  | Genres      -> viewGenres model
  | Album id    -> viewAlbum id model

let blank desc url =
  R.a [ Href url; Target "_blank" ] [ R.str desc ]

let view model dispatch =
  R.div [] [
    R.div [ Id "header" ] [ 
      R.h1 [] [ 
        R.a [ href Home ] [ R.str "SAFE Music Store" ]
      ]
    ]

    R.div [ Id "main" ] (viewMain model dispatch)

    R.div [ Id "footer"] [
      R.str "built with "
      blank "F#" "http://fsharp.org"
      R.str " and "
      blank "SAFE Stack" "http://SAFE-Stack.github.io"
    ]
  ]

Program.mkProgram init update view
|> Program.toNavigable (parseHash route) urlUpdate
|> Program.withReact "elmish-app"
|> Program.run