module MusicStore.App

open Elmish
open Elmish.Browser.Navigation
open Elmish.React

open MusicStore.Api
open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

type Msg =
| AlbumsFetched of Result<Album[], exn>
| ManageMsg of Manage.Msg
| NewAlbumMsg of NewAlbum.Msg
| EditAlbumMsg of EditAlbum.Msg
| LogonMsg of Logon.Msg

let init route =
  let route = defaultArg route Home
  let model =
    { Route     = route
      Artists   = []
      Genres    = []
      Albums    = []
      User      = None
      NewAlbum  = NewAlbum.init ()
      EditAlbum = EditAlbum.initEmpty ()
      LogonForm = Logon.init () }
  
  model, promise albums () AlbumsFetched

let urlUpdate (result:Option<Route>) model =
  match result with
  | Some route ->
    { model with Route = route }, Cmd.none
  | None ->
    { model with Route = Woops }, Navigation.modifyUrl (hash Woops)

let update msg (model : Model) =
  match msg with
  | AlbumsFetched (Ok albums) ->
    let albums = List.ofArray albums
    let genres =
      albums
      |> List.map (fun a -> a.Genre)
      |> List.distinct
    let artists =
      albums
      |> List.map (fun a -> a.Artist)
      |> List.distinct
    let newAlbum = 
      { model.NewAlbum with
          Genre  = genres.[0].Id
          Artist = artists.[0].Id }
    let model =
      { model with
          NewAlbum = newAlbum
          Albums   = albums
          Genres   = genres
          Artists  = artists }
    model, Cmd.none
  | AlbumsFetched (Error _) ->
    model, Cmd.none
  | ManageMsg msg ->
    let m, msg = Manage.update msg model
    m, Cmd.map ManageMsg msg
  | NewAlbumMsg msg ->
    let m, msg = NewAlbum.update msg model
    m, Cmd.map NewAlbumMsg msg
  | EditAlbumMsg msg ->
    let m, msg = EditAlbum.update msg model
    m, Cmd.map EditAlbumMsg msg
  | LogonMsg msg ->
    let m, msg = Logon.update msg model
    m, Cmd.map LogonMsg msg

open Fable.Helpers.React
open Fable.Helpers.React.Props

let viewLoading = [ str "Loading..." ]

let viewNotFound = [
  str "Woops... requested resource was not found."
]

let viewMain model dispatch =
  if List.isEmpty model.Albums then 
    viewLoading
  else
    match model.Route with 
    | Home        -> Home.view
    | Genres      -> Genres.view model
    | Manage      -> Manage.view model (ManageMsg >> dispatch)
    | NewAlbum    -> NewAlbum.view model (NewAlbumMsg >> dispatch)
    | Logon       -> Logon.view model (LogonMsg >> dispatch)
    | Woops       -> viewNotFound
    | Genre genre ->
      match model.Genres |> List.tryFind (fun g -> g.Name = genre) with
      | Some genre -> Genre.view genre model
      | None       -> viewNotFound
    | Album id    ->
      match model.Albums |> List.tryFind (fun a -> a.Id = id) with
      | Some album -> Album.view album model
      | None       -> viewNotFound
    | EdAlbum id ->
      match model.Albums |> List.tryFind (fun a -> a.Id = id) with
      | Some album -> EditAlbum.view album model (EditAlbumMsg >> dispatch)
      | None       -> viewNotFound

let blank desc url =
  a [ Href url; Target "_blank" ] [ str desc ]

let navView = 
  list 
    [ Id "navlist" ]
    [ "Home", Home
      "Store", Genres
      "Admin", Manage ]

let userView model =
  div [Id "part-user"] [
    match model.User with
    | Some user ->
      yield str (sprintf "Logged on as %s, " user.Name)
      yield a [Href (hash Home)] [str "Log off"]
    | None ->
      yield aHref "Log on" Logon
  ]

let view model dispatch =
  div [] [
    div [ Id "header" ] [ 
      h1 [] [
        aHref "SAFE Music Store" Home
      ]
      navView
      userView model
    ]

    div [ Id "main" ] (viewMain model dispatch)

    div [ Id "footer"] [
      str "built with "
      blank "F#" "http://fsharp.org"
      str " and "
      blank "SAFE Stack" "http://SAFE-Stack.github.io"
    ]
  ]

Program.mkProgram init update view
|> Program.toNavigable parser urlUpdate
|> Program.withReact "elmish-app"
|> Program.run