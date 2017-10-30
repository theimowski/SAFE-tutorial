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

type Model = 
  { Route  : Route
    Genres : string list }

type Msg =
| GenresFetched    of string []
| GenresFetchError of exn

let getGenres () = promise {
  return! Fetch.fetchAs<string[]> "/api/genres" []
}

let init _ = 
  let model =
    { Route  = Home
      Genres = [ "Rock" ] }
  let cmd = Cmd.ofPromise getGenres () GenresFetched GenresFetchError
  model, cmd

let update msg (model : Model) =
  match msg with
  | GenresFetched gs   ->
    { model with Genres = List.ofArray gs }, Cmd.none
  | GenresFetchError _ ->
    model, Cmd.none

let viewMain model dispatch =
  match model.Route with 
  | Home    -> 
    [ R.str "Home"
      R.br []
      R.a [ Href "#genres" ] [ R.str "Genres" ] ]
  | Genre g -> [ R.str ("Genre: " + g) ]
  | Genres  -> 
    [ R.h2 [] [ R.str "Browse Genres" ]
      R.p [] [
        R.str (sprintf "Select from %d genres:" model.Genres.Length)
      ]
      R.ul [] [
        for genre in model.Genres ->
          let url = sprintf "#genre/%s" genre
          R.li [] [ R.a [ Href url ] [ R.str genre ] ]
      ]
    ]

let view model dispatch =
  R.div [] [
    R.div [ Id "header" ] [ 
      R.h1 [] [ 
        R.a [ Href "#" ] [ R.str "SAFE Music Store" ]
      ]
    ]

    R.div [ Id "main" ] (viewMain model dispatch)

    R.div [ Id "footer"] [
      R.str "built with "
      R.a [ Href "http://fsharp.org" ] [ R.str "F#" ]
      R.str " and "
      R.a [ Href "http://SAFE-Stack.github.io" ] [ R.str "SAFE Stack" ]
    ]
  ]

let route : Parser<Route -> Route, _> =
  oneOf [
    map Home (top)
    map Genres (s "genres")
    map Genre  (s "genre" </> str)
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