using System.Text.RegularExpressions;

namespace Demo.Middlewares
{
    public class Person
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int Age { get; set; }
    };

    public class ApiMiddleware
    {
        private readonly RequestDelegate next;

        public ApiMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;
            var request = context.Request;
            var path = request.Path;
            //string expressionForNumber = "/api/users/([0 - 9]+)$";
            string expressionForGuid = @"^/api/users/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";

            if (path == "/api/users" && request.Method == "GET")
            {
                await GetAllPeople(response);
            }
            else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
            {
                string? id = path.Value?.Split("/")[3];
                await GetPerson(id, response);
            }
            else if (path == "/api/users" && request.Method == "POST")
            {
                await CreatePerson(response, request);
            }
            else if (path == "/api/users" && request.Method == "PUT")
            {
                await UpdatePerson(response, request);
            }
            else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "DELETE")
            {
                string? id = path.Value?.Split("/")[3];
                await DeletePerson(id, response);
            }
            else
            {
                await next.Invoke(context);
            }

        }

        async Task GetAllPeople(HttpResponse response)
        {
            await response.WriteAsJsonAsync(users);
        }

        async Task GetPerson(string? id, HttpResponse response)
        {
            Person? user = users.FirstOrDefault((u) => u.Id == id);
            if (user != null)
            {
                await response.WriteAsJsonAsync(user);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "No such user" });
            }
        }

        async Task CreatePerson(HttpResponse response, HttpRequest request)
        {
            try
            {
                var user = await request.ReadFromJsonAsync<Person>();
                if (user != null)
                {
                    user.Id = Guid.NewGuid().ToString();
                    users.Add(user);
                    await response.WriteAsJsonAsync(user);
                }
                else
                {
                    throw new Exception("Wrong data");
                }
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Wrong data" });
            }
        }

        async Task DeletePerson(string? id, HttpResponse response)
        {
            Person? user = users.FirstOrDefault((u) => u.Id == id);

            if (user != null)
            {
                users.Remove(user);
                await response.WriteAsJsonAsync(user);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "No such user" });
            }
        }
        async Task UpdatePerson(HttpResponse response, HttpRequest request)
        {
            try
            {
                Person? userData = await request.ReadFromJsonAsync<Person>();
                if (userData != null)
                {
                    var user = users.FirstOrDefault((u) => u.Id == userData.Id);
                    if (user != null)
                    {
                        user.Age = userData.Age;
                        user.Name = userData.Name;
                        await response.WriteAsJsonAsync(user);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        await response.WriteAsJsonAsync(new { message = "No such user" });
                    }
                }
                else
                {
                    throw new Exception("Wrong data");
                }
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Wrong data" });
            }
        }


        //List of persons
        List<Person> users = new List<Person>
        {
            new() {Id = Guid.NewGuid().ToString(), Name = "Tolya", Age = 10},
            new() {Id = Guid.NewGuid().ToString(), Name = "Mike", Age = 51},
            new() {Id = Guid.NewGuid().ToString(), Name = "Sam", Age = 35},
        };
    }
}
