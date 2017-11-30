module MusicStore.DTO

type Genre =
  { Id   : int
    Name : string }

type Bestseller =
  { Id       : int
    Title    : string
    ArtUrl   : string }

type Artist =
  { Id   : int
    Name : string }

type Album =
  { Id       : int
    Genre    : Genre
    Artist   : Artist
    Title    : string
    Price    : decimal
    ArtUrl   : string }

type AlbumDetails =
  { Id       : int
    Genre    : string
    Artist   : string
    Title    : string
    Price    : decimal
    ArtUrl   : string }

type Role =
| Admin
| StandardUser

type CartItem =
  { Album : Album
    Count : int }

module CartItem =
  let totalPrice =
    List.sumBy (fun item -> item.Album.Price * (decimal) item.Count)

  let totalCount =
    List.sumBy (fun item -> item.Count)

type Credentials =
  { Name  : string
    Token : string
    Role  : Role }

module Form =

  type NewAlbum =
    { Genre  : int
      Artist : int
      Title  : string
      Price  : decimal }

  type EditAlbum =
    { Id     : int
      Genre  : int
      Artist : int
      Title  : string
      Price  : decimal }

  type Logon =
    { UserName : string 
      Password : string }

  type Register =
     { UserName : string
       Email    : string
       Password : string
       RepeatPassword : string }

module ApiRemoting =
  let routeBuilder = sprintf "/api/%s/%s"

  type Albums = {
    getAll      : unit -> Async<AlbumDetails list>
    getById     : int -> Async<AlbumDetails option>
    getForGenre : string -> Async<AlbumDetails list>
    create      : Form.NewAlbum -> Async<unit>
    delete      : int -> Async<int>
  }

  type Genres = {
    get : unit -> Async<Genre list>
  }

  type Bestsellers = {
    get : unit -> Async<Bestseller list>
  }

  type Account = {
    logon : Form.Logon -> Async<Credentials option>
  }

  type Artists = {
    get : unit -> Async<Artist list>
  }