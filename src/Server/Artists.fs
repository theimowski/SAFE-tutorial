module Artists

open MusicStore.DTO
open MusicStore.DTO.ApiRemoting

let get () =
  async {
    return
      Db.ctx().Public.Artists
      |> Seq.toList
      |> List.map (fun a -> 
        { Artist.Id = a.Artistid
          Name = a.Name  })
  }

let webpart = 
  { Artists.get = get }
  |> fun x -> 
    Fable.Remoting.Suave.FableSuaveAdapter.webPartWithBuilderFor 
      x ApiRemoting.routeBuilder