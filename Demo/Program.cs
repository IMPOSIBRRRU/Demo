using System.Text.Json;
using System.Text.Json.Serialization;

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
    else if (request.Path == "/api/user") //api user json hell
    {

        var responseText = "wrong data";
        if (request.HasJsonContentType())
        {
            var jsonOption = new JsonSerializerOptions();
            jsonOption.Converters.Add(new PersonConverter());
            var person = await request.ReadFromJsonAsync<Person>(jsonOption);
            if (person != null)
            {
                responseText = $"Name: {person.Name} Age: {person.Age}";
            }
        }
        await response.WriteAsJsonAsync(new { text = responseText });
    }
    else //shows path if nothing matches
    {
        await response.WriteAsync($"Path: {request.Path}");
    }
});

app.Run();

public record Person(string Name, int Age);
public class PersonConverter : JsonConverter<Person>
{
    public override Person Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var personName = "Who?";
        var personAge = 0;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                switch (propertyName?.ToLower())
                {
                    case "age" when reader.TokenType == JsonTokenType.Number:
                        personAge = reader.GetInt32();
                        break;

                    case "age" when reader.TokenType == JsonTokenType.String:
                        string? stringValue = reader.GetString();
                        if (int.TryParse(stringValue, out int value))
                        {
                            personAge = value;
                        }
                        break;

                    case "name":
                        string? name = reader.GetString();
                        if (name != null && name != "")
                        {
                            personName = reader.GetString();
                        }
                        break;
                }
            }
        }
        return new Person(personName, personAge);
    }

    public override void Write(Utf8JsonWriter writer, Person person, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("name", person.Name);
        writer.WriteNumber("age", person.Age);

        writer.WriteEndObject();
    }
}