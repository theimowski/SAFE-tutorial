module MusicStore.Model

open MusicStore.DTO
open MusicStore.Navigation

type Model = 
  { Route     : Route
    Genres    : Genre list
    Artists   : Artist list
    Albums    : Album list
    User      : User option
    NewAlbum  : Form.NewAlbum
    EditAlbum : Form.EditAlbum
    LogonForm : Form.Logon }