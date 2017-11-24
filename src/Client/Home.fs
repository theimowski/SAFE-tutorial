module MusicStore.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore
open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View
open MusicStore.Api.Remoting

let view model dispatch = [ 
  yield img [ Src "/home-showcase.png" ]
  yield h2 [] [ str "Fresh off the grill" ]
  match model.Bestsellers with
  | Ready bestsellers ->
    yield ul [ Id "album-list" ] (
      bestsellers
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
  | Loading ->
    yield gear "album-list"
  | _ ->
    yield str "Failed to fetch bestsellers"
]