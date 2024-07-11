using System;
using System.Net.Http;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace api.Service
{
    public class FMPService : IFMPService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<FMPService> _logger;


        public FMPService(HttpClient httpClient, IConfiguration config, ILogger<FMPService> logger)


        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Stock> FindStockBySymbolAsync(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));
            }

            var apiKey = _config["FMPKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("FMP API key is not configured.");
                throw new InvalidOperationException("FMP API key is not configured.");
            }

            try
            {
                var url = $"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={apiKey}";
                _logger.LogInformation($"Requesting stock data for symbol: {symbol} from URL: {url}");

                var result = await _httpClient.GetAsync(url);

                _logger.LogInformation($"HTTP response status code: {result.StatusCode}");

                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(content))
                    {
                        _logger.LogWarning($"Empty response for symbol {symbol}");
                        return null;
                    }

                    var stocks = JsonConvert.DeserializeObject<FMPStock[]>(content);
                    if (stocks == null || stocks.Length == 0)
                    {
                        _logger.LogWarning($"No stock information found for symbol {symbol}");
                        return null;
                    }

                    var stock = stocks[0];
                    _logger.LogInformation($"Stock data retrieved for symbol: {symbol}");
                    return stock.ToStockFromFMP();
                }

                _logger.LogError($"Error fetching data for symbol {symbol}: {result.StatusCode}");
                _logger.LogError($"Response content: {await result.Content.ReadAsStringAsync()}");
                return null;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"HTTP request error while fetching stock data for symbol {symbol}");
                return null;
            }
            catch (JsonException e)
            {
                _logger.LogError(e, $"JSON deserialization error for symbol {symbol}");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unexpected error while fetching stock data for symbol {symbol}");
                return null;
            }
        }
    }
}
