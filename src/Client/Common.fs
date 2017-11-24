namespace MusicStore

type WebData<'a> =
| NotAsked
| Loading
| Ready  of 'a
| Failed of exn