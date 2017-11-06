module MusicStore.View

open Fable.Core.JsInterop

open Fable.Helpers.React
open Fable.Helpers.React.Props

open MusicStore.Navigation

let aHref txt route = a [ Href (hash route) ] [ str txt ]

let thStr s = th [] [ str s ]

let tdStr s = td [] [ str s ]

let list props xs = 
  ul props [ 
    for (txt, route) in xs -> 
      li [] [ aHref txt route ] 
  ]

let onInput action = OnInput (fun e -> action !!e.target?value) 

let onClick action arg = OnClick (fun _ -> action arg)

let formLbl label = div [ ClassName "editor-label" ] [ str label ]
let formFld field = div [ ClassName "editor-field" ] [ field ]
let select props options selected = 
  let options =
    options
    |> List.map (fun (v,txt) -> 
      option [Value v; Selected (v = selected) ] [str txt])
  select props options
