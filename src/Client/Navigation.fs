module MusicStore.Navigation

open Fable.Import.Browser

open Elmish.Browser.UrlParser

type Route =
| Home
| Genre of string
| Album of int
| Manage
| NewAlbum
| EdAlbum of int
| Logon
| Register
| Cart
| Woops

let hash = function
| Home      -> sprintf "#"
| Genre g   -> sprintf "#genre/%s" g
| Album a   -> sprintf "#album/%d" a
| Manage    -> sprintf "#manage"
| NewAlbum  -> sprintf "#albums/new"
| EdAlbum a -> sprintf "#album/%d/edit" a
| Logon     -> sprintf "#account/logon"
| Register  -> sprintf "#account/register"
| Cart      -> sprintf "#cart"
| Woops     -> sprintf "#notfound"

let route : Parser<Route -> Route, _> =
  oneOf [
    map Home      (top)
    map Genre     (s "genre" </> str)
    map Album     (s "album" </> i32)
    map NewAlbum  (s "albums" </> s "new")
    map EdAlbum   (s "album" </> i32 </> s "edit")
    map Logon     (s "account" </> s "logon")
    map Register  (s "account" </> s "register")
    map Cart      (s "cart")
    map Manage    (s "manage")
  ]

let redirect route = Elmish.Browser.Navigation.Navigation.newUrl (hash route)

let parser : Location -> Route option = parseHash route