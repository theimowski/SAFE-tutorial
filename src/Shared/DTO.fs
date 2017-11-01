module Shared.DTO

type Genre =
  { Id   : int
    Name : string }

type Artist =
  { Id   : int
    Name : string }

type Album =
  { Id       : int
    Genre    : string 
    ArtistId : int
    Title    : string 
    Price    : decimal
    ArtUrl   : string }