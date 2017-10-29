module App.App

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.React

open Fable.Helpers.React.Props
module R = Fable.Helpers.React

open App

type Model = 
| Home   of Home.Model
| Genre  of Genre.Model
| Genres of Genres.Model

type Msg = Unit

let init _ = let m, c = Home.init() in Home m, c

let update msg (model : Model) =
  model, Cmd.none

let subView = function
| Home   m -> Home.view m
| Genre  m -> Genre.view m
| Genres m -> Genres.view m

let view model dispatch =
  R.div [] [
    R.div [ Id "header" ] [ 
      R.h1 [] [ 
        R.a [ Href "#" ] [ R.str "SAFE Music Store" ]
      ]
    ]

    R.div [ Id "main" ] (subView model dispatch)

    R.div [ Id "footer"] [
      R.str "built with "
      R.a [ Href "http://fsharp.org" ] [ R.str "F#" ]
      R.str " and "
      R.a [ Href "http://SAFE-Stack.github.io" ] [ R.str "SAFE Stack" ]
    ]
  ]

type Route =
| Home
| Genres
| Genre of string

let route : Parser<Route -> Route, _> =
  oneOf [
    map Home (top)
    map Genres (s "genres")
    map Genre  (s "genre" </> str)
  ]

let urlUpdate (result:Option<Route>) model =
  match result with
  | Some Home ->
    let m, c = Home.init ()
    Model.Home m, c
  | Some (Genres) ->
    let m, c = Genres.init ()
    Model.Genres m, c
  | Some (Genre g) ->
    let m, c = Genre.init g
    Model.Genre m, c
  | None ->
    model, Navigation.modifyUrl "#"

Program.mkProgram init update view
|> Program.toNavigable (parseHash route) urlUpdate
|> Program.withReact "elmish-app"
|> Program.run