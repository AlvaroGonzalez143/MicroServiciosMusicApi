using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc; // Requerido para [FromForm]
using Microsoft.EntityFrameworkCore; //
using System.IO.Compression; // Necesario para ZipArchive
using System.Runtime.InteropServices;
using System.Text.Json;
namespace YourProject.Endpoints;
public static class DescargarEndpoint
{
    // El método de extensión toma 'this WebApplication app' como parámetro
    public static void Descargar(this WebApplication app)
    {
        app.MapGet("/DescargarCancion/v1/{songName}/{Artista}", async (string songName, string Artista, SongBD db, IWebHostEnvironment env) =>
             {
                 // 1. Buscar la canción y obtener todos los metadatos y rutas
                 var song = await db.Song
                     .Where(s => s.Name.ToLower() == songName.ToLower() && s.Artist.ToLower() == Artista.ToLower())
                     .FirstOrDefaultAsync(); // Traemos el objeto completo

                 if (song == null)
                 {
                     return Results.NotFound(new { message = $"No se encontró la canción '{songName}'." });
                 }

                 // 2. Definir las rutas físicas

                 var mp3Path = Path.Combine(env.WebRootPath, song.RutaArchivo.TrimStart('/'));
                 var imgPath = song.ImageRuta != null ? Path.Combine(env.WebRootPath, song.ImageRuta.TrimStart('/')) : null;

                 // 3. Crear el Stream de Memoria para el Archivo ZIP
                 using (var memoryStream = new MemoryStream())
                 {
                     // Usamos un ZipArchive para construir el archivo ZIP en el MemoryStream
                     using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                     {
                         // === A. Agregar el MP3 (Obligatorio) ===
                         if (!File.Exists(mp3Path))
                             return Results.Problem("El archivo MP3 no se encontró en el disco.", statusCode: 404);

                         var mp3Entry = zipArchive.CreateEntry($"{song.Name}.mp3");
                         using (var entryStream = mp3Entry.Open())
                         using (var fileStream = new FileStream(mp3Path, FileMode.Open, FileAccess.Read))
                         {
                             await fileStream.CopyToAsync(entryStream);
                         }

                         // === B. Agregar la Imagen (Opcional) ===
                         if (imgPath != null && File.Exists(imgPath))
                         {
                             var imgExtension = Path.GetExtension(imgPath);
                             var imgEntry = zipArchive.CreateEntry($"{song.Name}-Portada{imgExtension}");
                             using (var entryStream = imgEntry.Open())
                             using (var fileStream = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
                             {
                                 await fileStream.CopyToAsync(entryStream);
                             }
                         }

                         // === C. Agregar los Metadatos (JSON) ===
                         // Creamos un DTO ligero para los metadatos sin las rutas de disco.
                         var metadata = new DTO
                         {
                             Title = song.Name,
                             Artist = song.Artist,

                             // Se podría incluir la URL para S3 o Azure si se usaran
                         };
                         var jsonString = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });

                         var jsonEntry = zipArchive.CreateEntry("metadata.json");
                         using (var entryStream = jsonEntry.Open())
                         using (var streamWriter = new StreamWriter(entryStream))
                         {
                             await streamWriter.WriteAsync(jsonString);
                         }
                     }

                     // 4. Devolver el Archivo ZIP
                     // Es crucial hacer un rewind al stream antes de devolverlo
                     memoryStream.Seek(0, SeekOrigin.Begin);

                     return Results.File(
                         memoryStream.ToArray(), // Convierte el MemoryStream a byte[]
                         contentType: "application/zip",
                         fileDownloadName: $"{song.Name} - Paquete.zip"
                     );
                 }
             });
              app.MapGet("/DescargarAlbum/v1/{AlbumName}/{Artista}", async (string AlbumName, string Artista, SongBD db, IWebHostEnvironment env) =>
             {
                 // 1. Buscar la canción y obtener todos los metadatos y rutas
                 var Albums = await db.Album.Where(a => a.Title == AlbumName && a.Artista == Artista)
                 .Include(a => a.Songs).Select(a => new { a.Songs, a.Title, a.ReleaseYear }).FirstOrDefaultAsync();
                 if (Albums == null)
                 {
                     return Results.NotFound(new { message = $"No se encontró la canción '{AlbumName}'." });
                 }

                 // 2. Definir las rutas físicas



                 // 3. Crear el Stream de Memoria para el Archivo ZIP
                 using (var memoryStream = new MemoryStream())
                 {
                     using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                     {
                         foreach (var song in Albums.Songs)
                         {
                             var mp3Path = Path.Combine(env.WebRootPath, song.RutaArchivo.TrimStart('/'));
                             var imgPath = song.ImageRuta != null ? Path.Combine(env.WebRootPath, song.ImageRuta.TrimStart('/')) : null;
                             // === A. Agregar el MP3 (Obligatorio) ===
                             if (!File.Exists(mp3Path))
                                 return Results.Problem("El archivo MP3 no se encontró en el disco.", statusCode: 404);

                             var mp3Entry = zipArchive.CreateEntry($"{song.Name}.mp3");
                             using (var entryStream = mp3Entry.Open())
                             using (var fileStream = new FileStream(mp3Path, FileMode.Open, FileAccess.Read))
                             {
                                 await fileStream.CopyToAsync(entryStream);
                             }

                             // === B. Agregar la Imagen (Opcional) ===
                             if (imgPath != null && File.Exists(imgPath))
                             {
                                 var imgExtension = Path.GetExtension(imgPath);
                                 var imgEntry = zipArchive.CreateEntry($"{song.Name}-Portada{imgExtension}");
                                 using (var entryStream = imgEntry.Open())
                                 using (var fileStream = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
                                 {
                                     await fileStream.CopyToAsync(entryStream);
                                 }
                             }

                             // === C. Agregar los Metadatos (JSON) ===
                             // Creamos un DTO ligero para los metadatos sin las rutas de disco.
                             var metadata = new
                             {
                                 Title = song.Name,
                                 año = Albums.ReleaseYear,
                                 Artist = song.Artist,
                                 Description = song.Description,
                                 Lanzamiento = Albums.Title

                                 // Se podría incluir la URL para S3 o Azure si se usaran
                             };
                             var jsonString = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });

                             var jsonEntry = zipArchive.CreateEntry("metadata.json");
                             using (var entryStream = jsonEntry.Open())
                             using (var streamWriter = new StreamWriter(entryStream))
                             {
                                 await streamWriter.WriteAsync(jsonString);
                             }
                         }
                     }

                     // 4. Devolver el Archivo ZIP
                     // Es crucial hacer un rewind al stream antes de devolverlo
                     memoryStream.Seek(0, SeekOrigin.Begin);
                 
                 return Results.File(
                     memoryStream.ToArray(), // Convierte el MemoryStream a byte[]
                     contentType: "application/zip",
                     fileDownloadName: $"{AlbumName} - Paquete.zip"
                 );
             }
             });
    }
}