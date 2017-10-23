module App.Genres

open Fable.Helpers.React.Props
module R = Fable.Helpers.React

type Model = string list

type Msg = Unit

let init () = 
  ["Rock"; "Disco"; "Pop"]

let update _ (model : Model) =
  model

let view model _ = [
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