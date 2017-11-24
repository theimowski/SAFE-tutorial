module MusicStore.Cart

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Elmish

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View
open MusicStore.Api

type Msg = 
| Remove of Album
| RemovedFromCart of Result<CartItem[], exn>

let update msg model =
  match msg with
  | Remove album ->
    match model.User with
    | LoggedIn { Name = cartId }
    | CartIdOnly cartId ->
      model, promise removeFromCart (cartId, album.Id) RemovedFromCart
    | _ ->
      model, Cmd.none
  | RemovedFromCart (Ok items) ->
    { model with CartItems = List.ofArray items}, Cmd.none
  | RemovedFromCart (Error _) ->
    model, Cmd.none

let emptyView = 
  [ h2 [] [ str "Your cart is empty" ] 
    str "Find some great music in our "
    aHref "store" Home
    str "!" ]

let nonEmptyView items dispatch =
  [ h2 [] [ str "Review your cart:" ]
    table [] [
      thead [] [
        tr [] [
          thStr "Album Name"
          thStr "Price (each)"
          thStr "Quantity"
          thStr ""
        ]
      ]

      tbody [] [
        for i in items |> List.sortBy (fun i -> i.Album.Artist.Name) do
          yield tr [] [
            td [] [ aHref i.Album.Title (Album i.Album.Id) ]
            tdStr (string i.Album.Price)
            tdStr (string i.Count)
            td [] [ 
              a [ Href (hash Cart)
                  onClick dispatch (Remove i.Album)
            ] [ 
              str "Remove from Cart" ] 
            ]
          ]

        yield tr [] [
          tdStr "Total"
          tdStr ""
          tdStr ""
          tdStr (CartItem.totalPrice items |> string)
        ]
      ]
    ]
  ]

let view model dispatch =
  match model.CartItems with
  | [] -> emptyView
  | xs -> nonEmptyView xs dispatch