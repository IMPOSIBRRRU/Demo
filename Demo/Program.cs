WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async(context) =>
{
    var response = context.Response;
    var request = context.Request;
    var fullPath = $"html/{request.Path}";
    var now = DateTime.Now;

    response.ContentType = "text/html; charset=utf-8";

    if (File.Exists(fullPath)) //open html file by path
    {
        await response.SendFileAsync(fullPath);
    }
    else if (request.Path == "/") //kinda index page, but it's not
    {
        await response.WriteAsync("<h1>SUP!</h1>");
    }
    else if (request.Path == "/headers") //shows headers as table
    {

        var stringBuilder = new System.Text.StringBuilder("<h1>Headers</h1><table>");

        foreach (var header in request.Headers)
        {
            stringBuilder.Append($"<tr><td>{header.Key}</td><td>{header.Value}</td></tr>");
        }
        stringBuilder.Append("</table>");

        await response.WriteAsync(stringBuilder.ToString());
    }
    else if (request.Path.StartsWithSegments("/query")) //shows query as table 
    {

        var stringBuilder = new System.Text.StringBuilder("<h1>Query</h1><table>");

        foreach (var param in request.Query)
        {
            stringBuilder.Append($"<tr><td>{param.Key}</td><td>-</td><td>{param.Value}</td></tr>");
        }
        stringBuilder.Append("</table>");

        await response.WriteAsync(stringBuilder.ToString());
    }
    else if(request.Path == "/date") //shows date
    {
        await response.WriteAsync($"Date: {now.ToShortDateString()}");
    }
    else if (request.Path == "/time") //shows time
    {
        await response.WriteAsync($"Date: {now.ToShortTimeString()}");
    }
    else if (request.Path == "/postuser") //shows data from form.html
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
    else if (request.Path == "/old") //open "old page"
    {
        //await response.WriteAsync("<h2>Old page</h2>");
        response.Redirect("/new"); //but it redirect to "new page"
    }
    else if (request.Path == "/new") //new page
    {
        await response.WriteAsync($"<h2>New page</h2>");
    }
    else //shows path if nothing matches
    {
        await response.WriteAsync($"Path: {request.Path}");
    }
});

app.Run();