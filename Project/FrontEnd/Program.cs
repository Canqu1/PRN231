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
            builder.Services.AddAuthentication("CookieAuthentication")
           .AddCookie("CookieAuthentication", options =>
           {
               options.LoginPath = "/Login";
               //options.AccessDeniedPath = "/Forbidden";
               options.ExpireTimeSpan = TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(50));
           });
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20); // Thời gian hết hạn
                options.Cookie.HttpOnly = true; // Cookie chỉ có thể được truy cập qua HTTP
                options.Cookie.IsEssential = true; // Cookie là cần thiết cho phiên làm việc
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}