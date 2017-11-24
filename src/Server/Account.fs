module Account

open System

open MusicStore.DTO
open MusicStore.DTO.ApiRemoting


let passHash (pass: string) =
  use sha = Security.Cryptography.SHA256.Create()
  Text.Encoding.UTF8.GetBytes(pass)
  |> sha.ComputeHash
  |> Array.map (fun b -> b.ToString("x2"))
  |> String.concat ""

let parseRole = function
| "admin" -> Admin
| "user"  -> StandardUser
| x       -> failwithf "cannot parse role '%s'" x

let logon (form : Form.Logon) = 
  async {
    let user =
      query {
        for user in Db.ctx().Public.Users do
            where (user.Username = form.UserName && user.Password = passHash form.Password)
            select user
      } |> Seq.tryHead

    match user with
    | Some u ->
      let rights : ServerCode.Auth.UserRights = 
        { UID  = Guid.NewGuid()
          Role = parseRole u.Role }
      let user : MusicStore.DTO.Credentials =
        { Name  = u.Username
          Token = ServerCode.Auth.encode rights
          Role  = parseRole u.Role }
      return Some user
    | None ->
      return None
  }


let webpart = 
  { Account.logon = logon }
  |> fun x -> 
    Fable.Remoting.Suave.FableSuaveAdapter.webPartWithBuilderFor 
      x ApiRemoting.routeBuilder