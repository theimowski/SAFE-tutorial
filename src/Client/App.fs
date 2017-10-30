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
  { Id   : int
    Name : string }

type Model = 
  { Route  : Route
    Genres : string list
    Albums : Album list
    Album  : Album option }

type Msg =
| GenresFetched of Result<string [], exn>
| ShowAlbum of Album

let getGenres () = promise {
  return! Fetch.fetchAs<string[]> "/api/genres" []
}

let init _ = 
  let model =
    { Route  = Home
      Genres = [ ]
      Albums = [ { Id = 1; Name = "Yo!" } ]
      Album  = None }
  let cmd = 
    Cmd.ofPromise getGenres () (Ok >> GenresFetched) (Error >> GenresFetched)
  model, cmd

let hash = function
| Home    -> "#"
| Genre g -> sprintf "#genre/%s" g
| Genres  -> "#genres"
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
  | GenresFetched (Ok gs) ->
    { model with Genres = List.ofArray gs }, Cmd.none
  | GenresFetched (Error _) ->
    model, Cmd.none
  | ShowAlbum album ->
    { model with Album = Some album }, Navigation.newUrl (hash (Album album.Id))

let onClick dispatch msg = OnClick (fun _ -> dispatch msg)

let viewMain model dispatch =
  match model.Route with 
  | Home    -> 
    [ R.str "Home"
      R.br []
      R.a [ href Genres ] [ R.str "Genres" ] ]
  | Genre genre -> 
    [ R.str ("Genre: " + genre)
      R.ul [] [
        for album in model.Albums ->
          R.li [] [ 
            R.button [ onClick dispatch (ShowAlbum album) ] [ 
              R.str album.Name 
            ] 
          ]
      ] ]
  | Genres  -> 
    [ R.h2 [] [ R.str "Browse Genres" ]
      R.p [] [
        R.str (sprintf "Select from %d genres:" model.Genres.Length)
      ]
      R.ul [] [
        for genre in model.Genres ->
          R.li [] [ R.a [ href (Genre genre) ] [ R.str genre ] ]
      ]
    ]
  | Album _ -> 
    match model.Album with
    | Some album -> 
      [ R.str (album.Name) ]
    | None ->
      [ ]

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