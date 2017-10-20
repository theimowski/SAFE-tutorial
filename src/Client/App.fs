module App

open Fable.Import

let GreetingWords = "Hello world!"

// Browser is in Fable.Import namespace.
let content = Browser.document.getElementById "content"
content.innerHTML <- (sprintf "<h1>%s</h1>" GreetingWords)

Browser.window.console.log GreetingWords