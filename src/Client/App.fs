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
      Genres = [ ] }
  let cmd = Cmd.ofPromise getGenres () GenresFetched GenresFetchError
  model, cmd

let update msg (model : Model) =
  match msg with
  | GenresFetched gs   ->
    { model with Genres = List.ofArray gs }, Cmd.none
  | GenresFetchError _ ->
    model, Cmd.none

let hash = function
| Home    -> "#"
| Genre g -> sprintf "#genre/%s" g
| Genres  -> "#genres"

let route : Parser<Route -> Route, _> =
  oneOf [
    map Home (top)
    map Genres (s "genres")
    map Genre  (s "genre" </> str)
  ]

let href = hash >> Href

let viewMain model dispatch =
  match model.Route with 
  | Home    -> 
    [ R.str "Home"
      R.br []
      R.a [ href Genres ] [ R.str "Genres" ] ]
  | Genre g -> [ R.str ("Genre: " + g) ]
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