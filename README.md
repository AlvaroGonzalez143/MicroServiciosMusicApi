# MicroServiciosMusicApi

## Casos de uso

### CU: Consultar musica

- Precondicion: Ninguna

- Camino normal:

  -Al consultar por el catalogo de musica el microservicio respondera con un cod 201 y respondera en forma de un json con el titulo de la cancion el album al que pertenece y el artista

- Caminos alternativos:
  - Si no existiece ninguna cancion en la base de datos este respondera con un codigo 404 y enviara un json con el mensaje Actualmente el listado se encuentra vacio.
  - Si se accede al endpoint de consultar la existencia de un album tendra el mismo proceder, con la diferencia que solo traera como respuesta dicho album si existiece
  - Si se accede al endpoint para consultar la existencia de una cancion tendra el mismo proceder, con la diferencia que solo traera como respuesta dicha cancion si existiece
  - Si se quisiera cargar una cancion con el mismo nombre y del mismo artista el microservicio dejara un mensaje de error por la existencia de dicha cancion 

### CU: Cargar Cancion/Album

- Precondición: Ninguna

- Camino normal:
  
-Se solicita la carga de una cancion
-El microservicio busca en la BD la existencia de dicha cancion con dicho artista
-Si no existe se carga asignandole como predeterminado cancion suelta es decir sin album al que pertenece

- Caminos alternativos:

- Si se especifica el album de la cancion este se creara en caso de no existir y se asociara a dicha cancion o en caso de existir se asociara la cancion al album con el nombre especificado anteriormente
-Mostrara un error si la cancion ya existe 

### CU: Descargar Musica/Album

 Camino normal:
  
-Se solicita la descarga de una cancion
-El microservicio busca en la BD la existencia de dicha cancion con dicho artista
-Si no existe se descarga en un archivo zip que contendra la cancion y la imagen asignada
  - Caminos alternativos:
    -Si no existe o el archivo mp3 no se puede acceder enviara un codigo de error
    -Si se solicita un album completo se descargaran todas las canciones de dicho album y se comprimiran en un zip

## MODELO DE DATOS/ENTIDADES

**Song**
 Id
 Name 
 Lyrics 
 Artist 
 ImageRuta 
 RutaArchivo
 AlbumId
 ArchivoMp3  
 image 
 Album 

 **Album**
 Id
 Title
 Artista
 ReleaseYear
 Songs

 **ENDPOINTS**
 /ConsultarCatalogo/v1/Album/{Name}/{Artist}
 METODO GET : DEVUELVE UN ALBUM CON SUS CANCIONES Y EL NOMBRE DE LAS MISMAS EN FORMATO JSON
 
 /ConsultarCatalogo/v1/{Name}/{Artist}
  METODO GET : DEVUELVE UNA STATUS en el que se indica la existencia de la cancion solicitada
  
 /ConsultarCatalogo/v1
   METODO GET : DEVUELVE UNA LISTA CON LOS ALBUMS Y SUS RESPECTIVAS CANCIONES ASOCIADAS

/AgregarCancion/v1
METODO POST : AGREGA AL MICROSERVICIO LA CANCION SOLICITADA 
El formato de la solicitud post debe ser 

NAME [obligatorio]
Description [obligatorio]
Artist [obligatorio]
image[OPCIONAL]
ArchivoMp3[Obligatorio]
Album[Opcional] 
Dentro de album debe incluirse :
Title [obligatorio]
Artista [obligatorio]
ReleaseYear [obligatorio]

/DescargarAlbum/v1/{AlbumName}/{Artista}
METODO GET: DEVUELVE EL ARCHIVO ZIP DEL ALBUM

/DescargarCancion/v1/{songName}/{Artista}
METODO GET: DEVUELVE EL ARCHIVO ZIP DE LA CANCION ALBUM

LAS TABLAS DE LA BASE DE DATOS SON LAS SIGUIENTES
album :
ID INT pk
Title VARCHAR
Artista VARCHAR
RelaseYear int

song 
ID INT pk
Name Varchar
Lyrics Text
Artist Varchar
RutaArchivo Varchar
ImageRuta Varchar
AlbumID INT fk