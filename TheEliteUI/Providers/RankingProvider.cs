using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using TheEliteUI.Dtos;

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

        public IReadOnlyCollection<PlayerRankingDto> GetRanking(Game game, DateTime date, int page, int limit)
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

            return JsonConvert.DeserializeObject<IReadOnlyCollection<PlayerRankingDto>>(content);
        }

        private string ToDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
