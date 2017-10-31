module App

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.React

open Fable.Helpers.React.Props
open Fable.PowerPack
module R = Fable.Helpers.React

type Route =
| Home
| Genres
| Genre of string
| Album of int

type Album =
  { Name : string }

type WebData<'a> =
| Loading
| Success of 'a
| Failure of exn

type Model = 
  { Route  : Route
    Genres : WebData<string list>
    Albums : Map<int, Album> }

type Msg =
| GenresFetched of WebData<string list>
| AlbumFetched  of WebData<int * Album>
| RouteChanged  of Route

let getGenres () = promise {
  let! genres = Fetch.fetchAs<string[]> "/api/genres" []
  return List.ofArray genres
}

let init _ = 
  let model =
    { Route  = Home
      Genres = Loading
      Albums = Map.ofList [ 1, { Name = "Yo!" } ] }
  let cmd = 
    Cmd.ofPromise getGenres () (Success >> GenresFetched) 
                               (Failure >> GenresFetched)
  model, cmd

let hash = function
| Home    -> sprintf "#"
| Genre g -> sprintf "#genre/%s" g
| Genres  -> sprintf "#genres"
| Album a -> sprintf "#album/%d" a

let route : Parser<Route -> Route, _> =
  oneOf [
    map Home (top)
    map Genres (s "genres")
    map Genre  (s "genre" </> str)
    map Album  (s "album" </> i32)
  ]

let href = hash >> Href

let update msg (model : Model) =
  match msg with
  | GenresFetched genres ->
    { model with Genres = genres }, Cmd.none
  | AlbumFetched (Success (id, album)) ->
    { model with Albums = Map.add id album model.Albums }, Cmd.none
  | RouteChanged route ->
    { model with Route = route }, Navigation.newUrl (hash route)
  | _ ->
    model, Cmd.none

let onClick dispatch msg = OnClick (fun _ -> dispatch msg)

let viewHome = [ 
  R.str "Home"
  R.br []
  R.a [ href Genres ] [ R.str "Genres" ] 
]

let viewGenre genre model dispatch = [
  R.str ("Genre: " + genre)
  R.ul [] [
    let albums = Map.toList model.Albums
    for (id,album) in albums ->
      R.li [] [ 
        R.button [ onClick dispatch (RouteChanged (Album id)) ] [ 
          R.str album.Name
        ] 
      ]
  ]
]

let viewGenres model =
  match model.Genres with
  | Loading ->
    [ R.str "Loading genres..." ]
  | Success genres -> 
    [ R.h2 [] [ R.str "Browse Genres" ]
      R.p [] [
        R.str (sprintf "Select from %d genres:" genres.Length)
      ]
    
      R.ul [] [
        for genre in genres ->
          R.li [] [ R.a [ href (Genre genre) ] [ R.str genre ] ]
      ]
    ]
  | Failure _ ->
    [ R.str "Failed to load genres" ]

let viewAlbum id model =
  match Map.tryFind id model.Albums with
  | Some album -> 
    [ R.str (album.Name) ]
  | None ->
    [ R.str "Loading..." ]

let viewMain model dispatch =
  match model.Route with 
  | Home        -> viewHome
  | Genre genre -> viewGenre genre model dispatch
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

let urlUpdate (result:Option<Route>) model =
  match result with
  | Some route -> 
    { model with Route = route }, Cmd.none
  | None ->
    model, Navigation.modifyUrl "#"

Program.mkProgram init update view
|> Program.toNavigable (parseHash route) urlUpdate
|> Program.withReact "elmish-app"
|> Program.run