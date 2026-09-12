using BoutiqueBeads.Data;
using BoutiqueBeads.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---- Database ----
// TEMPORARY: running on an in-memory database seeded with dummy data so the site
// works with zero setup. Nothing persists between app restarts.
// To reconnect a real database, comment the line below back out and uncomment
// the UseSqlServer line (and re-run `dotnet ef database update`).
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("BoutiqueBeadsDummyDb"));

// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---- Identity: powers the Admin login only (no public customer accounts needed for v1) ----
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ---- Blob Storage for product/tutorial images & videos ----
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

// ---- Razor Pages, with the Admin area locked to the "Admin" role ----
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("Admin", "/", "AdminOnly");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// TEMPORARY: creates the in-memory schema and drops in dummy products/tutorials
// every time the app starts. Remove this block once a real database is wired up.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    BoutiqueBeads.Data.SeedData.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
