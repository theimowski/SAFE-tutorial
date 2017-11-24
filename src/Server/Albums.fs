module Albums

open MusicStore.DTO
open MusicStore.DTO.ApiRemoting

let get id =
  async {
    return
      query { 
        for album in Db.ctx().Public.Albumdetails do
          where (album.Albumid = id)
          select album
      }
      |> Seq.tryHead
      |> Option.map (fun a -> 
        { Id     = a.Albumid
          Title  = a.Title
          Genre  = a.Genre
          Artist = a.Artist
          Price  = a.Price
          ArtUrl = a.Albumarturl })
  }

let webpart = 
  { Albums.getById = get }
  |> fun x -> 
    Fable.Remoting.Suave.FableSuaveAdapter.webPartWithBuilderFor 
      x ApiRemoting.routeBuilder