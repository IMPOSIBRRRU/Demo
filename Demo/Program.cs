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

//middleware pipline
//token exception
app.UseMiddleware<TokenMiddleware>();

//api
app.UseMiddleware<ApiMiddleware>();

app.UseWhen(context => context.Request.Path == "/upload" && context.Request.Method == "POST",
    appBuilder => appBuilder.UseMiddleware<UploadMiddleware>());

//simple mapping
app.Run(async(context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    string path = context.Request.Path;
    string fullPath = $"html/{path}";
    if (File.Exists(fullPath))
    {
        await context.Response.SendFileAsync(fullPath);
    }
    else if (fullPath == "html//")
    {
        await context.Response.SendFileAsync("html/index.html");
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync($"<h2>Not found</h2> {fullPath}<br/>");
        await Task.Delay(3000);
        await context.Response.WriteAsync("lel");
    }

});

app.Run();