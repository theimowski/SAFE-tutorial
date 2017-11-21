module Db

open FSharp.Data.Sql

[<Literal>]
let resolutionPath = @"c:\github\SAFE-tutorial\src\Server\sqlprovider_dlls"

[<Literal>]
let TPConnectionString = 
  "Server=192.168.99.100;"    + 
  "Database=suavemusicstore;" + 
  "User Id=suave;"            + 
  "Password=1234;"

type Sql = 
  SqlDataProvider< 
    ConnectionString      = TPConnectionString,
    DatabaseVendor        = Common.DatabaseProviderTypes.POSTGRESQL,
    ResolutionPath        = resolutionPath,
    CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL >

type DbContext = Sql.dataContext
type Album = DbContext.``public.albumsEntity``
type Genre = DbContext.``public.genresEntity``
type AlbumDetails = DbContext.``public.albumdetailsEntity``
type Artist = DbContext.``public.artistsEntity``
type User = DbContext.``public.usersEntity``
type CartDetails = DbContext.``public.cartdetailsEntity``
type Cart = DbContext.``public.cartsEntity``
type BestSeller = DbContext.``public.bestsellersEntity``
