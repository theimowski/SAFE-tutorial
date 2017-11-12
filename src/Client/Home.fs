module MusicStore.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let view model = [ 
  img [ Src "/home-showcase.png" ]
  h2 [] [ str "Fresh off the grill" ]
  ul [ Id "album-list" ] (
      model.Albums 
      |> Seq.sortByDescending (fun a -> a.Id)
      |> Seq.take 5
      |> Seq.toList
      |> List.map (fun album -> 
        li [] [
          a [ Href (hash (Album album.Id))] [
            img [ Src album.ArtUrl ]
            str album.Title
          ]
        ]
      ))
]