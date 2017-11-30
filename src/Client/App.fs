module MusicStore.App

open Elmish
open Elmish.Browser.Navigation
open Elmish.React

open MusicStore.Api
open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View
open MusicStore.Api.Remoting
open MusicStore.Navigation
open MusicStore.DTO.Form
open MusicStore.DTO.ApiRemoting
open Fable.PowerPack.PromiseImpl
open MusicStore.Api.Remoting

type Msg =
| GenresFetched      of WebData<Genre list>
| BestsellersFetched of WebData<Bestseller list>
| AlbumFetched       of WebData<AlbumDetails option>
| AlbumsFetched      of WebData<AlbumDetails list>
| ArtistsFetched     of WebData<Artist list>

| ManageMsg of Manage.Msg
| NewAlbumMsg of NewAlbum.Msg
| EditAlbumMsg of EditAlbum.Msg
| LogonMsg of Logon.Msg
| RegisterMsg of Register.Msg
| AlbumMsg of Album.Msg
| CartMsg of Cart.Msg
| LogOff

let routeUpdate model = function
| Album id ->
  { model with SelectedAlbum = Loading }, promiseWD albums.getById id AlbumFetched
| Genre g ->
  { model with Albums = Loading }, promiseWD albums.getForGenre g AlbumsFetched
| Logon ->
  { model with LogonForm = Logon.init(); LogonMsg = None }, Cmd.none
| Manage ->
  { model with Albums = Loading }, promiseWD albums.getAll () AlbumsFetched
| _ ->
  model, Cmd.none

let urlUpdate (result:Option<Route>) model =
  let model, cmd = 
    match result with
    | Some route ->
      routeUpdate model route 
    | None ->
      model, Cmd.none

  match result with
  | Some route ->
    { model with Route = route }, cmd
  | None ->
    { model with Route = Woops }, Navigation.modifyUrl (hash Woops)

let init route =

  let initModel =
    { Route         = Home
      Albums        = NotAsked
      Artists       = NotAsked
      Genres        = Loading
      Bestsellers   = Loading
      SelectedAlbum = NotAsked

      User          = LoggedOff
      CartItems     = []
      NewAlbum      = NewAlbum.init ()
      EditAlbum     = EditAlbum.initEmpty ()
      LogonForm     = Logon.init ()
      RegisterForm  = Register.init()
      LogonMsg      = None }

  let initCmd =
    Cmd.batch [
      promiseWD genres.get () GenresFetched
      promiseWD bestsellers.get () BestsellersFetched
      promiseWD artists.get () ArtistsFetched
    ]

  match route with
  | Some r ->
    let model, cmd = urlUpdate (Some r) initModel
    model, Cmd.batch [ initCmd; cmd ]
  | None ->
    initModel, initCmd

let update msg (model : Model) =
  match msg with
  | GenresFetched genres ->
    { model with Genres = genres }, Cmd.none
  | BestsellersFetched bestsellers ->
    { model with Bestsellers = bestsellers }, Cmd.none
  | AlbumFetched album ->
    { model with SelectedAlbum = album }, Cmd.none
  | AlbumsFetched albums ->
    { model with Albums = albums }, Cmd.none
  | ArtistsFetched artists ->
    { model with Artists = artists }, Cmd.none

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
        User     = LoggedOff
        CartItems = [] }, Cmd.none

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import.Browser

let viewLoading = [ str "Loading..." ]

let viewUnauthorized = [
  str "Woops... you're not allowed to do this."
]

let admin model view =
  match model.User with
  | LoggedIn { Role = Admin } -> view
  | _ -> viewUnauthorized

let viewMain model dispatch =
  match model.Route with 
  | Home        -> Home.view model dispatch
  | Manage      -> 
    admin model (Manage.view model (ManageMsg >> dispatch))
  | NewAlbum    -> 
    NewAlbum.view model (NewAlbumMsg >> dispatch)
  | Logon       -> Logon.view model (LogonMsg >> dispatch)
  | Register    -> Register.view model (RegisterMsg >> dispatch)
  | Cart        -> Cart.view model (CartMsg >> dispatch)
  | Woops       -> viewNotFound
  | Genre genre -> Genre.view genre model
  | Album id    -> Album.view model (AlbumMsg >> dispatch)
  | EdAlbum id -> viewNotFound

let blank desc url =
  a [ Href url; Target "_blank" ] [ str desc ]

let navView model =
  let cartTotal = CartItem.totalCount model.CartItems
  let tabs =
    [ yield "Home", Home
      if cartTotal > 0 then
        yield sprintf "Cart (%d)" cartTotal, Cart
      match model.User with
      | LoggedIn { Role = Admin } ->
        yield "Admin", Manage
      | _ -> ()
    ]

  list [ Id "navlist" ] tabs

let userView model dispatch =
  div [Id "part-user"] [ 
    match model.User with
    | LoggedIn creds ->
      yield str (sprintf "Logged on as %s, " creds.Name)
      yield a [Href (hash Home); onClick dispatch LogOff] [str "Log off"]
    | LoggingIn ->
      yield gear ""
    | LoggedOff
    | CartIdOnly _ ->
      yield aHref "Log on" Logon
  ]

let genresView model =
  match model.Genres with
  | Loading ->
    gear "categories"
  | Ready genres ->
    genres
    |> List.map (fun g -> g.Name, (Genre g.Name))
    |> list [ Id "categories" ]
  | _ ->
    str "Failed to load genres"

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

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif
Program.mkProgram init update view
|> Program.toNavigable parser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run