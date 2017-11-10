module MusicStore.Cart

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

let emptyView = 
  [ h2 [] [ str "Your cart is empty" ] 
    str "Find some great music in our "
    aHref "store" Home
    str "!" ]

let nonEmptyView items =
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
          td [] [ a [ Href (hash Cart) ] [ str "Remove from Cart" ] ]
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

let view model =
  match model.CartItems with
  | [] -> emptyView
  | xs -> nonEmptyView xs