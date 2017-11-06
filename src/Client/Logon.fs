module MusicStore.Logon

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Elmish

open MusicStore.Api
open MusicStore.DTO
open MusicStore.Model
open MusicStore.Navigation
open MusicStore.View

type Msg =
| UserName of string
| Password of string
| Logon    of Form.Logon
| LoggedOn of Result<User, exn>

let init () : Form.Logon =
  { UserName = ""
    Password = "" }

let update msg model =
  let set logon = { model with LogonForm = logon }
  match msg with 
  | UserName name -> set { model.LogonForm with UserName = name }, Cmd.none
  | Password pass -> set { model.LogonForm with Password = pass }, Cmd.none
  | Logon form -> 
    let cmd = 
      Cmd.batch [ promise logon form LoggedOn 
                  redirect Home ]
    model, cmd
  | LoggedOn (Ok user) -> 
    { model with User = Some user }, Cmd.none
  | LoggedOn _ -> model, Cmd.none

let view model dispatch = [
  h2 [] [str "Log On"]
  p [] [ str "Please enter your user name and password."]

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