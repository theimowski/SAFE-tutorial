module MusicStore.Model

open MusicStore.DTO
open MusicStore.Navigation

type User =
| LoggedOff
| CartIdOnly of string
| LoggingIn
| LoggedIn of Credentials

type Model = 
  { Route         : Route
    Albums        : WebData<AlbumDetails list>
    Artists       : WebData<Artist list>
    Genres        : WebData<Genre list>
    Bestsellers   : WebData<Bestseller list>
    SelectedAlbum : WebData<AlbumDetails option>

    User          : User
    CartItems     : CartItem list
    NewAlbum      : Form.NewAlbum
    EditAlbum     : Form.EditAlbum
    LogonForm     : Form.Logon
    RegisterForm  : Form.Register
    LogonMsg      : string option }