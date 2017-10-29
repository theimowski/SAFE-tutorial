module App.Genres

open Elmish

open Fable.Helpers.React.Props
module R = Fable.Helpers.React
open Fable.PowerPack

type Model = string list

type Msg =
| GenresFetched of string array
| GenresFetchError of exn

let getGenres () = promise {
  return! Fetch.fetchAs<string[]> "/api/genres" []
}

let init _ = [],  Cmd.ofPromise getGenres () GenresFetched GenresFetchError

let update msg (model : Model) =
  match msg with
  | GenresFetched gs -> List.ofArray gs
  | GenresFetchError exn -> [ exn.Message ]

let view model _ = [
  R.h2 [] [ R.str "Browse Genres" ]
  R.p [] [ 
    R.str (sprintf "Select from %d genres:" (List.length model))
  ]
  R.ul [] [
    for genre in model ->
      let url = sprintf "#genre/%s" genre
      R.li [] [ R.a [ Href url ] [ R.str genre ] ]
  ]
]