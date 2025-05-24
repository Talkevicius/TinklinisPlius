using TinklinisPlius.Models;
using TinklinisPlius.Services.Match;
using TinklinisPlius.Services.Player;
using TinklinisPlius.Services.Team;
using TinklinisPlius.Services.Tournament;
using TinklinisPlius.Services.Participate;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// DB SERVICES FOR TOURNAMENT/MATCH
// DB SERVICES FOR TOURNAMENT/MATCH
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IParticipateService, ParticipateService>();
// DB SERVICES FOR TEAM
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

builder.Services.AddDbContext<AppDbContext>();
// Tell Razor where to find views in custom folder
builder.Services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("/FrontEnd/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/FrontEnd/Views/Shared/{0}.cshtml");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

