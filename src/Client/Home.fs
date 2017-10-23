module App.Home

open Fable.Helpers.React.Props
module R = Fable.Helpers.React

type Model = Unit

type Msg = Unit

let init () = ()

let update _ (model : Model) =
  model

let view _ _ = [
  R.str "Home"
]