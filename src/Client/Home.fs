module MusicStore.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let view = [ 
  str "Home"
  br []
  aHref "Genres" Genres
]