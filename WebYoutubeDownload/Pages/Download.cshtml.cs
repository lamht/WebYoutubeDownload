using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebYoutubeDownload.Pages
{
    public class DownloadModel : PageModel
    {
        private readonly ILogger<DownloadModel> _logger;
        public List<FileInfo> Files { get; private set; }
        public DownloadModel(ILogger<DownloadModel> logger)
        {
            _logger = logger;        

        }
        public void OnGet()
        {
            if (!System.IO.Directory.Exists(Constants.DOWNLOAD_FOLDER))
            {
                System.IO.Directory.CreateDirectory(Constants.DOWNLOAD_FOLDER);
            }
            Files = Directory.GetFiles(Constants.DOWNLOAD_FOLDER).Select(f => new FileInfo(f)).ToList();
        }
    }
}
