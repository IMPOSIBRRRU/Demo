namespace Demo.Middlewares
{
    public class UploadMiddleware
    {
        private readonly RequestDelegate next;

        public UploadMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var responce = context.Response;
            var request = context.Request;

            responce.ContentType = "text/html; charset=utf-8";

            IFormFileCollection files = request.Form.Files;

            var uploadPath = $"{Directory.GetCurrentDirectory()}/uploads";

            Directory.CreateDirectory(uploadPath);

            foreach (var file in files)
            {
                string fullPath = $"{uploadPath}/{file.FileName}";

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            await responce.WriteAsync("Uploaded");
        }
    }
}
