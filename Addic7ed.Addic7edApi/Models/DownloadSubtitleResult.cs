using System.IO;

namespace Addic7ed.Addic7edApi.Models
{
    public class DownloadSubtitleResult
    {
        public string Filename { get; set; }
        public Stream Stream { get; set; }
        public string Mediatype { get; set; }
    }
}