using Microsoft.EntityFrameworkCore;
using working_with_timescale_data.Models.DbContextDir;

namespace working_with_timescale_data;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();
        var connect = builder.Configuration.GetConnectionString("TimescaleDb");
        builder.Services.AddDbContext<TimescaleDb>(c=>c.UseNpgsql(connect??throw new NullReferenceException()));


        var app = builder.Build();
        app.UseCors(c =>
        {
            c.AllowAnyOrigin();
            c.AllowAnyMethod();  
            c.AllowAnyHeader();
        });
        if(!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
