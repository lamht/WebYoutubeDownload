using System;
using System.Collections.Concurrent;
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
        private static ConcurrentDictionary<string, string> dataResult = new ConcurrentDictionary<string, string>();
        private readonly ILogger<IndexModel> _logger;
        public string YoutubeLink { get; private set; }
        public string Uuid { get; private set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnPost(string url, string option, string customParams)
        {
            Uuid = System.Guid.NewGuid().ToString();
            YoutubeLink = url;
            // Define the cancellation token.
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            Task.Factory.StartNew(() => Download(Uuid, url, option, customParams),
            token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        public JsonResult OnGetDownloadProgess(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
            {
                return new JsonResult(new { code = -1, message = "Error" });
            }
            if (dataResult.TryGetValue(uuid, out string result))
            {
                Task.Factory.StartNew(() => {
                    Task.Delay(10000);
                    dataResult.TryRemove(uuid, out string d);
                });

                return new JsonResult(new { code = 2, message = "Finished", result = result });
            }
            else
            {
                return new JsonResult(new { code = 1, message = "Progresing" });
            }
        }

        private string Download(string uuid, string url, string option, string customParams)
        {
            string result;
            try
            {
                string downloadPath = Constants.DOWNLOAD_FOLDER;
                if(!System.IO.Directory.Exists(downloadPath)){
                    System.IO.Directory.CreateDirectory(downloadPath);
                }
                string cmd = $"cd {downloadPath} && youtube-dl --verbose -f 140 {url}";
                option = option?.ToLower();
                if ("audio-m4a".Equals(option))
                {
                    cmd = $"cd {downloadPath} && youtube-dl --verbose -f 140 {url}";
                }
                else if ("audio-opus".Equals(option))
                {
                    cmd = $"cd {downloadPath} && youtube-dl --verbose -f 251 {url}";
                }
                else if ("video".Equals(option))
                {
                    cmd = $"cd {downloadPath} && youtube-dl --verbose -f '136+140' --merge-output-format mp4 {url}";
                }
                else if ("video1".Equals(option))
                {
                    cmd = $"cd {downloadPath} && youtube-dl --verbose -f '137+140' --merge-output-format mp4 {url}";
                }
                else if ("custom".Equals(option?.ToLower()))
                {
                    cmd = $"cd {downloadPath} && youtube-dl --verbose -f '{customParams}' --merge-output-format mp4 {url}";
                }
                _logger.LogInformation(cmd);
                result = cmd.Bash();
                _logger.LogInformation(result);
            }
            catch (Exception e)
            {
                string cmd = $"youtube-dl -F {url}";
                string listFormat = cmd.Bash();
                result = $"{e.StackTrace} </br> {listFormat}";
                _logger.LogError(e, "Error download youtube");
            }
            dataResult.TryAdd(uuid, result);
            return result;
        }
    }
}
