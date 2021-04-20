using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using TheEliteUI.Models;

namespace TheEliteUI.Providers
{
    class RankingProvider : IRankingProvider
    {
        private const int TimeoutSec = 10;
        private const string BaseUrl = "http://localhost:54460/";
        private const string RankingRoute = "games/{0}/rankings/{1}?page={2}&count={3}&full={4}";
        private const bool GetFull = false;

        private readonly HttpClient _client;

        public RankingProvider()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = new TimeSpan(0, 0, TimeoutSec)
            };
        }

        public IReadOnlyCollection<Ranking> GetRanking(Game game, DateTime date, int page, int limit)
        {
            var response = _client
                .GetAsync(string.Format(RankingRoute, (int)game, ToDateString(date), page, limit, GetFull ? 1 : 0))
                .GetAwaiter()
                .GetResult();

            response.EnsureSuccessStatusCode();

            var content = response
                .Content
                .ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            var rankingList = JsonConvert.DeserializeObject<IReadOnlyCollection<Ranking>>(content);

            return rankingList
                .Select((r, i) => r.WithRank(i + 1))
                .ToList();
        }

        private string ToDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
