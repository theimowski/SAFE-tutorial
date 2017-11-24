module MusicStore.Genre

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let view genre model = 
  match model.Albums with
  | Ready albums ->
    [ div [ ClassName "genre"] [
        h2 [] [ str ("Genre: " + genre) ]
      
        ul [ Id "album-list" ] (
          albums 
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
  | Loading -> [ gear "album-list" ]
  | _ -> [ str "Failed to load genre" ]