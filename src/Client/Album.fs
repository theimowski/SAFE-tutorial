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
| AddedToCart of Api2.Response<CartItem[]>

let update msg model =
  match msg with
  | AddToCart albumId ->
    let cartId, model =
      match model.State with
      | LoggedIn creds -> creds.Name, model
      | CartIdOnly cartId -> cartId, model
      | LoggedOff -> 
        let uid = System.Guid.NewGuid().ToString()
        let model = { model with State = CartIdOnly uid }
        uid, model
    model, promise2 (Api2.Cart.add cartId) albumId AddedToCart
  | AddedToCart (Api2.Ok items) ->
    { model with CartItems = List.ofArray items}, Cmd.none
  | AddedToCart (Api2.Exception _) ->
    model, Cmd.none

let labeled caption elem =
  p [] [
    em [] [ str caption ]
    elem
  ]

let view album model dispatch = [
  h2 [] [ str (sprintf "%s - %s" album.Artist.Name album.Title) ]
  p [] [ img [ Src album.ArtUrl ] ]
  div [ Id "album-details" ] [
    labeled "Artist: " (str album.Artist.Name)
    labeled "Title: " (str album.Title)
    labeled "Genre: " (aHref album.Genre.Name (Genre album.Genre.Name))
    labeled "Price: " ((str (album.Price.ToString())))
    p [ ClassName "button"; onClick dispatch (AddToCart album.Id) ] [
      a [ Href (hash Cart) ] [ str "Add to cart" ]
    ]
  ]
]