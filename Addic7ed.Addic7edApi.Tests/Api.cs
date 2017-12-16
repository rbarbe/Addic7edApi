using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Addic7ed.Addic7edApi.Tests
{
    public class Api
    {
        [Fact]
        public async Task TestBreakingBadSubs1()
        {
            var api = new Addic7edApi.Api();
            var tvShows = await api.GetShows();
            Assert.True(tvShows.Count > 0);

            var breakingBad = tvShows.FirstOrDefault(s => s.Name == "Breaking Bad");
            Assert.NotNull(breakingBad);

            var episodes = await api.GetSeasonSubtitles(breakingBad.Id, 1);
            Assert.Equal(7, episodes.Count);

            var episodeRoughStuff = episodes.OrderBy(e => e.Number).ElementAt(6);
            Assert.Equal("A No Rough Stuff Type Deal", episodeRoughStuff.Title);
            Assert.Equal(1, episodeRoughStuff.Season);

            var italianDvdripSub =
                episodeRoughStuff.Subtitles.First(s => s.Language == "Italian" && s.Version == "DVDRip");
            Assert.False(italianDvdripSub.Corrected);
            Assert.False(italianDvdripSub.HD);
            Assert.False(italianDvdripSub.HearingImpaired);
            Assert.True(italianDvdripSub.Completed);

            var englishBluraySub =
                episodeRoughStuff.Subtitles.First(s => s.Language == "English" && s.Version == "Bluray-CtrlHD");
            Assert.False(englishBluraySub.Corrected);
            Assert.True(englishBluraySub.HD);
            Assert.False(englishBluraySub.HearingImpaired);
            Assert.True(englishBluraySub.Completed);
        }

        [Fact]
        public async Task TestBreakingBadSubs2()
        {
            var api = new Addic7edApi.Api();
            var tvShows = await api.GetShows();
            Assert.True(tvShows.Count > 0);

            var breakingBad = tvShows.FirstOrDefault(s => s.Name == "Breaking Bad");
            Assert.NotNull(breakingBad);

            var episodeRoughStuff = await api.GetEpisodeSubtitles(breakingBad.Id, 1, 7);

            Assert.Equal("A No Rough Stuff Type Deal", episodeRoughStuff.Title);
            Assert.Equal(7779, episodeRoughStuff.Id);

            var italianDvdripSub =
                episodeRoughStuff.Subtitles.First(s => s.Language == "Italian" && s.Version == "DVDRip");
            Assert.False(italianDvdripSub.Corrected);
            Assert.False(italianDvdripSub.HD);
            Assert.False(italianDvdripSub.HearingImpaired);

            var englishBluraySub =
                episodeRoughStuff.Subtitles.First(s => s.Language == "English" && s.Version == "Bluray-CtrlHD");
            Assert.False(englishBluraySub.Corrected);
            Assert.True(englishBluraySub.HD);
            Assert.False(englishBluraySub.HearingImpaired);
        }

        [Fact]
        public async Task TestBreakingBadSubs3()
        {
            var api = new Addic7edApi.Api();
            var tvShows = await api.GetShows();
            Assert.True(tvShows.Count > 0);

            var breakingBad = tvShows.FirstOrDefault(s => s.Name == "Breaking Bad");
            Assert.NotNull(breakingBad);

            var episodeRoughStuff = await api.GetEpisodeSubtitles(breakingBad.Id, 1, 67);
            Assert.Null(episodeRoughStuff);
        }

        [Fact]
        public async Task TestBreakingBadSubs4()
        {
            var api = new Addic7edApi.Api();
            var tvShows = await api.GetShows();
            Assert.True(tvShows.Count > 0);

            var breakingBad = tvShows.FirstOrDefault(s => s.Name == "Breaking Bad");
            Assert.NotNull(breakingBad);

            var episode67Season32 = await api.GetEpisodeSubtitles(breakingBad.Id, 32, 67);
            Assert.Null(episode67Season32);
        }

        [Fact]
        public async Task TestBreakingBadSubs5()
        {
            var api = new Addic7edApi.Api();
            var tvShows = await api.GetShows();
            Assert.True(tvShows.Count > 0);

            var breakingBad = tvShows.FirstOrDefault(s => s.Name == "Breaking Bad");
            Assert.NotNull(breakingBad);

            var season32 = await api.GetSeasonSubtitles(breakingBad.Id, 32);
            Assert.Empty(season32);
        }

        [Fact]
        public async Task TestBreakingBadSubs6()
        {
            var api = new Addic7edApi.Api();
            var tvShows = await api.GetShows();
            Assert.True(tvShows.Count > 0);

            var breakingBad = tvShows.FirstOrDefault(s => s.Name == "Breaking Bad");
            Assert.NotNull(breakingBad);

            var episode = await api.GetEpisodeSubtitles(breakingBad.Id, 1, 1);
            Assert.NotNull(episode);

            var subtitle = episode.Subtitles.First();

            var downloadSubtitleResult = await api.DownloadSubtitle(breakingBad.Id, subtitle.DownloadUri);

            Assert.Equal(".srt", Path.GetExtension(downloadSubtitleResult.Filename));
            Assert.Equal("text/srt", downloadSubtitleResult.Mediatype);
            var subtitleText = new StreamReader(downloadSubtitleResult.Stream).ReadToEnd();
            Assert.Contains("www.subsfactory.it", subtitleText);
        }

        [Fact]
        public async Task TestGetNumberOfSeasons()
        {
            var api = new Addic7edApi.Api();
            var tvShows = await api.GetShows();
            Assert.True(tvShows.Count > 0);

            var breakingBad = tvShows.FirstOrDefault(s => s.Name == "Breaking Bad");
            Assert.NotNull(breakingBad);

            var numberOfSeasons = await api.GetNumberOfSeasons(breakingBad.Id);
            Assert.Equal(5, numberOfSeasons);
        }

        [Fact]
        public async Task TestGetShows()
        {
            var api = new Addic7edApi.Api();
            var tvShows = await api.GetShows();

            Assert.True(tvShows.Count > 0);
        }

        [Fact]
        public async Task TestNoSeason()
        {
            var api = new Addic7edApi.Api();

            var numberOfSeasons = await api.GetNumberOfSeasons(53403);
            Assert.Equal(0, numberOfSeasons);
        }
    }
}