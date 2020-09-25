using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using WebYoutubeDownload.Service;

namespace WebYoutubeDownload
{
    [Route("download-file")]
    public class DownloadFileController : Controller
    {
        private readonly DownloadedCache _downloadedCache;

        public DownloadFileController(DownloadedCache downloadedCache)
        {
            _downloadedCache = downloadedCache;
        }

        [HttpPost]
        public IActionResult Index1([FromForm] string file)
        {
            string filePath = $"download/{file}";
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { file = $"Not found {file}" });
            }
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            _ = _downloadedCache.Add(filePath);
            string fileName = file;
            if(fileName.Length > 54) 
            {
                //4 extension charater

                fileName = file.Substring(0, 50) + System.IO.Path.GetExtension(file);
            }

            contentType = "application/octet-stream";
            return File(fileStream, contentType, fileName);
        }
    }
}
