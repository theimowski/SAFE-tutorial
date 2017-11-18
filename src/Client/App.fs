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
| RegisterMsg of Register.Msg
| AlbumMsg of Album.Msg
| CartMsg of Cart.Msg
| LogOff
| GenresResponse of Api2.Response<Genre[]>

let init route =
  let route = defaultArg route Home
  let model =
    { Route        = route
      Artists      = []
      Genres       = []
      Albums       = []
      State        = LoggedOff
      CartItems    = []
      NewAlbum     = NewAlbum.init ()
      EditAlbum    = EditAlbum.initEmpty ()
      LogonForm    = Logon.init ()
      RegisterForm = Register.init()
      LogonMsg     = None }
  
  let cmd =
    Cmd.batch [
      promise albums () AlbumsFetched
      promise2 Api2.Genres.get () GenresResponse
    ]
  model, cmd

let urlUpdate (result:Option<Route>) model =
  match result with
  | Some Logon ->
    { model with 
        Route = Logon
        LogonForm = Logon.init ()
        LogonMsg = None }, Cmd.none
  | Some route ->
    { model with Route = route }, Cmd.none
  | None ->
    { model with Route = Woops }, Navigation.modifyUrl (hash Woops)

let update msg (model : Model) =
  match msg with
  | GenresResponse (Api2.Response.Ok genres) ->
    { model with Genres = Array.toList genres }, Cmd.none
  | GenresResponse _ ->
    model, Cmd.none
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
         // Genres   = genres
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
  | RegisterMsg msg ->
    let m, msg = Register.update msg model
    m, Cmd.map RegisterMsg msg
  | AlbumMsg msg ->
    let m, msg = Album.update msg model
    m, Cmd.map AlbumMsg msg
  | CartMsg msg ->
    let m, msg = Cart.update msg model
    m, Cmd.map CartMsg msg
  | LogOff ->
    { model with 
        State     = LoggedOff
        CartItems = [] }, Cmd.none

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import.Browser

let viewLoading = [ str "Loading..." ]

let viewNotFound = [
  str "Woops... requested resource was not found."
]

let viewUnauthorized = [
  str "Woops... you're not allowed to do this."
]

let admin model view =
  match model.State with
  | LoggedAsAdmin _ -> view
  | _ -> viewUnauthorized

let viewMain model dispatch =
  if List.isEmpty model.Albums then 
    viewLoading
  else
    match model.Route with 
    | Home        -> Home.view model
    | Manage      -> 
      admin model (Manage.view model (ManageMsg >> dispatch))
    | NewAlbum    -> 
      admin model (NewAlbum.view model (NewAlbumMsg >> dispatch))
    | Logon       -> Logon.view model (LogonMsg >> dispatch)
    | Register    -> Register.view model (RegisterMsg >> dispatch)
    | Cart        -> Cart.view model (CartMsg >> dispatch)
    | Woops       -> viewNotFound
    | Genre genre ->
      match model.Genres |> List.tryFind (fun g -> g.Name = genre) with
      | Some genre -> Genre.view genre model
      | None       -> viewNotFound
    | Album id    ->
      match model.Albums |> List.tryFind (fun a -> a.Id = id) with
      | Some album -> Album.view album model (AlbumMsg >> dispatch)
      | None       -> viewNotFound
    | EdAlbum id ->
      match model.Albums |> List.tryFind (fun a -> a.Id = id) with
      | Some album -> 
        admin model (EditAlbum.view album model (EditAlbumMsg >> dispatch))
      | None       -> viewNotFound

let blank desc url =
  a [ Href url; Target "_blank" ] [ str desc ]

let navView model =
  let cartTotal = CartItem.totalCount model.CartItems
  let tabs =
    [ yield "Home", Home
      if cartTotal > 0 then
        yield sprintf "Cart (%d)" cartTotal, Cart
      match model.State with
      | LoggedIn { Role = Admin } ->
        yield "Admin", Manage
      | _ -> ()
    ]

  list [ Id "navlist" ] tabs

let userView model dispatch =
  div [Id "part-user"] [ 
    match model.State with
    | LoggedIn creds ->
      yield str (sprintf "Logged on as %s, " creds.Name)
      yield a [Href (hash Home); onClick dispatch LogOff] [str "Log off"]
    | LoggedOff
    | CartIdOnly _ ->
      yield aHref "Log on" Logon
  ]

let genresView model =
  model.Genres 
  |> List.map (fun g -> g.Name, (Genre g.Name))
  |> list [ Id "categories" ]

let view model dispatch =
  div [] [
    div [ Id "header" ] [ 
      h1 [] [
        aHref "SAFE Music Store" Home
      ]
      navView model
      userView model dispatch
    ]

    genresView model

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