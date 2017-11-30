open System.IO

open System.Net

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Suave.Successful

open MusicStore.DTO
open ServerCode.Auth

let clientPath = Path.Combine("src","Client") |> Path.GetFullPath
let port = 8085us

let config =
  { defaultConfig with 
      homeFolder = Some clientPath
      bindings = [ HttpBinding.create HTTP (IPAddress.Parse "0.0.0.0") port ] }

let genres : Map<int, Genre> =
  [ 1, "Rock" 
    2, "Jazz" 
    3, "Metal" 
    4, "Alternative" 
    5, "Disco" 
    6, "Blues" 
    7, "Latin" 
    8, "Reggae" 
    9, "Pop" 
    10, "Classical" ]
  |> List.map (fun (k, v) -> k, { Genre.Id = k; Name = v })
  |> Map.ofList

let artists : Map<int, Artist> =
  [ 1, "AC/DC"
    2, "Accept"
    3, "Aerosmith"
    4, "Alanis Morissette"
    5, "Alice In Chains"
    6, "Antônio Carlos Jobim"
    7, "Apocalyptica"
    8, "Audioslave"
    10, "Billy Cobham"
    11, "Black Label Society"
    12, "Black Sabbath"
    14, "Bruce Dickinson"
    15, "Buddy Guy"
    16, "Caetano Veloso"
    17, "Chico Buarque"
    18, "Chico Science & Nação Zumbi"
    19, "Cidade Negra"
    20, "Cláudio Zoli"
    21, "Various Artists"
    22, "Led Zeppelin"
    23, "Frank Zappa & Captain Beefheart"
    24, "Marcos Valle"
    27, "Gilberto Gil"
    37, "Ed Motta"
    41, "Elis Regina"
    42, "Milton Nascimento"
    46, "Jorge Ben"
    50, "Metallica"
    51, "Queen"
    52, "Kiss"
    53, "Spyro Gyra"
    55, "David Coverdale"
    56, "Gonzaguinha"
    58, "Deep Purple"
    59, "Santana"
    68, "Miles Davis"
    72, "Vinícius De Moraes"
    76, "Creedence Clearwater Revival"
    77, "Cássia Eller"
    79, "Dennis Chambers"
    80, "Djavan"
    81, "Eric Clapton"
    82, "Faith No More"
    83, "Falamansa"
    84, "Foo Fighters"
    86, "Funk Como Le Gusta"
    87, "Godsmack"
    88, "Guns N'' Roses"
    89, "Incognito"
    90, "Iron Maiden"
    92, "Jamiroquai"
    94, "Jimi Hendrix"
    95, "Joe Satriani"
    96, "Jota Quest"
    98, "Judas Priest"
    99, "Legião Urbana"
    100, "Lenny Kravitz"
    101, "Lulu Santos"
    102, "Marillion"
    103, "Marisa Monte"
    105, "Men At Work"
    106, "Motörhead"
    109, "Mötley Crüe"
    110, "Nirvana"
    111, "O Terço"
    112, "Olodum"
    113, "Os Paralamas Do Sucesso"
    114, "Ozzy Osbourne"
    115, "Page & Plant"
    117, "Paul D''Ianno"
    118, "Pearl Jam"
    120, "Pink Floyd"
    124, "R.E.M."
    126, "Raul Seixas"
    127, "Red Hot Chili Peppers"
    128, "Rush"
    130, "Skank"
    132, "Soundgarden"
    133, "Stevie Ray Vaughan & Double Trouble"
    134, "Stone Temple Pilots"
    135, "System Of A Down"
    136, "Terry Bozzio, Tony Levin & Steve Stevens"
    137, "The Black Crowes"
    139, "The Cult"
    140, "The Doors"
    141, "The Police"
    142, "The Rolling Stones"
    144, "The Who"
    145, "Tim Maia"
    150, "U2"
    151, "UB40"
    152, "Van Halen"
    153, "Velvet Revolver"
    155, "Zeca Pagodinho"
    157, "Dread Zeppelin"
    179, "Scorpions"
    196, "Cake"
    197, "Aisha Duo"
    200, "The Posies"
    201, "Luciana Souza/Romero Lubambo"
    202, "Aaron Goldberg"
    203, "Nicolaus Esterhazy Sinfonia"
    204, "Temple of the Dog"
    205, "Chris Cornell"
    206, "Alberto Turco & Nova Schola Gregoriana"
    208, "English Concert & Trevor Pinnock"
    211, "Wilhelm Kempff"
    212, "Yo-Yo Ma"
    213, "Scholars Baroque Ensemble"
    217, "Royal Philharmonic Orchestra & Sir Thomas Beecham"
    219, "Britten Sinfonia, Ivor Bolton & Lesley Garrett"
    221, "Sir Georg Solti & Wiener Philharmoniker"
    223, "London Symphony Orchestra & Sir Charles Mackerras"
    224, "Barry Wordsworth & BBC Concert Orchestra"
    226, "Eugene Ormandy"
    229, "Boston Symphony Orchestra & Seiji Ozawa"
    230, "Aaron Copland & London Symphony Orchestra"
    231, "Ton Koopman"
    232, "Sergei Prokofiev & Yuri Temirkanov"
    233, "Chicago Symphony Orchestra & Fritz Reiner"
    234, "Orchestra of The Age of Enlightenment"
    236, "James Levine"
    237, "Berliner Philharmoniker & Hans Rosbaud"
    238, "Maurizio Pollini"
    240, "Gustav Mahler"
    242, "Edo de Waart & San Francisco Symphony"
    244, "Choir Of Westminster Abbey & Simon Preston"
    245, "Michael Tilson Thomas & San Francisco Symphony"
    247, "The King''s Singers"
    248, "Berliner Philharmoniker & Herbert Von Karajan"
    250, "Christopher O''Riley"
    251, "Fretwork"
    252, "Amy Winehouse"
    253, "Calexico"
    255, "Yehudi Menuhin"
    258, "Les Arts Florissants & William Christie"
    259, "The 12 Cellists of The Berlin Philharmonic"
    260, "Adrian Leaper & Doreen de Feis"
    261, "Roger Norrington, London Classical Players"
    264, "Kent Nagano and Orchestre de l''Opéra de Lyon"
    265, "Julian Bream"
    266, "Martin Roscoe"
    267, "Göteborgs Symfoniker & Neeme Järvi"
    270, "Gerald Moore"
    271, "Mela Tenenbaum, Pro Musica Prague & Richard Kapp"
    274, "Nash Ensemble"
    276, "Chic"
    277, "Anita Ward"
    278, "Donna Summer" ]
  |> List.map (fun (k, v) -> k, { Artist.Id = k; Name = v })
  |> Map.ofList

let mutable albums =
  [ 1, 1, "For Those About To Rock We Salute You", 8.99M, "/placeholder.gif"
    1, 1, "Let There Be Rock", 8.99M, "/placeholder.gif"
    1, 100, "Greatest Hits", 8.99M, "/placeholder.gif"
    1, 102, "Misplaced Childhood", 8.99M, "/placeholder.gif"
    1, 105, "The Best Of Men At Work", 8.99M, "/placeholder.gif"
    1, 110, "Nevermind", 8.99M, "/placeholder.gif"
    1, 111, "Compositores", 8.99M, "/placeholder.gif"
    1, 114, "Bark at the Moon (Remastered)", 8.99M, "/placeholder.gif"
    1, 114, "Blizzard of Ozz", 8.99M, "/placeholder.gif"
    1, 114, "Diary of a Madman (Remastered)", 8.99M, "/placeholder.gif"
    1, 114, "No More Tears (Remastered)", 8.99M, "/placeholder.gif"
    1, 114, "Speak of the Devil", 8.99M, "/placeholder.gif"
    1, 115, "Walking Into Clarksdale", 8.99M, "/placeholder.gif"
    1, 117, "The Beast Live", 8.99M, "/placeholder.gif"
    1, 118, "Live On Two Legs [Live]", 8.99M, "/placeholder.gif"
    1, 118, "Riot Act", 8.99M, "/placeholder.gif"
    1, 118, "Ten", 8.99M, "/placeholder.gif"
    1, 118, "Vs.", 8.99M, "/placeholder.gif"
    1, 120, "Dark Side Of The Moon", 8.99M, "/placeholder.gif"
    1, 124, "New Adventures In Hi-Fi", 8.99M, "/placeholder.gif"
    1, 126, "Raul Seixas", 8.99M, "/placeholder.gif"
    1, 127, "By The Way", 8.99M, "/placeholder.gif"
    1, 127, "Californication", 8.99M, "/placeholder.gif"
    1, 128, "Retrospective I (1974-1980)", 8.99M, "/placeholder.gif"
    1, 130, "Maquinarama", 8.99M, "/placeholder.gif"
    1, 130, "O Samba Poconé", 8.99M, "/placeholder.gif"
    1, 132, "A-Sides", 8.99M, "/placeholder.gif"
    1, 134, "Core", 8.99M, "/placeholder.gif"
    1, 136, "[1997] Black Light Syndrome", 8.99M, "/placeholder.gif"
    1, 139, "Beyond Good And Evil", 8.99M, "/placeholder.gif"
    1, 140, "The Doors", 8.99M, "/placeholder.gif"
    1, 141, "The Police Greatest Hits", 8.99M, "/placeholder.gif"
    1, 142, "Hot Rocks, 1964-1971 (Disc 1)", 8.99M, "/placeholder.gif"
    1, 142, "No Security", 8.99M, "/placeholder.gif"
    1, 142, "Voodoo Lounge", 8.99M, "/placeholder.gif"
    1, 144, "My Generation - The Very Best Of The Who", 8.99M, "/placeholder.gif"
    1, 150, "Achtung Baby", 8.99M, "/placeholder.gif"
    1, 150, "B-Sides 1980-1990", 8.99M, "/placeholder.gif"
    1, 150, "How To Dismantle An Atomic Bomb", 8.99M, "/placeholder.gif"
    1, 150, "Pop", 8.99M, "/placeholder.gif"
    1, 150, "Rattle And Hum", 8.99M, "/placeholder.gif"
    1, 150, "The Best Of 1980-1990", 8.99M, "/placeholder.gif"
    1, 150, "War", 8.99M, "/placeholder.gif"
    1, 150, "Zooropa", 8.99M, "/placeholder.gif"
    1, 152, "Diver Down", 8.99M, "/placeholder.gif"
    1, 152, "The Best Of Van Halen, Vol. I", 8.99M, "/placeholder.gif"
    1, 152, "Van Halen III", 8.99M, "/placeholder.gif"
    1, 152, "Van Halen", 8.99M, "/placeholder.gif"
    1, 153, "Contraband", 8.99M, "/placeholder.gif"
    1, 157, "Un-Led-Ed", 8.99M, "/placeholder.gif"
    1, 2, "Balls to the Wall", 8.99M, "/placeholder.gif"
    1, 2, "Restless and Wild", 8.99M, "/placeholder.gif"
    1, 200, "Every Kind of Light", 8.99M, "/placeholder.gif"
    1, 22, "BBC Sessions [Disc 1] [Live]", 8.99M, "/placeholder.gif"
    1, 22, "BBC Sessions [Disc 2] [Live]", 8.99M, "/placeholder.gif"
    1, 22, "Coda", 8.99M, "/placeholder.gif"
    1, 22, "Houses Of The Holy", 8.99M, "/placeholder.gif"
    1, 22, "In Through The Out Door", 8.99M, "/placeholder.gif"
    1, 22, "IV", 8.99M, "/placeholder.gif"
    1, 22, "Led Zeppelin I", 8.99M, "/placeholder.gif"
    1, 22, "Led Zeppelin II", 8.99M, "/placeholder.gif"
    1, 22, "Led Zeppelin III", 8.99M, "/placeholder.gif"
    1, 22, "Physical Graffiti [Disc 1]", 8.99M, "/placeholder.gif"
    1, 22, "Physical Graffiti [Disc 2]", 8.99M, "/placeholder.gif"
    1, 22, "Presence", 8.99M, "/placeholder.gif"
    1, 22, "The Song Remains The Same (Disc 1)", 8.99M, "/placeholder.gif"
    1, 22, "The Song Remains The Same (Disc 2)", 8.99M, "/placeholder.gif"
    1, 23, "Bongo Fury", 8.99M, "/placeholder.gif"
    1, 3, "Big Ones", 8.99M, "/placeholder.gif"
    1, 4, "Jagged Little Pill", 8.99M, "/placeholder.gif"
    1, 5, "Facelift", 8.99M, "/placeholder.gif"
    1, 51, "Greatest Hits I", 8.99M, "/placeholder.gif"
    1, 51, "Greatest Hits II", 8.99M, "/placeholder.gif"
    1, 51, "News Of The World", 8.99M, "/placeholder.gif"
    1, 52, "Greatest Kiss", 8.99M, "/placeholder.gif"
    1, 52, "Unplugged [Live]", 8.99M, "/placeholder.gif"
    1, 55, "Into The Light", 8.99M, "/placeholder.gif"
    1, 58, "Come Taste The Band", 8.99M, "/placeholder.gif"
    1, 58, "Deep Purple In Rock", 8.99M, "/placeholder.gif"
    1, 58, "Fireball", 8.99M, "/placeholder.gif"
    1, 58, "Machine Head", 8.99M, "/placeholder.gif"
    1, 58, "MK III The Final Concerts [Disc 1]", 8.99M, "/placeholder.gif"
    1, 58, "Purpendicular", 8.99M, "/placeholder.gif"
    1, 58, "Slaves And Masters", 8.99M, "/placeholder.gif"
    1, 58, "Stormbringer", 8.99M, "/placeholder.gif"
    1, 58, "The Battle Rages On", 8.99M, "/placeholder.gif"
    1, 58, "The Final Concerts (Disc 2)", 8.99M, "/placeholder.gif"
    1, 59, "Santana - As Years Go By", 8.99M, "/placeholder.gif"
    1, 59, "Santana Live", 8.99M, "/placeholder.gif"
    1, 59, "Supernatural", 8.99M, "/placeholder.gif"
    1, 76, "Chronicle, Vol. 1", 8.99M, "/placeholder.gif"
    1, 76, "Chronicle, Vol. 2", 8.99M, "/placeholder.gif"
    1, 8, "Audioslave", 8.99M, "/placeholder.gif"
    1, 82, "King For A Day Fool For A Lifetime", 8.99M, "/placeholder.gif"
    1, 84, "In Your Honor [Disc 1]", 8.99M, "/placeholder.gif"
    1, 84, "In Your Honor [Disc 2]", 8.99M, "/placeholder.gif"
    1, 84, "The Colour And The Shape", 8.99M, "/placeholder.gif"
    1, 88, "Appetite for Destruction", 8.99M, "/placeholder.gif"
    1, 88, "Use Your Illusion I", 8.99M, "/placeholder.gif"
    1, 90, "A Matter of Life and Death", 8.99M, "/placeholder.gif"
    1, 90, "Brave New World", 8.99M, "/placeholder.gif"
    1, 90, "Fear Of The Dark", 8.99M, "/placeholder.gif"
    1, 90, "Live At Donington 1992 (Disc 1)", 8.99M, "/placeholder.gif"
    1, 90, "Live At Donington 1992 (Disc 2)", 8.99M, "/placeholder.gif"
    1, 90, "Rock In Rio [CD2]", 8.99M, "/placeholder.gif"
    1, 90, "The Number of The Beast", 8.99M, "/placeholder.gif"
    1, 90, "The X Factor", 8.99M, "/placeholder.gif"
    1, 90, "Virtual XI", 8.99M, "/placeholder.gif"
    1, 92, "Emergency On Planet Earth", 8.99M, "/placeholder.gif"
    1, 94, "Are You Experienced?", 8.99M, "/placeholder.gif"
    1, 95, "Surfing with the Alien (Remastered)", 8.99M, "/placeholder.gif"
    10, 203, "The Best of Beethoven", 8.99M, "/placeholder.gif"
    10, 208, "Pachelbel: Canon & Gigue", 8.99M, "/placeholder.gif"
    10, 211, "Bach: Goldberg Variations", 8.99M, "/placeholder.gif"
    10, 212, "Bach: The Cello Suites", 8.99M, "/placeholder.gif"
    10, 213, "Handel: The Messiah (Highlights)", 8.99M, "/placeholder.gif"
    10, 217, "Haydn: Symphonies 99 - 104", 8.99M, "/placeholder.gif"
    10, 219, "A Soprano Inspired", 8.99M, "/placeholder.gif"
    10, 221, "Wagner: Favourite Overtures", 8.99M, "/placeholder.gif"
    10, 223, "Tchaikovsky: The Nutcracker", 8.99M, "/placeholder.gif"
    10, 224, "The Last Night of the Proms", 8.99M, "/placeholder.gif"
    10, 226, "Respighi:Pines of Rome", 8.99M, "/placeholder.gif"
    10, 226, "Strauss: Waltzes", 8.99M, "/placeholder.gif"
    10, 229, "Carmina Burana", 8.99M, "/placeholder.gif"
    10, 230, "A Copland Celebration, Vol. I", 8.99M, "/placeholder.gif"
    10, 231, "Bach: Toccata & Fugue in D Minor", 8.99M, "/placeholder.gif"
    10, 232, "Prokofiev: Symphony No.1", 8.99M, "/placeholder.gif"
    10, 233, "Scheherazade", 8.99M, "/placeholder.gif"
    10, 234, "Bach: The Brandenburg Concertos", 8.99M, "/placeholder.gif"
    10, 236, "Mascagni: Cavalleria Rusticana", 8.99M, "/placeholder.gif"
    10, 237, "Sibelius: Finlandia", 8.99M, "/placeholder.gif"
    10, 242, "Adams, John: The Chairman Dances", 8.99M, "/placeholder.gif"
    10, 245, "Berlioz: Symphonie Fantastique", 8.99M, "/placeholder.gif"
    10, 245, "Prokofiev: Romeo & Juliet", 8.99M, "/placeholder.gif"
    10, 247, "English Renaissance", 8.99M, "/placeholder.gif"
    10, 248, "Mozart: Symphonies Nos. 40 & 41", 8.99M, "/placeholder.gif"
    10, 250, "SCRIABIN: Vers la flamme", 8.99M, "/placeholder.gif"
    10, 255, "Bartok: Violin & Viola Concertos", 8.99M, "/placeholder.gif"
    10, 259, "South American Getaway", 8.99M, "/placeholder.gif"
    10, 260, "Górecki: Symphony No. 3", 8.99M, "/placeholder.gif"
    10, 261, "Purcell: The Fairy Queen", 8.99M, "/placeholder.gif"
    10, 264, "Weill: The Seven Deadly Sins", 8.99M, "/placeholder.gif"
    10, 266, "Szymanowski: Piano Works, Vol. 1", 8.99M, "/placeholder.gif"
    10, 267, "Nielsen: The Six Symphonies", 8.99M, "/placeholder.gif"
    10, 274, "Mozart: Chamber Music", 8.99M, "/placeholder.gif"
    2, 10, "The Best Of Billy Cobham", 8.99M, "/placeholder.gif"
    2, 197, "Quiet Songs", 8.99M, "/placeholder.gif"
    2, 202, "Worlds", 8.99M, "/placeholder.gif"
    2, 27, "Quanta Gente Veio ver--Bônus De Carnaval", 8.99M, "/placeholder.gif"
    2, 53, "Heart of the Night", 8.99M, "/placeholder.gif"
    2, 53, "Morning Dance", 8.99M, "/placeholder.gif"
    2, 6, "Warner 25 Anos", 8.99M, "/placeholder.gif"
    2, 68, "Miles Ahead", 8.99M, "/placeholder.gif"
    2, 68, "The Essential Miles Davis [Disc 1]", 8.99M, "/placeholder.gif"
    2, 68, "The Essential Miles Davis [Disc 2]", 8.99M, "/placeholder.gif"
    2, 79, "Outbreak", 8.99M, "/placeholder.gif"
    2, 89, "Blue Moods", 8.99M, "/placeholder.gif"
    3, 100, "Greatest Hits", 8.99M, "/placeholder.gif"
    3, 106, "Ace Of Spades", 8.99M, "/placeholder.gif"
    3, 109, "Motley Crue Greatest Hits", 8.99M, "/placeholder.gif"
    3, 11, "Alcohol Fueled Brewtality Live! [Disc 1]", 8.99M, "/placeholder.gif"
    3, 11, "Alcohol Fueled Brewtality Live! [Disc 2]", 8.99M, "/placeholder.gif"
    3, 114, "Tribute", 8.99M, "/placeholder.gif"
    3, 12, "Black Sabbath Vol. 4 (Remaster)", 8.99M, "/placeholder.gif"
    3, 12, "Black Sabbath", 8.99M, "/placeholder.gif"
    3, 135, "Mezmerize", 8.99M, "/placeholder.gif"
    3, 14, "Chemical Wedding", 8.99M, "/placeholder.gif"
    3, 50, "...And Justice For All", 8.99M, "/placeholder.gif"
    3, 50, "Black Album", 8.99M, "/placeholder.gif"
    3, 50, "Garage Inc. (Disc 1)", 8.99M, "/placeholder.gif"
    3, 50, "Garage Inc. (Disc 2)", 8.99M, "/placeholder.gif"
    3, 50, "Load", 8.99M, "/placeholder.gif"
    3, 50, "Master Of Puppets", 8.99M, "/placeholder.gif"
    3, 50, "ReLoad", 8.99M, "/placeholder.gif"
    3, 50, "Ride The Lightning", 8.99M, "/placeholder.gif"
    3, 50, "St. Anger", 8.99M, "/placeholder.gif"
    3, 7, "Plays Metallica By Four Cellos", 8.99M, "/placeholder.gif"
    3, 87, "Faceless", 8.99M, "/placeholder.gif"
    3, 88, "Use Your Illusion II", 8.99M, "/placeholder.gif"
    3, 90, "A Real Dead One", 8.99M, "/placeholder.gif"
    3, 90, "A Real Live One", 8.99M, "/placeholder.gif"
    3, 90, "Live After Death", 8.99M, "/placeholder.gif"
    3, 90, "No Prayer For The Dying", 8.99M, "/placeholder.gif"
    3, 90, "Piece Of Mind", 8.99M, "/placeholder.gif"
    3, 90, "Powerslave", 8.99M, "/placeholder.gif"
    3, 90, "Rock In Rio [CD1]", 8.99M, "/placeholder.gif"
    3, 90, "Rock In Rio [CD2]", 8.99M, "/placeholder.gif"
    3, 90, "Seventh Son of a Seventh Son", 8.99M, "/placeholder.gif"
    3, 90, "Somewhere in Time", 8.99M, "/placeholder.gif"
    3, 90, "The Number of The Beast", 8.99M, "/placeholder.gif"
    3, 98, "Living After Midnight", 8.99M, "/placeholder.gif"
    4, 196, "Cake: B-Sides and Rarities", 8.99M, "/placeholder.gif"
    4, 204, "Temple of the Dog", 8.99M, "/placeholder.gif"
    4, 205, "Carry On", 8.99M, "/placeholder.gif"
    4, 253, "Carried to Dust (Bonus Track Version)", 8.99M, "/placeholder.gif"
    4, 8, "Revelations", 8.99M, "/placeholder.gif"
    6, 133, "In Step", 8.99M, "/placeholder.gif"
    6, 137, "Live [Disc 1]", 8.99M, "/placeholder.gif"
    6, 137, "Live [Disc 2]", 8.99M, "/placeholder.gif"
    6, 81, "The Cream Of Clapton", 8.99M, "/placeholder.gif"
    6, 81, "Unplugged", 8.99M, "/placeholder.gif"
    6, 90, "Iron Maiden", 8.99M, "/placeholder.gif"
    7, 103, "Barulhinho Bom", 8.99M, "/placeholder.gif"
    7, 112, "Olodum", 8.99M, "/placeholder.gif"
    7, 113, "Acústico MTV", 8.99M, "/placeholder.gif"
    7, 113, "Arquivo II", 8.99M, "/placeholder.gif"
    7, 113, "Arquivo Os Paralamas Do Sucesso", 8.99M, "/placeholder.gif"
    7, 145, "Serie Sem Limite (Disc 1)", 8.99M, "/placeholder.gif"
    7, 145, "Serie Sem Limite (Disc 2)", 8.99M, "/placeholder.gif"
    7, 155, "Ao Vivo [IMPORT]", 8.99M, "/placeholder.gif"
    7, 16, "Prenda Minha", 8.99M, "/placeholder.gif"
    7, 16, "Sozinho Remix Ao Vivo", 8.99M, "/placeholder.gif"
    7, 17, "Minha Historia", 8.99M, "/placeholder.gif"
    7, 18, "Afrociberdelia", 8.99M, "/placeholder.gif"
    7, 18, "Da Lama Ao Caos", 8.99M, "/placeholder.gif"
    7, 20, "Na Pista", 8.99M, "/placeholder.gif"
    7, 201, "Duos II", 8.99M, "/placeholder.gif"
    7, 21, "Sambas De Enredo 2001", 8.99M, "/placeholder.gif"
    7, 21, "Vozes do MPB", 8.99M, "/placeholder.gif"
    7, 24, "Chill: Brazil (Disc 1)", 8.99M, "/placeholder.gif"
    7, 27, "Quanta Gente Veio Ver (Live)", 8.99M, "/placeholder.gif"
    7, 37, "The Best of Ed Motta", 8.99M, "/placeholder.gif"
    7, 41, "Elis Regina-Minha História", 8.99M, "/placeholder.gif"
    7, 42, "Milton Nascimento Ao Vivo", 8.99M, "/placeholder.gif"
    7, 42, "Minas", 8.99M, "/placeholder.gif"
    7, 46, "Jorge Ben Jor 25 Anos", 8.99M, "/placeholder.gif"
    7, 56, "Meus Momentos", 8.99M, "/placeholder.gif"
    7, 6, "Chill: Brazil (Disc 2)", 8.99M, "/placeholder.gif"
    7, 72, "Vinicius De Moraes", 8.99M, "/placeholder.gif"
    7, 77, "Cássia Eller - Sem Limite [Disc 1]", 8.99M, "/placeholder.gif"
    7, 80, "Djavan Ao Vivo - Vol. 02", 8.99M, "/placeholder.gif"
    7, 80, "Djavan Ao Vivo - Vol. 1", 8.99M, "/placeholder.gif"
    7, 81, "Unplugged", 8.99M, "/placeholder.gif"
    7, 83, "Deixa Entrar", 8.99M, "/placeholder.gif"
    7, 86, "Roda De Funk", 8.99M, "/placeholder.gif"
    7, 96, "Jota Quest-1995", 8.99M, "/placeholder.gif"
    7, 99, "Mais Do Mesmo", 8.99M, "/placeholder.gif"
    8, 100, "Greatest Hits", 8.99M, "/placeholder.gif"
    8, 151, "UB40 The Best Of - Volume Two [UK]", 8.99M, "/placeholder.gif"
    8, 19, "Acústico MTV [Live]", 8.99M, "/placeholder.gif"
    8, 19, "Cidade Negra - Hits", 8.99M, "/placeholder.gif"
    9, 21, "Axé Bahia 2001", 8.99M, "/placeholder.gif"
    9, 252, "Frank", 8.99M, "/placeholder.gif"
    5, 276, "Le Freak", 8.99M, "/placeholder.gif"
    5, 278, "MacArthur Park Suite", 8.99M, "/placeholder.gif"
    5, 277, "Ring My Bell", 8.99M, "/placeholder.gif"
  ]
  |> List.mapi (fun i (gid, aid, title, price, art) -> 
      let album : Album =
        { Id       = i + 1
          Genre    = Map.find gid genres
          Artist   = Map.find aid artists
          Title    = title
          Price    = price
          ArtUrl   = art}
      i + 1, album)
  |> Map.ofList

type User =
  { Id : int
    Name : string
    Email : string 
    Password : string
    Role : Role}

let mutable users =
  [{ Id = 1 
     Name  = "admin"
     Email = "admin@musicstore.com"
     // password is 'admin'
     Password = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918"
     Role = Admin }
   { Id = 2
     Name  = "user"
     Email = "user@musicstore.com"
     // password is 'user'
     Password = "04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb"
     Role = StandardUser }]
  |> List.map (fun u -> u.Id, u)
  |> Map.ofList

type Cart =
  { RecordId : int 
    CartId   : string
    AlbumId  : int
    Count    : int }

let mutable carts : Map<int, Cart> = 
  []
  |> Map.ofList

let OK body : WebPart = fun ctx ->
  async {
    do! Async.Sleep 1000
    return! OK body ctx
  }

let rand = System.Random()

let simulateLatency : WebPart = 
  warbler (fun _ -> 
    fun ctx ->
    async {
      do! Async.Sleep (rand.Next 1000)
      return Some ctx
    })

let getGenres = 
  genres
  |> Seq.map (fun kv -> kv.Value)
  |> Seq.toArray
  |> ServerCode.FableJson.toJson
  |> OK

let getAlbum id = 
  match Map.tryFind id albums with
  | Some album ->
    OK (ServerCode.FableJson.toJson album)
  | None ->
    RequestErrors.NOT_FOUND "Album not found"

let getAlbumsForGenre genre =
  let genre =
    genres
    |> Seq.tryFind (fun g -> g.Value.Name = genre)
    |> Option.map (fun kv -> kv.Value)
  match genre with
  | Some genre ->
    albums
    |> Seq.map (fun kv -> kv.Value)
    |> Seq.filter (fun a -> a.Genre.Name = genre.Name)
    |> Seq.toArray
    |> ServerCode.FableJson.toJson
    |> OK
  | None ->
    RequestErrors.NOT_FOUND "Genre not found"

let getAlbums = 
  warbler (fun _ ->
    albums
    |> Seq.map (fun kv -> kv.Value)
    |> Seq.toArray
    |> ServerCode.FableJson.toJson
    |> OK)

let admin success =
  ServerCode.Auth.loggedOn (function
      | Some { UserRights.Role = Admin } -> success
      | Some _ -> FORBIDDEN "Only for admin"
      | _ -> UNAUTHORIZED "Not logged in"
  )

let updateAlbum id ctx = async {
  let editedAlbum =
    ctx.request.rawForm
    |> System.Text.Encoding.UTF8.GetString
    |> ServerCode.FableJson.ofJson<Form.EditAlbum>
  
  let artist = Map.find editedAlbum.Artist artists
  let genre  = Map.find editedAlbum.Genre genres

  let album : Album =
    { Id     = editedAlbum.Id
      Artist = artist
      Genre  = genre
      Title  = editedAlbum.Title
      Price  = editedAlbum.Price
      ArtUrl = "/placeholder.gif" }

  albums <- Map.add album.Id album albums

  return! (OK (ServerCode.FableJson.toJson album) ctx)
}

open System

let passHash (pass: string) =
  use sha = Security.Cryptography.SHA256.Create()
  Text.Encoding.UTF8.GetBytes(pass)
  |> sha.ComputeHash
  |> Array.map (fun b -> b.ToString("x2"))
  |> String.concat ""


let album id =
  choose [
    PATCH  >=> admin (updateAlbum id)
  ]

let getCart id ctx = async {
  match carts |> Seq.filter (fun c -> c.Value.CartId = id) |> Seq.toList with
  | [] ->
    return! NOT_FOUND "Cart not found" ctx
  | carts ->
    let carts =
      carts
      |> List.choose (fun c -> albums 
                               |> Map.tryFind c.Value.AlbumId
                               |> Option.map (fun a -> a, c.Value.Count))
      |> List.map (fun (a, c) -> { Album = a; Count = c} )
    match carts with
    | [] ->
      return! NOT_FOUND "Cart not found" ctx
    | _ ->
      return! (OK (ServerCode.FableJson.toJson carts) ctx)
}

let addToCart id ctx = 
  async {
    let albumId =
      ctx.request.rawForm
      |> System.Text.Encoding.UTF8.GetString
      |> ServerCode.FableJson.ofJson<int>

    let cart =
      carts
      |> Seq.tryFind (fun c -> c.Value.CartId = id && c.Value.AlbumId = albumId)

    let cart : Cart =
      match cart with
      | Some kv ->
        { kv.Value with Count = kv.Value.Count + 1 }
      | None ->
        let newId = 
          if carts.IsEmpty then 1
          else
            carts 
            |> Seq.maxBy (fun kv -> kv.Key)
            |> fun x -> x.Key + 1
        { Count = 1; RecordId = newId; CartId = id; AlbumId = albumId }

    carts <- Map.add cart.RecordId cart carts
  
    return! getCart id ctx
  }

let deleteFromCart id ctx =
  async {
    let albumId =
      ctx.request.rawForm
      |> System.Text.Encoding.UTF8.GetString
      |> ServerCode.FableJson.ofJson<int>

    let cart =
      carts
      |> Seq.tryFind (fun c -> c.Value.CartId = id && c.Value.AlbumId = albumId)

    match cart with
    | Some cart ->
      if cart.Value.Count <= 1 then
        carts <- Map.remove cart.Key carts
        return! (OK (ServerCode.FableJson.toJson [||]) ctx)
      else
        let cart =
          { cart.Value with Count = cart.Value.Count - 1 }
        carts <- Map.add cart.RecordId cart carts
        return! getCart id ctx
    | _ ->
      return! NOT_FOUND "Cart not found" ctx
  }

let upgradeCart oldCartId ctx =
  async {
    let newCartId =
      ctx.request.rawForm
      |> System.Text.Encoding.UTF8.GetString
      |> ServerCode.FableJson.ofJson<string>

    let filteredCarts =
      carts
      |> Seq.map (fun kv -> kv.Value)
      |> Seq.filter (fun c -> c.CartId = oldCartId)

    for cart in filteredCarts do
      let cart' = { cart with CartId = newCartId }
      carts <- Map.add cart.RecordId cart' carts

    return! getCart newCartId ctx
  }

let cart id =
  choose [
    POST >=> addToCart id
    PATCH >=> upgradeCart id
    GET >=> getCart id
    DELETE >=> deleteFromCart id
  ]

let register ctx = async {
  let form =
    ctx.request.rawForm
    |> System.Text.Encoding.UTF8.GetString
    |> ServerCode.FableJson.ofJson<Form.Register>
  
  let newId =
    users
    |> Seq.map (fun kv -> kv.Key)
    |> Seq.max
    |> ((+)1)

  let newUser : User =
    { Id       = newId
      Name     = form.UserName
      Email    = form.Email
      Password = passHash form.Password 
      Role     = StandardUser }

  users <- Map.add newId newUser users
  
  let rights : ServerCode.Auth.UserRights = 
      { UID  = Guid.NewGuid()
        Role = newUser.Role }
  let user : MusicStore.DTO.Credentials =
      { Name  = newUser.Name
        Token = ServerCode.Auth.encode rights
        Role  = newUser.Role }
  return! (OK (ServerCode.FableJson.toJson user) ctx)
}

let app =
  choose [
    Account.webpart
    Albums.webpart
    Genres.webpart
    Artists.webpart
    Bestsellers.webpart

    pathScan "/api/album/%d" album
    pathScan "/api/cart/%s" cart
    path "/api/accounts/register" >=> POST >=> register

    Files.browseHome
  ] >=> simulateLatency

startWebServer config app