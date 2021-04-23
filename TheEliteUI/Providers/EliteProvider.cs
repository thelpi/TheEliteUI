using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using TheEliteUI.Dtos;

namespace TheEliteUI.Providers
{
    class EliteProvider : IEliteProvider
    {
        private const int TimeoutSec = 10;
        private const string BaseUrl = "http://localhost:54460/";

        private const string PlayerRankingRoute = "games/{0}/rankings/{1}?page={2}&count={3}&full={4}";
        private const bool GetFull = false;

        private const string StandingWrRoute = "games/{0}/longest-standing-world-records?atDate={1}&untied={2}&page={3}&count={4}&stillStanding={5}";
        private const bool StillStanding = false;

        private readonly HttpClient _client;

        public EliteProvider()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = new TimeSpan(0, 0, TimeoutSec)
            };
        }

        public IReadOnlyCollection<PlayerRankingDto> GetRanking(Game game, DateTime date, int page, int limit)
        {
            var route = string.Format(PlayerRankingRoute, (int)game, ToDateString(date), page, limit, GetFull.ToString());

            var response = _client
                .GetAsync(route)
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

        public IReadOnlyCollection<StandingWrDto> GetStandingWr(Game game, DateTime atDate, bool untied, int page, int limit)
        {
            var route = string.Format(StandingWrRoute, (int)game, ToDateString(atDate), untied.ToString(), page, limit, StillStanding.ToString());

            var response = _client
                .GetAsync(route)
                .GetAwaiter()
                .GetResult();

            response.EnsureSuccessStatusCode();

            var content = response
                .Content
                .ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            return JsonConvert.DeserializeObject<IReadOnlyCollection<StandingWrDto>>(content);
        }

        private string ToDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
