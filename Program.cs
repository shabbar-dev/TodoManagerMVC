var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<TodoMvcNet8_Final.Data.TodoRepository>();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
var app = builder.Build();

//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
//    c.RoutePrefix = string.Empty; // Makes Swagger the default page
//});
app.UseStaticFiles();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Todo}/{action=Index}/{id?}");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Todo}/{action=Index}/{id?}");
});
app.Run();
