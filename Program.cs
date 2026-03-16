using Amazon.DynamoDBv2;
using TodoMvcNet8_Final.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<TodoRepository>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://cognito-idp.eu-north-1.amazonaws.com/eu-north-1_ujDjhbnra";

    options.ClientId = "2b6vec4kliejhthfp4878t9d2i";
    options.ClientSecret = "q5nm8e8gg0q7j1b8q19qst2n74fb9pcqdjomg2d3mdva2ck3548";
    options.ResponseType = "code";
    options.CallbackPath = "/signin-oidc";
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("email");

    options.SaveTokens = true;
});


var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();    
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Todo}/{action=Index}/{id?}");
});
app.Run();
