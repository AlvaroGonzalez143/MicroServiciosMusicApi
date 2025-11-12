using Microsoft.EntityFrameworkCore;
public class SongBD : DbContext
{
 public SongBD(DbContextOptions<SongBD> options)
        : base(options) { }
public DbSet<Album> Album { get; set; }
    public DbSet<Song> Song => Set<Song>();
}