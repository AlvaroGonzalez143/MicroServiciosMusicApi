
using System.ComponentModel.DataAnnotations;

public class Album
{
    // Clave Primaria (PK)
    public int Id { get; set; }
    
    // Propiedades del Álbum
    [Required]
    public string Title { get; set; }
    public string Artista { get; set; }
    public int ReleaseYear { get; set; }
    public ICollection<Song> Songs { get; set; } = new List<Song>();
}