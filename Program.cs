using ApiPlaylistDeMusicas.models;
using Microsoft.EntityFrameworkCore;
using ApiPlaylistDeMusicas.Data;
using ApiPlaylistDeMusicas.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite("Data Source=playlist.db"));

var app = builder.Build();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            mensagem = "Ocorreu um erro interno. Tente novamente mais tarde."
        });
    });
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    db.Database.EnsureCreated();
}

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapPost("/api/musicas", async (MusicaEntradaDto dados, ApiDbContext db) =>
{
    var novaMusica = new MusicaEntity { Titulo = dados.Titulo, Artista = dados.Artista };
    db.Musicas.Add(novaMusica);
    await db.SaveChangesAsync();
    return Results.Created($"/api/musicas/{novaMusica.Id}", novaMusica);
});

app.MapGet("/api/musicas", async (ApiDbContext db) =>
    await db.Musicas.ToListAsync());

app.MapGet("/api/musicas/{id:int}", async (int id, ApiDbContext db) =>
{
    var musica = await db.Musicas.FindAsync(id);
    if (musica is null) return Results.NotFound();
    return Results.Ok(musica);
});

app.MapPut("/api/musicas/{id:int}", async (int id, MusicaEntradaDto dados, ApiDbContext db) => 
{
    var musica = await db.Musicas.FindAsync(id);
    if (musica is null) return Results.NotFound();

    musica.Titulo = dados.Titulo;
    musica.Artista = dados.Artista;
    await db.SaveChangesAsync();

    return Results.Ok(musica);
});

app.MapDelete("/api/musicas/{id:int}", async (int id, ApiDbContext db) => 
{
    var musica = await db.Musicas.FindAsync(id);
    if (musica is null) return Results.NotFound();

    db.Musicas.Remove(musica);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

