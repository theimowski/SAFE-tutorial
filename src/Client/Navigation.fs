module MusicStore.Navigation

open Fable.Import.Browser

open Elmish.Browser.UrlParser

type Route =
| Home
| Genres
| Genre of string
| Album of int
| Manage
| NewAlbum
| EditAlbum of int
| Woops

let hash = function
| Home        -> sprintf "#"
| Genre g     -> sprintf "#genre/%s" g
| Genres      -> sprintf "#genres"
| Album a     -> sprintf "#album/%d" a
| Manage      -> sprintf "#manage"
| NewAlbum    -> sprintf "#albums/new"
| EditAlbum a -> sprintf "#album/%d/edit" a
| Woops       -> sprintf "#notfound"

let route : Parser<Route -> Route, _> =
  oneOf [
    map Home      (top)
    map Genres    (s "genres")
    map Genre     (s "genre" </> str)
    map Album     (s "album" </> i32)
    map NewAlbum  (s "albums" </> s "new")
    map EditAlbum (s "album" </> i32 </> s "edit")
    map Manage    (s "manage")
  ]

let redirect route = Elmish.Browser.Navigation.Navigation.newUrl (hash route)

let parser : Location -> Route option = parseHash route