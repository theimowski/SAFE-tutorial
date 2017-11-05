module MusicStore.Genre

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let view (genre : Genre) model = [ 
  str ("Genre: " + genre.Name)
    
  model.Albums 
  |> Seq.filter (fun a -> a.Genre = genre) 
  |> Seq.toList
  |> Seq.map (fun a -> a.Title, (Album a.Id))
  |> list
]