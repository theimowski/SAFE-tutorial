module Bestsellers

open MusicStore.DTO
open MusicStore.DTO.ApiRemoting

let get () =
  async {
    return
      Db.ctx().Public.Bestsellers
      |> Seq.toList
      |> List.map (fun b -> 
        { Bestseller.Id = b.Albumid
          Title  = b.Title
          ArtUrl = b.Albumarturl })
  }

let webpart = 
  { Bestsellers.get = get }
  |> fun x -> 
    Fable.Remoting.Suave.FableSuaveAdapter.webPartWithBuilderFor 
      x ApiRemoting.routeBuilder