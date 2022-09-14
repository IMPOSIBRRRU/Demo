namespace Demo.Middlewares
{
    public class _template
    {
        private readonly RequestDelegate next;

        public _template(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await next.Invoke(context);
        }
    }
}
