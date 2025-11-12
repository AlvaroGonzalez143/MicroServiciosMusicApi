
using System.ComponentModel.DataAnnotations.Schema;
public class Song
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Lyrics { get; set; }
    public string Artist { get; set; }
    public string? ImageRuta { get; set; }
    public string RutaArchivo { get; set; }
    
    public int? AlbumId { get; set; }
    [NotMapped]
    public IFormFile ArchivoMp3 { get; set; }
    [NotMapped]
    public IFormFile image { get; set; }

    public Album? Album { get; set; }
}

