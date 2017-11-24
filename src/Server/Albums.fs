module Albums

open MusicStore.DTO
open MusicStore.DTO.ApiRemoting

let albumDetails (a : Db.AlbumDetails) =
  { Id     = a.Albumid
    Title  = a.Title
    Genre  = a.Genre
    Artist = a.Artist
    Price  = a.Price
    ArtUrl = a.Albumarturl }

let getById id =
  async {
    return
      query { 
        for album in Db.ctx().Public.Albumdetails do
          where (album.Albumid = id)
          select album
      }
      |> Seq.tryHead
      |> Option.map albumDetails
  }

let getForGenre genre =
  async {
    return
      query {
        for album in Db.ctx().Public.Albumdetails do
          where (album.Genre = genre)
          select album
      }
      |> Seq.toList
      |> List.map albumDetails
  }

let webpart = 
  { getById     = getById
    getForGenre = getForGenre }
  |> fun x -> 
    Fable.Remoting.Suave.FableSuaveAdapter.webPartWithBuilderFor 
      x ApiRemoting.routeBuilder