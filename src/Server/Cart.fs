module Cart

open MusicStore.DTO

let addToCart cartId =
  ()

let webpart id = Common.mkWebpart (Api2.Cart.add id) addToCart
