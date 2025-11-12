using Microsoft.EntityFrameworkCore; //
namespace YourProject.Endpoints;
public static class ConsultarEndpoints
{
    // El método de extensión toma 'this WebApplication app' como parámetro
    public static void Consultar(this WebApplication app)
    {
         app.MapGet("/ConsultarCatalogo/v1/Album/{Name}/{Artist}", async (string Name, string? Artist, SongBD db) =>
        {
            if (Artist == null || Artist.Length == 0|| Artist == "NONE")
            {
                var AlbumNA = await db.Album.Where(a => a.Title.ToLower() == Name.ToLower())
               .Include(a => a.Songs).Select(a => new { a.Title, a.Artista,canciones = a.Songs.Select(s => new
               {
                   s.Name,
                   s.Description
               }) })
               .ToListAsync();
                return Results.Ok(AlbumNA);
            }
            return Results.NotFound(new { Album = $"No encontrado" });
        });
        app.MapGet("/ConsultarCatalogo/v1/{Name}/{Artist}", async (string? Name, string? Artist, SongBD db) =>
     {

         if (Name != null && Artist != null && Name != "" && Artist != "")
         {
             var songSelect = await db.Song.Where(s => s.Name.ToLower() == Name.ToLower() && s.Artist.ToLower() == Artist.ToLower())
             .Select(s => s.Id).FirstOrDefaultAsync();
             if (songSelect != 0)
             {
                 var resultado = new
                 {
                     Status = "Existe la cancion " + Name + " Del Artista :" + Artist
                 };
                 return Results.Ok(resultado);
             }
             else
             {
                 return Results.NotFound(new { message = $"No se ha encontrado la cancion" });
             }
         }
         return Results.NotFound(new { message = $"Datos ingresados de forma incorrecta" });
     });
        app.MapGet("/ConsultarCatalogo/v1", async (SongBD db) =>
             {
                 var song = await db.Song.Select(s => new
                 {
                     Nombre = s.Name,
                     Artista = s.Artist,
                     Album = s.Album!.Title ?? "Ninguno"
                 })
        .ToListAsync();
                 if (song.Count != 0)
                 {
                     return Results.Ok(song);
                 }
                 return Results.NotFound(new { mesaje = $"Actualmente el listado se encuentra vacio :)" });
             });
    }
}