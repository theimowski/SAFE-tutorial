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

type WebData<'a> =
| Loading
| Success of 'a
| Failure of exn

type Model = 
  { Route  : Route
    Genres : WebData<Genre list>
    Albums : Map<int, WebData<Album>> }

type Msg =
| GenresFetched of WebData<Genre list>
| AlbumsFetched of Map<int, WebData<Album>>

let getGenres () = promise {
  let! genres = Fetch.fetchAs<Genre[]> "/api/genres" []
  return List.ofArray genres
}

let getAlbum id = promise {
  let! album = Fetch.fetchAs<Album> (sprintf "/api/album/%d" id) []
  return Map.ofList [id, album ]
}

let getAlbumsForGenre genre = promise {
  let! albums = Fetch.fetchAs<Album[]> (sprintf "/api/genre/%s/albums" genre) []
  return 
    albums
    |> Array.map (fun a -> a.Id, a)
    |> Map.ofArray
}

let init _ = 
  let model =
    { Route  = Home
      Genres = Loading
      Albums = Map.empty }
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
  | AlbumsFetched albums ->
    { model with Albums = Map.join albums model.Albums }, Cmd.none
  | _ ->
    model, Cmd.none

let onClick dispatch msg = OnClick (fun _ -> dispatch msg)

let viewHome = [ 
  R.str "Home"
  R.br []
  R.a [ href Genres ] [ R.str "Genres" ] 
]

let viewGenre genre model = [
  R.str ("Genre: " + genre)
  R.ul [] [
    let albums = 
      Map.toList model.Albums
      |> List.choose (fun (_,a) -> 
        match a with 
        | Success a when a.Genre = genre -> Some a
        | _ -> None )
    for album in albums do
      yield R.li [] [ R.a [ href (Album album.Id) ] [ R.str album.Title ] ]
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
          R.li [] [ R.a [ href (Genre genre.Name) ] [ R.str genre.Name ] ]
      ]
    ]
  | Failure _ ->
    [ R.str "Failed to load genres" ]

let viewAlbum id model =
  match Map.tryFind id model.Albums with
  | Some (Success album) -> 
    [ R.str (album.Title) ]
  | Some (Failure _) ->
    [ R.str "Cannot download album" ]
  | Some (Loading)
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

let urlUpdate (result:Option<Route>) model =
  match result with
  | Some (Genre genre) ->
    let cmd = 
      Cmd.ofPromise 
        getAlbumsForGenre 
        genre 
        (Map.map (fun _ v -> Success v) >> AlbumsFetched) 
        (fun _ -> AlbumsFetched Map.empty)
    { model with Route = Genre genre }, cmd
  | Some (Album id) when not (Map.containsKey id model.Albums) ->
    let cmd = 
      Cmd.ofPromise 
        getAlbum id (Map.map (fun _ v -> Success v) >> AlbumsFetched) 
                    (fun exn -> AlbumsFetched (Map.ofList [ id, Failure exn ]))
    { model with Route = Album id }, cmd
  | Some route -> 
    { model with Route = route }, Cmd.none
  | None ->
    model, Navigation.modifyUrl "#"

Program.mkProgram init update view
|> Program.toNavigable (parseHash route) urlUpdate
|> Program.withReact "elmish-app"
|> Program.run