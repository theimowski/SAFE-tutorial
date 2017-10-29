module App.Genre

open Elmish

open Fable.Helpers.React.Props
module R = Fable.Helpers.React

type Model = string

type Msg = Unit

let init genre = genre, Cmd.none

let update _ (model : Model) =
  model

let view model _ = [
  R.str (sprintf "Genre: %s" model)
]