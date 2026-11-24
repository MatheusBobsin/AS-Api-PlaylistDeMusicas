namespace ApiPlaylistDeMusicas.Data;
using Microsoft.EntityFrameworkCore;
using ApiPlaylistDeMusicas.models;

class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }
    public DbSet<MusicaEntity> Musicas => Set<MusicaEntity>();
}