open Suave

let config =
  { defaultConfig with homeFolder = Some @"c:\github\SAFE-tutorial\src\Client" }

startWebServer config Files.browseHome