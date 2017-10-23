module App

open Elmish
open Elmish.React

open Fable.Helpers.React.Props
module R = Fable.Helpers.React

open App

type Model = 
| Home   of Home.Model
| Genre  of Genre.Model
| Genres of Genres.Model

type Msg = Unit

let init () = Genres (Genres.init())

let update msg (model : Model) =
  model

let subView = function
| Home   m -> Home.view m
| Genre  m -> Genre.view m
| Genres m -> Genres.view m

let view model dispatch =
  R.div [] [
    R.div [ Id "header" ] [ 
      R.h1 [] [ 
        R.a [ Href "/index.html" ] [ R.str "SAFE Music Store" ]
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

Program.mkSimple init update view
|> Program.withReact "elmish-app"
|> Program.run