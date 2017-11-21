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
      model.Bestsellers 
      |> Seq.sortByDescending (fun a -> a.Id)
      |> fun x -> if Seq.length x > 5 then Seq.take 5 x else x
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