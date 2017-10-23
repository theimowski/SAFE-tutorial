module App

open Elmish
open Elmish.React

open Fable.Helpers.React.Props
module R = Fable.Helpers.React

type Model = string list

type Msg = Idle

let init () = 
  ["Rock"; "Disco"; "Pop"]

let update msg (model : Model) =
  model

let view model dispatch =
  R.div [] [
    R.div [ Id "header" ] [ 
      R.h1 [] [ 
        R.a [ Href "/index.html" ] [ R.str "SAFE Music Store" ]
      ] 
    ]

    R.div [ Id "main" ] [
      R.h2 [] [ R.str "Browse Genres" ]
      R.p [] [ 
        R.str (sprintf "Select from %d genres:" (List.length model))
      ]
      R.ul [] [
        for genre in model ->
          let url = "/index.html"
          R.li [] [ R.a [ Href url ] [ R.str genre ] ]
      ]
    ]

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