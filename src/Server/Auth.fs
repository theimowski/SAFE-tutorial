module ServerCode.Auth

open System.Security.Cryptography

open Suave
open Suave.RequestErrors

open Newtonsoft.Json

open Jose

open MusicStore.DTO
open Suave.Http.ServerKey

let passPhrase = 
  let crypto = RandomNumberGenerator.Create()
  let randomNumber = Array.init 32 byte
  crypto.GetBytes(randomNumber)
  randomNumber

let alg = JweAlgorithm.A256KW
let enc = JweEncryption.A256CBC_HS512

let private encodeString (payload:string) =
  JWT.Encode(payload, passPhrase, alg, enc)

let private decodeString (jwt:string) =
  JWT.Decode(jwt, passPhrase, alg, enc)

type UserRights = 
  { UID  : System.Guid
    Role : Role }

let encode (token : UserRights) =
  JsonConvert.SerializeObject token
  |> encodeString

let decode (jwt : string) : UserRights option =
  try 
    decodeString jwt
    |> JsonConvert.DeserializeObject<UserRights>
    |> Some
  with _ ->
    None

let bearerPrefix = "Bearer "

let loggedOn success ctx = 
  async {
    let credentials =
      match ctx.request.header "Authorization" with
      | Choice1Of2 token when token.StartsWith bearerPrefix ->
        let jwt = token.Substring(bearerPrefix.Length)
        decode jwt
      | _ ->
        None
    return! (success credentials) ctx
  }