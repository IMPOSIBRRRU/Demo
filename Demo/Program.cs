WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async(context) =>
{
    var response = context.Response;
    var request = context.Request;
    var fullPath = $"html/{request.Path}";
    var now = DateTime.Now;

    response.ContentType = "text/html; charset=utf-8";

    if (File.Exists(fullPath))
    {
        await response.SendFileAsync(fullPath);
    }
    else if (request.Path == "/")
    {
        await response.WriteAsync("<h1>SUP!</h1>");
    }
    else if (request.Path == "/headers")
    {

        var stringBuilder = new System.Text.StringBuilder("<h1>Headers</h1><table>");

        foreach (var header in request.Headers)
        {
            stringBuilder.Append($"<tr><td>{header.Key}</td><td>{header.Value}</td></tr>");
        }
        stringBuilder.Append("</table>");

        await response.WriteAsync(stringBuilder.ToString());
    }
    else if (request.Path.StartsWithSegments("/query"))
    {

        var stringBuilder = new System.Text.StringBuilder("<h1>Query</h1><table>");

        foreach (var param in request.Query)
        {
            stringBuilder.Append($"<tr><td>{param.Key}</td><td>-</td><td>{param.Value}</td></tr>");
        }
        stringBuilder.Append("</table>");

        await response.WriteAsync(stringBuilder.ToString());
    }
    else if(request.Path == "/date")
    {
        await response.WriteAsync($"Date: {now.ToShortDateString()}");
    }
    else if (request.Path == "/time")
    {
        await response.WriteAsync($"Date: {now.ToShortTimeString()}");
    }
    else if (request.Path == "/postuser")
    {
        var form = request.Form;
        string name = form["name"];
        string age = form["age"];
        string[] languages = form["languages"];
        string langList = "";
        foreach (var language in languages)
        {
            langList += $" {language}";
        }
        await response.WriteAsync($"<div><p>Name: {name}</p><p>Age: {age}</p><p>Languages:{langList}</p></div>");
    }
    else
    {
        await response.WriteAsync($"Path: {request.Path}");
    }
});

app.Run();
