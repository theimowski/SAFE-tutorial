module MusicStore.Album

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Elmish

open MusicStore.Api
open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

type Msg =
| AddToCart of int
| AddedToCart of Result<CartItem[], exn>

let update msg model =
  match msg with
  | AddToCart albumId ->
    let cartId, model =
      match model.User with
      | LoggedIn creds -> creds.Name, model
      | CartIdOnly cartId -> cartId, model
      | LoggedOff -> 
        let uid = System.Guid.NewGuid().ToString()
        let model = { model with User = CartIdOnly uid }
        uid, model
    model, promise addToCart (cartId, albumId) AddedToCart
  | AddedToCart (Ok items) ->
    { model with CartItems = List.ofArray items}, Cmd.none
  | AddedToCart (Error _) ->
    model, Cmd.none

let labeled caption elem =
  p [] [
    em [] [ str caption ]
    elem
  ]

let view model dispatch = 
  match model.SelectedAlbum with
  | Ready (Some album) ->
    [ h2 [] [ str (sprintf "%s - %s" album.Artist album.Title) ]
      p [] [ img [ Src album.ArtUrl ] ]
      div [ Id "album-details" ] [
        labeled "Artist: " (str album.Artist)
        labeled "Title: " (str album.Title)
        labeled "Genre: " (aHref album.Genre (Genre album.Genre))
        labeled "Price: " ((str (album.Price.ToString())))
        p [ ClassName "button"; onClick dispatch (AddToCart album.Id) ] [
          a [ Href (hash Cart) ] [ str "Add to cart" ]
        ]
      ]
    ]
  | Ready None -> viewNotFound
  | Loading -> [ gear "album-details" ]
  | _ -> [ str "Failed to fetch album" ]