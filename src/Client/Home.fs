module App.Home

open Elmish

open Fable.Helpers.React.Props
module R = Fable.Helpers.React

type Model = Unit

type Msg = Unit

let init _ = (), Cmd.none

let update _ (model : Model) =
  model

let view _ _ = [
  R.str "Home"
  R.br []
  R.a [ Href "#genres" ] [ R.str "Genres" ]
]