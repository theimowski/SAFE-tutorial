module MusicStore.Model

open MusicStore.DTO
open MusicStore.Navigation

type User =
| LoggedOff
| CartIdOnly of string
| LoggedIn of Credentials

type Model = 
  { Route         : Route
    Genres        : WebData<Genre list>
    Bestsellers   : WebData<Bestseller list>
    SelectedAlbum : WebData<AlbumDetails option>
    Albums        : WebData<AlbumDetails list>

    Artists       : Artist list
    User          : User
    CartItems     : CartItem list
    NewAlbum      : Form.NewAlbum
    EditAlbum     : Form.EditAlbum
    LogonForm     : Form.Logon
    RegisterForm  : Form.Register
    LogonMsg      : string option }