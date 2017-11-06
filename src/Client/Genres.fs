module MusicStore.Genres

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let view model = [ 
  h2 [] [ str "Browse Genres" ]
  p [] [
    str (sprintf "Select from %d genres:" model.Genres.Length)
  ]

  model.Genres
  |> Seq.map (fun g -> g.Name, (Genre g.Name))
  |> list []
]