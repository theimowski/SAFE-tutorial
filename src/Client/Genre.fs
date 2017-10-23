module App.Genre

open Fable.Helpers.React.Props
module R = Fable.Helpers.React

type Model = string

type Msg = Unit

let init genre = genre

let update _ (model : Model) =
  model

let view model _ = [
  R.str (sprintf "Genre: %s" model)
]