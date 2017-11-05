module MusicStore.Album

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let labeled caption elem =
  p [] [
    em [] [ str caption ]
    elem
  ]

let view a model = [
  h2 [] [ str (sprintf "%s - %s" a.Artist.Name a.Title) ]
  p [] [ img [ Src a.ArtUrl ] ]
  div [ Id "album-details" ] [
    labeled "Artist: " (str a.Artist.Name)
    labeled "Title: " (str a.Title)
    labeled "Genre: " (aHref a.Genre.Name (Genre a.Genre.Name))
    labeled "Price: " ((str (a.Price.ToString())))
  ]
]