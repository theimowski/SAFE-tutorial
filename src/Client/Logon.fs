module MusicStore.Logon

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Elmish

open MusicStore.Api
open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View
open System.Net
open System.Threading

type Msg =
| UserName         of string
| Password         of string
| Logon            of Form.Logon
| LoggedOn         of Result<Credentials, exn>
| CartItemsFetched of Result<CartItem[], exn>

let init () : Form.Logon =
  { UserName = ""
    Password = "" }

let update msg model =
  let set logon = { model with LogonForm = logon }
  match msg with 
  | UserName name -> set { model.LogonForm with UserName = name }, Cmd.none
  | Password pass -> set { model.LogonForm with Password = pass }, Cmd.none
  | Logon form -> 
    model, promise logon form LoggedOn
  | LoggedOn (Ok creds) ->
    let cartCmd =
      match model.State with
      | CartIdOnly oldCartId ->
        promise upgradeCart (oldCartId, creds.Name) CartItemsFetched
      | _ ->
        promise cartItems creds.Name CartItemsFetched
    let cmd = 
      Cmd.batch [
        redirect Home
        cartCmd
      ]
    { model with State = LoggedIn creds }, cmd
  | LoggedOn (Error _) ->
    let msg = "Incorrect Login or Password"
    { model with LogonMsg = Some msg }, Cmd.none
  | CartItemsFetched (Ok items) ->
    { model with CartItems = List.ofArray items }, Cmd.none
  | CartItemsFetched (Error _) ->
    model, Cmd.none


let view model dispatch = [
  h2 [] [str "Log On"]
  p [] [ 
    str "Please enter your user name and password."
    aHref " Register" Register
    str " if you don't have an account yet."
  ]

  div [Id "logon-message"] [str (defaultArg model.LogonMsg "")]

  form [ ] [
    fieldset [] [
      legend [] [ str "Account Information" ]
      formLbl "User Name"
      formFld 
        (input [Value model.LogonForm.UserName
                Type "text"
                onInput (UserName >> dispatch)])
      formLbl "Password"
      formFld 
        (input [Value model.LogonForm.Password
                Type "password"
                onInput (Password >> dispatch)])
    ]
  ]
  button [ ClassName "button"; onClick dispatch (Logon model.LogonForm) ] [ str "Log in" ]
]