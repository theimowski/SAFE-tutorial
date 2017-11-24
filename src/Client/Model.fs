module MusicStore.Model

open MusicStore.DTO
open MusicStore.Navigation

type User =
| LoggedOff
| CartIdOnly of string
| LoggedIn of Credentials

type Model = 
  { Genres       : WebData<Genre list>
    Bestsellers  : WebData<Bestseller list>
    
    Route        : Route
    Artists      : Artist list
    Albums       : Album list
    User         : User
    CartItems    : CartItem list
    NewAlbum     : Form.NewAlbum
    EditAlbum    : Form.EditAlbum
    LogonForm    : Form.Logon
    RegisterForm : Form.Register
    LogonMsg     : string option }