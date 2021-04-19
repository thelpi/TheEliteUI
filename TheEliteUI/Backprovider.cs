using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheEliteUI.Model;

namespace TheEliteUI
{
    class BackProvider : IBackProvider
    {
        private const int TimeoutSec = 10;
        private const string BaseUrl = "http://localhost:54460/";
        private const string RankingRoute = "games/{0}/rankings/{1}?page={2}&count={3}&full={4}";
        private const int DefaultPage = 0;
        private const int DefaultLimit = 25;

        private readonly HttpClient _client;

        public BackProvider()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = new TimeSpan(0, 0, TimeoutSec)
            };
        }

        public async Task<IReadOnlyCollection<Ranking>> GetRankingAsync(Game game, DateTime date)
        {
            var response = await _client
                .GetAsync(string.Format(RankingRoute, (int)game, ToDateString(date), DefaultPage, DefaultLimit, 0))
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response
                .Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<IReadOnlyCollection<Ranking>>(content);
        }

        private string ToDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
