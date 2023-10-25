using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PruebaBot.Tables
{
    public class TokenRate
    {
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 原币种
        /// </summary>
        [JsonPropertyName("Currency")]
        public Currency Currency { get; set; }
        /// <summary>
        /// 转换币种
        /// </summary>
        [JsonPropertyName("ConvertCurrency")]
        public Currency ConvertCurrency { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        [JsonPropertyName("Rate")]
        public decimal Rate { get; set; }
        /// <summary>
        /// 反向汇率
        /// </summary>
        [JsonPropertyName("ReverseRate")]
        public decimal ReverseRate { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
    }

    public enum FiatCurrency
    {
        CNY = 10,
        USD
    }

    public enum Currency
    {
        BTC = 10,
        ETH,
        TRX,
        USDT,
    }
}
