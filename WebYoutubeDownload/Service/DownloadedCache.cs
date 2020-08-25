using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebYoutubeDownload.Service
{
    public class DownloadedCache
    {
        private HashSet<string> downloadedFiles;

        public DownloadedCache()
        {
            downloadedFiles = new HashSet<string>();
        }

        public async Task Add(string file)
        {
            await Task.Delay(4 * 60 * 1000);
            downloadedFiles.Add(file);
        }

        public HashSet<string> GetAndReset()
        {
            var data = new HashSet<string>(downloadedFiles);
            downloadedFiles = new HashSet<string>();
            return data;
        }
    }
}
