module MusicStore.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.Navigation

let aHref txt route = a [ Href (hash route) ] [ str txt ]

let thStr s = th [] [ str s ]

let tdStr s = td [] [ str s ]