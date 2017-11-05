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
| Woops

let hash = function
| Home     -> sprintf "#"
| Genre g  -> sprintf "#genre/%s" g
| Genres   -> sprintf "#genres"
| Album a  -> sprintf "#album/%d" a
| Manage   -> sprintf "#manage"
| NewAlbum -> sprintf "#albums/new"
| Woops    -> sprintf "#notfound"

let route : Parser<Route -> Route, _> =
  oneOf [
    map Home     (top)
    map Genres   (s "genres")
    map Genre    (s "genre" </> str)
    map Album    (s "album" </> i32)
    map NewAlbum (s "albums" </> s "new")
    map Manage   (s "manage")
  ]

let parser : Location -> Route option = parseHash route