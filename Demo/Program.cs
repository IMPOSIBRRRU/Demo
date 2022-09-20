using Demo.Middlewares;

//List of persons
List<Person> users = new List<Person>
{
    new() {Id = Guid.NewGuid().ToString(), Name = "Tolya", Age = 10},
    new() {Id = Guid.NewGuid().ToString(), Name = "Mike", Age = 51},
    new() {Id = Guid.NewGuid().ToString(), Name = "Sam", Age = 35},
};

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

//middleware pipline
//error handler
app.UseMiddleware<ErrorHandlingMiddleware>();

//thow exception if token = 1234567890. like primitive authentication, but reversed
app.UseMiddleware<TokenMiddleware>();

//api
app.UseMiddleware<ApiMiddleware>();

app.UseWhen(context => context.Request.Path == "/upload" && context.Request.Method == "POST",
     appBuilder => appBuilder.UseMiddleware<UploadMiddleware>());

//map to upload if method is not POST
app.Map("/upload", appBuilder =>
{
    appBuilder.Run(async context => 
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.SendFileAsync("html/upload.html");
    });
});

app.Run();