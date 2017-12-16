using System.Collections.Generic;

namespace Addic7ed.Addic7edApi.Models
{
    public class Episode
    {
        public Episode()
        {
            Subtitles = new List<Subtitle>();
        }

        public int Id { get; set; }
        public int Season { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public List<Subtitle> Subtitles { get; set; }
    }
}