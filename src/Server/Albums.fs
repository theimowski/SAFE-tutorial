module Albums

open MusicStore.DTO
open MusicStore.DTO.ApiRemoting
open MusicStore

let albumDetails (a : Db.AlbumDetails) =
  { Id     = a.Albumid
    Title  = a.Title
    Genre  = a.Genre
    Artist = a.Artist
    Price  = a.Price
    ArtUrl = a.Albumarturl }

let getAll () =
  async {
    return
      Db.ctx().Public.Albumdetails
      |> Seq.toList
      |> List.map albumDetails
  }

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

let delete id =
  async {
    let ctx = Db.ctx()
    let album =
      query {
        for album in ctx.Public.Albums do
          where (album.Albumid = id)
          select album
      }
      |> Seq.tryHead
    match album with
    | Some a ->
      a.Delete()
      ctx.SubmitUpdates()
    | None ->
      failwith "album not found"
    return id
  }

let create (form : Form.NewAlbum) =
  async {
    let ctx = Db.ctx()
    let album =
      ctx.Public.Albums.Create(
        form.Artist,
        form.Genre,
        form.Price,
        form.Title)
    ctx.SubmitUpdates()
  }

let webpart = 
  { getAll      = getAll
    getById     = getById
    getForGenre = getForGenre
    create      = create
    delete      = delete }
  |> fun x -> 
    Fable.Remoting.Suave.FableSuaveAdapter.webPartWithBuilderFor 
      x ApiRemoting.routeBuilder