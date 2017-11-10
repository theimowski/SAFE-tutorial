module MusicStore.DTO

type Genre =
  { Id   : int
    Name : string }

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