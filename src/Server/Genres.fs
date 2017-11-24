module Genres

open MusicStore.DTO
open MusicStore.DTO.ApiRemoting

let get () =
  async {
    return
      Db.ctx().Public.Genres
      |> Seq.toArray
      |> Array.map (fun g -> { Genre.Name = g.Name; Id = g.Genreid })
      |> Array.toList
  }

let webpart = 
  { Genres.get = get }
  |> fun x -> 
    Fable.Remoting.Suave.FableSuaveAdapter.webPartWithBuilderFor 
      x ApiRemoting.routeBuilder