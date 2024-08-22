namespace FrontEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHttpsRedirection(); // Ensure HTTPS redirection in production
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // Add if authentication is needed
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
