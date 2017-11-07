module ServerCode.Auth

open System.Security.Cryptography

open Suave
open Suave.RequestErrors

open Newtonsoft.Json

open Jose

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

let encode token =
  JsonConvert.SerializeObject token
  |> encodeString

let decode<'a> (jwt:string) : 'a =
  decodeString jwt
  |> JsonConvert.DeserializeObject<'a>

type UserRights =
  { Name : string }

let validate (jwt:string) : UserRights option =
  try
    let token = decode jwt
    Some token
  with
  | _ -> None
