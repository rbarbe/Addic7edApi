using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Addic7ed.Addic7edApi.Models;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;

namespace Addic7ed.Addic7edApi
{
    internal class Parser
    {
        public async Task<List<TvShow>> GetShows(string html)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(html);
            var selectShow = document.QuerySelector("#qsShow") as IHtmlSelectElement;

            var tvShows = new List<TvShow>();
            if (selectShow != null)
                tvShows.AddRange(selectShow.Options.Select(option => new TvShow
                {
                    Id = int.Parse(option.Value),
                    Name = option.Text
                }));
            return tvShows;
        }

        public async Task<int> GetNumberOfSeasons(string html)
        {
            var document = await new HtmlParser().ParseAsync(html);
            var selectSeason = document.QuerySelector("#qsiSeason") as IHtmlSelectElement;

            if (selectSeason?.Options?.Length > 0)
                return selectSeason.Options.Length - 1;
            return 0;
        }

        public async Task<List<Episode>> GetSeasonSubtitles(string html)
        {
            var episodes = new List<Episode>();
            var document = await new HtmlParser().ParseAsync(html);

            var table = document.QuerySelector("#season").QuerySelector("table") as IHtmlTableElement;

            var subtitlesRows = new List<SubtitleRow>();

            if (table != null)
                foreach (var row in table.Rows.Skip(1))
                    if (row.Cells.Length > 2)
                    {
                        var subtitleRow = new SubtitleRow();

                        for (var i = 0; i < row.Cells.Length; i++)
                            switch (i)
                            {
                                case 0:
                                    subtitleRow.Season = int.Parse(row.Cells[i].TextContent);
                                    break;
                                case 1:
                                    subtitleRow.Number = int.Parse(row.Cells[i].TextContent);
                                    break;
                                case 2:
                                    subtitleRow.Title = row.Cells[i].TextContent;
                                    break;
                                case 3:
                                    subtitleRow.Language = row.Cells[i].TextContent;
                                    break;
                                case 4:
                                    subtitleRow.Version = row.Cells[i].TextContent;
                                    break;
                                case 5:
                                    var state = row.Cells[i].TextContent;
                                    if (!state.Contains("%") && state.Contains("Completed"))
                                        subtitleRow.Completed = true;
                                    break;
                                case 6:
                                    subtitleRow.HearingImpaired = row.Cells[i].TextContent.Length > 0;
                                    break;
                                case 7:
                                    subtitleRow.Corrected = row.Cells[i].TextContent.Length > 0;
                                    break;
                                case 8:
                                    subtitleRow.HD = row.Cells[i].TextContent.Length > 0;
                                    break;
                                case 9:
                                    var downloadUri = row.Cells[i].FirstElementChild.Attributes["href"].Value;
                                    subtitleRow.EpisodeId =
                                        int.Parse(
                                            downloadUri.Replace("/updated/", "").Replace("/original/", "")
                                                .Split('/')[1]);
                                    subtitleRow.DownloadUri = new Uri(downloadUri, UriKind.Relative);
                                    break;
                            }
                        subtitlesRows.Add(subtitleRow);
                    }

            if (subtitlesRows.Any())
            {
                var episodeGroups = subtitlesRows.GroupBy(r => r.EpisodeId).ToList();
                foreach (var episodeGroup in episodeGroups)
                    if (episodeGroup.Any())
                    {
                        var episode = new Episode
                        {
                            Title = episodeGroup.First().Title,
                            Number = episodeGroup.First().Number,
                            Season = episodeGroup.First().Season,
                            Id = episodeGroup.First().EpisodeId
                        };

                        foreach (var subtitle in episodeGroup.Select(subtitleRow => new Subtitle
                        {
                            Version = subtitleRow.Version,
                            Corrected = subtitleRow.Corrected,
                            DownloadUri = subtitleRow.DownloadUri,
                            HD = subtitleRow.HD,
                            HearingImpaired = subtitleRow.HearingImpaired,
                            Language = subtitleRow.Language,
                            Completed = subtitleRow.Completed
                        }))
                            episode.Subtitles.Add(subtitle);
                        episodes.Add(episode);
                    }
            }
            return episodes;
        }
    }
}