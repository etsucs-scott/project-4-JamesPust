using SolitaireGame.Components;
using SolitaireGame.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Game services
builder.Services.AddScoped<GameEngine>();       // one game per browser session
builder.Services.AddSingleton<SaveGameService>(); // shared file access

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
