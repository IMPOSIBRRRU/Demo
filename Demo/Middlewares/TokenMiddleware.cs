namespace Demo.Middlewares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate next;

        public TokenMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Query["token"];
            if (token == "1234567890")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Invalid token!");
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}
