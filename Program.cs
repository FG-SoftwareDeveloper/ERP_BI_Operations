using ERP_BI_Operations.Data;
using ERP_BI_Operations.Models; // Your custom models namespace
using Microsoft.AspNetCore.Identity; // Add this at the top
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// ? Business data
builder.Services.AddDbContext<ERP_BIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? Identity data
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>() // <--- use ApplicationDbContext here
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ERP BI API v1"));
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // ? Add this if using Identity
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
