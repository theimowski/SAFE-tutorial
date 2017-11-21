module Genres

open MusicStore.DTO
open MusicStore.DTO.ApiRemoting

let get () =
  async {
    return
      Db.ctx().Public.Genres
      |> Seq.toArray
      |> Array.map (fun g -> { Genre.Name = g.Name; Id = g.Genreid })
  }

let webpart = 
  { get = get }
  |> fun x -> 
    Fable.Remoting.Suave.FableSuaveAdapter.webPartWithBuilderFor 
      x (sprintf "/api/%s/%s") 