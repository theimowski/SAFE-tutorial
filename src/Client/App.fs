module MusicStore.App

open Elmish
open Elmish.Browser.Navigation
open Elmish.React

open Fable.PowerPack
open Fable.PowerPack.Fetch

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

type Msg =
| AlbumsFetched of Result<Album[], exn>
| AlbumDeleted of Result<int, exn>
| ManageMsg of Manage.Msg

let albums () =
  fetchAs<Album[]> "/api/albums" []

let delete album =
  fetchAs<int> (sprintf "/api/album/%d" album.Id) [Method HttpMethod.DELETE]

let promise req args f = 
  Cmd.ofPromise req args (Ok >> f) (Error >> f)

let init route =
  let route = defaultArg route Home
  let model =
    { Route   = route
      Artists = []
      Genres  = []
      Albums  = [] }
  
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
    let model =
      { model with 
          Albums  = albums
          Genres  = genres
          Artists = artists }
    model, Cmd.none
  | AlbumsFetched (Error _)
  | AlbumDeleted  (Error _) ->
    model, Cmd.none
  | AlbumDeleted (Ok id) ->
    let albums = List.filter (fun a -> a.Id <> id) model.Albums
    { model with Albums = albums }, Cmd.none
  | ManageMsg (Manage.DeleteAlbum album) ->
    model, promise delete album AlbumDeleted

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
    | NewAlbum    -> NewAlbum.view model
    | Woops       -> viewNotFound
    | Genre genre ->
      match model.Genres |> List.tryFind (fun g -> g.Name = genre) with
      | Some genre -> Genre.view genre model
      | None       -> viewNotFound
    | Album id    ->
      match model.Albums |> List.tryFind (fun a -> a.Id = id) with
      | Some album -> Album.view album model
      | None       -> viewNotFound

let blank desc url =
  a [ Href url; Target "_blank" ] [ str desc ]

let view model dispatch =
  div [] [
    div [ Id "header" ] [ 
      h1 [] [
        aHref "SAFE Music Store" Home
      ]
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