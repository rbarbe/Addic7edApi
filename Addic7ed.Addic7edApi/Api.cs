using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Addic7ed.Addic7edApi.Models;

namespace Addic7ed.Addic7edApi
{
    public class Api
    {
        private readonly HttpClient _httpClient;

        public Api()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<TvShow>> GetShows()
        {
            var html = await _httpClient.GetShows();
            return await new Parser().GetShows(html);
        }

        public async Task<int> GetNumberOfSeasons(int addictedShowId)
        {
            var html = await _httpClient.GetNumberOfSeasons(addictedShowId);
            return await new Parser().GetNumberOfSeasons(html);
        }

        public async Task<DownloadSubtitleResult> DownloadSubtitle(int addictedShowId, Uri downloadUri)
        {
            var httpContent = await _httpClient.DownloadSubtitle(addictedShowId, downloadUri);

            var downloadSubtitleResult = new DownloadSubtitleResult
            {
                Stream = await httpContent.ReadAsStreamAsync(),
                Filename = httpContent.Headers.ContentDisposition.FileName.Trim('"'),
                Mediatype = httpContent.Headers.ContentType.MediaType
            };

            return downloadSubtitleResult;
        }

        public async Task<Episode> GetEpisodeSubtitles(int addictedShowId, int seasonNumber, int episodeNumber)
        {
            var episodes = await GetSeasonSubtitles(addictedShowId, seasonNumber);
            return episodes.FirstOrDefault(e => e.Number == episodeNumber);
        }

        public async Task<List<Episode>> GetSeasonSubtitles(int showId, int seasonNumber)
        {
            var html = await _httpClient.GetSeasonSubtitles(showId, seasonNumber);
            return await new Parser().GetSeasonSubtitles(html);
        }
    }
}