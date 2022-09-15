namespace Demo.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await next.Invoke(context);
            if (context.Response.StatusCode == 403)
            {
                await context.Response.WriteAsync("<h1>403</h1> <h2>No access?</h2>");
            }
            else if(context.Response.StatusCode == 404)
            {
                await context.Response.WriteAsync($"<h1>404</h1> <h2>Nothing here...</h2> {context.Request.Path}<br/>");
            }
        }
    }
}