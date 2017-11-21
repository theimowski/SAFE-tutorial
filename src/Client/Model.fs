module MusicStore.Model

open MusicStore.DTO
open MusicStore.Navigation

type State =
| LoggedOff
| CartIdOnly of string
| LoggedIn of Credentials

type Model = 
  { Route        : Route
    Genres       : Genre list
    Artists      : Artist list
    Albums       : Album list
    Bestsellers  : Bestseller list
    State        : State
    CartItems    : CartItem list
    NewAlbum     : Form.NewAlbum
    EditAlbum    : Form.EditAlbum
    LogonForm    : Form.Logon
    RegisterForm : Form.Register
    LogonMsg     : string option }