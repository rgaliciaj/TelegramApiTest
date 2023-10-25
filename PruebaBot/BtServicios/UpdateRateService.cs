using Flurl.Http;
using PruebaBot.BtServicios.Base;
using PruebaBot.Tables;

namespace PruebaBot.BtServicios
{
    public class UpdateRateService : BaseScheduledService
    {

        const string baseUrl = "https://www.okx.com";
        const string User_Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.0.0 Safari/537.36";
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UpdateRateService> _logger;
        private readonly FlurlClient client;

        public UpdateRateService(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<UpdateRateService> logger) : base("Actualiza cada 10 min", TimeSpan.FromMinutes(10), logger)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;

        }

        protected override async Task ExecuteAsync()
        {
            var list = new List<TokenRate>();

        }
    }

    class Datum
    {
        public Currency baseCcy { get; set; }
        public Currency quoteCcy { get; set; }
        public decimal askPx { get; set; }
        public decimal askQuoteSz { get; set; }
        public decimal askBaseSz { get; set; }
    }

    class Root
    {
        public int code { get; set; }
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public Datum data { get; set; }
        public string detailMsg { get; set; }
        public string error_code { get; set; }
        public string error_message { get; set; }
        public string msg { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }

    enum OkxSide
    {
        Buy,
        Sell
    }
}
