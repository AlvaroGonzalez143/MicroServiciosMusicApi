using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc; // Requerido para [FromForm]
using Microsoft.EntityFrameworkCore; //
namespace YourProject.Endpoints;
public static class UserEndpoints
{
    // El método de extensión toma 'this WebApplication app' como parámetro
    public static void Crear(this WebApplication app)
    {
        app.MapPost("/AgregarCancion/v1", async ([FromForm]Album? Album,[FromForm]Song song, SongBD db, IWebHostEnvironment env) =>
{


    var uploadsPath = Path.Combine(env.WebRootPath, "media");
    var archivoMp3 = song.ArchivoMp3;
    var Image = song.image;


    if (string.IsNullOrWhiteSpace(song.Name) || archivoMp3 == null || archivoMp3.Length == 0)
    {
        return Results.BadRequest(new { message = "El nombre y el archivo MP3 son obligatorios." });
    }
   
    bool existeCancion = await db.Song
         .AnyAsync(s =>
              s.Name.ToLower() == song.Name.ToLower()
           && s.Artist.ToLower() == song.Artist.ToLower()
         );
    if (existeCancion)
    {
        return Results.Conflict(new { message = $"Ya existe una canción con el nombre '{song.Name}'." });
    }
    if (Album?.Artista != null&& Album?.Title != null)
    {
         bool existeAlbum = await db.Album
                .AnyAsync(a =>
                     a.Title.ToLower() == Album.Title.ToLower()
                  && a.Artista.ToLower() == Album.Artista.ToLower()
                );
        if (existeAlbum)
        {
            int id = await db.Album.Where(a => a.Title.ToLower() == Album.Title.ToLower() &&
             a.Artista.ToLower() == Album.Artista.ToLower())
        .Select(a => a.Id)
        .FirstOrDefaultAsync();
              song.AlbumId = id;
        }
        else
        {
        db.Album.Add(Album);
        await db.SaveChangesAsync();
        song.Album = Album;
        song.AlbumId = Album.Id; 
        }
    }
    Directory.CreateDirectory(uploadsPath);

    var extension = Path.GetExtension(archivoMp3.FileName);
    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
    var filePath = Path.Combine(uploadsPath, uniqueFileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await archivoMp3.CopyToAsync(stream);
    }
    //image manejo 
    if (Image != null && Image.Length != 0) {
        var imgExtension = Path.GetExtension(Image.FileName);
        var imgUniqueFileName = $"{Guid.NewGuid()}{imgExtension}";
        var imgFilePath = Path.Combine(uploadsPath, imgUniqueFileName);

        using (var stream = new FileStream(imgFilePath, FileMode.Create))
        {
            await Image.CopyToAsync(stream);
        }
        //imagen manejo
        song.ImageRuta = $"/media/{imgUniqueFileName}";
    }
    // 3. PERSISTENCIA: Establecer la RutaArchivo antes de guardar
    song.RutaArchivo = $"/media/{uniqueFileName}";

    // El campo ArchivoMp3 (IFormFile) será ignorado por EF Core si usaste [NotMapped]
    db.Song.Add(song);
    await db.SaveChangesAsync();
    if (Album?.Title != null)
    {
        var DATA = new
        {
            Album = Album.Title,
            Artista = song.Artist,
        };
        return Results.Created($"/AgregarCancion{song.Id}", DATA);
    }
    else
    {
         var DATA = new
         {
             Title = song.Name,
             Artista = song.Artist,
            Album = "Ninguno"
         };
         return Results.Created($"/AgregarCancion{song.Id}", DATA);
    }
}).DisableAntiforgery();
    }
}