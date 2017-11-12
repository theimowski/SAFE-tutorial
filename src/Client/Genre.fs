module MusicStore.Genre

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let view (genre : Genre) model = [ 
  div [ ClassName "genre"] [
    h2 [] [ str ("Genre: " + genre.Name) ]
  
    ul [ Id "album-list" ] (
      model.Albums 
      |> Seq.filter (fun a -> a.Genre = genre) 
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
]