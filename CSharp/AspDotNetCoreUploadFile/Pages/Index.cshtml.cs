using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspDotNetCoreUploadFile.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHostEnvironment _environment;

        public string UploadedImage { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnGet()
        {
        }

        public async Task OnPostAsync(IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                return;
            }

            _logger.LogInformation($"Uploading {uploadedFile.FileName}.");
            string targetFileName = $"{_environment.ContentRootPath}/wwwroot/{uploadedFile.FileName}";

            using (var stream = new FileStream(targetFileName, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(stream);
            }

            if (uploadedFile.ContentType.IndexOf("image", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                UploadedImage = $"/{uploadedFile.FileName}";
            }
        }
    }
}
