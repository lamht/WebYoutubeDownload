using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebYoutubeDownload.Exetension;

namespace WebYoutubeDownload.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string Url { get; private set; }
        public string Output { get; private set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnPost(string url, string option)
        {
            Url = url;
            string cmd = $"cd download && youtube-dl --verbose -f 140 {url}";
            if ("video".Equals(option?.ToLower()))
            {
                cmd = $"cd download && youtube-dl --verbose -f '136+140' --merge-output-format mp4 {url}";
            }
            _logger.LogInformation(cmd);
             Output = cmd.Bash();
            _logger.LogInformation(Output);
        }
    }
}
