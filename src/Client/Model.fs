module MusicStore.Model

open MusicStore.DTO
open MusicStore.Navigation

type State =
| LoggedOff
| LoggedIn of Credentials

type Model = 
  { Route     : Route
    Genres    : Genre list
    Artists   : Artist list
    Albums    : Album list
    State     : State
    CartItems : CartItem list
    NewAlbum  : Form.NewAlbum
    EditAlbum : Form.EditAlbum
    LogonForm : Form.Logon
    LogonMsg  : string option }