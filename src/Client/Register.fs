module MusicStore.Register

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
| Email    of string
| Password of string
| RepeatPassword of string
| Register of Form.Register
| Registered of Result<Credentials, exn>

let init () : Form.Register =
  { UserName = ""
    Email    = ""
    Password = ""
    RepeatPassword = "" }

let update msg model =
  let set register = { model with RegisterForm = register }
  match msg with 
  | UserName x -> set { model.RegisterForm with UserName = x }, Cmd.none
  | Email x -> set { model.RegisterForm with Email = x }, Cmd.none
  | Password x -> set { model.RegisterForm with Password = x }, Cmd.none
  | RepeatPassword x -> set { model.RegisterForm with RepeatPassword = x }, Cmd.none
  | Register form -> 
    model, promise register form Registered
  | Registered (Ok creds) ->
    { model with User = LoggedIn creds }, redirect Home
  | Registered (Error _) ->
    model, Cmd.none

let view model dispatch = [
  h2 [] [ str "Create a New Account" ]
  p [] [ str "Use the form below to create a new account." ]
  form [ ] [
    fieldset [] [
      legend [] [ str "Create a New Account" ]
      formLbl "User name"
      formFld 
        (input [Value model.RegisterForm.UserName
                Type "text"
                onInput (UserName >> dispatch)])
      formLbl "Email"
      formFld 
        (input [Value model.RegisterForm.Email
                Type "text"
                onInput (Email >> dispatch)])
      formLbl "Password"
      formFld 
        (input [Value model.RegisterForm.Password
                Type "password"
                onInput (Password >> dispatch)])
      formLbl "Repeat password"
      formFld 
        (input [Value model.RegisterForm.RepeatPassword
                Type "password"
                onInput (RepeatPassword >> dispatch)])
    ]
  ]
  button [ ClassName "button"; onClick dispatch (Register model.RegisterForm) ] [ str "Register" ]
]