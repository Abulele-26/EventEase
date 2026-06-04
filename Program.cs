using EventEase.Data;
using EventEase.Models;
using EventEase.Services;
using Microsoft.EntityFrameworkCore;

namespace EventEase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC
            builder.Services.AddControllersWithViews();

            // Blob Storage
            builder.Services.AddScoped<BlobService>();

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // SEED EVENT TYPES
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                context.Database.Migrate();

                if (!context.EventTypes.Any())
                {
                    context.EventTypes.AddRange(
                        new EventType { TypeName = "Wedding" },
                        new EventType { TypeName = "Conference" },
                        new EventType { TypeName = "Birthday" },
                        new EventType { TypeName = "Concert" },
                        new EventType { TypeName = "Corporate" }
                    );

                    context.SaveChanges();
                }
            }

            // HTTP pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();   // IMPORTANT (replaces MapStaticAssets)
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}