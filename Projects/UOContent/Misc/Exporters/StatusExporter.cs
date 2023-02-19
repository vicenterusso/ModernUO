using System;
using Server.Configurations;
using StackExchange.Redis;

namespace Server.Misc.Exporters
{
    public class StatusExporter : Timer
    {
        private static ConnectionMultiplexer redis;

        private readonly IDatabase db;

        public StatusExporter(IDatabase getDatabase) : base(
            TimeSpan.FromSeconds(StatusExporterConfiguration.OnlineStatusDelay),
            TimeSpan.FromSeconds(StatusExporterConfiguration.OnlineStatusInterval)
        )
        {
            db = getDatabase;
        }

        public static void Initialize()
        {
            if (!StatusExporterConfiguration.Enabled)
            {
                return;
            }

            var connectionString = $"{StatusExporterConfiguration.RedisHost}:{StatusExporterConfiguration.RedisPort}";
            var options = ConfigurationOptions.Parse(connectionString);

            if(!string.IsNullOrEmpty(StatusExporterConfiguration.RedisPassword))
            {
                options.Password = StatusExporterConfiguration.RedisPassword;
            }

            redis = ConnectionMultiplexer.Connect(options);

            new StatusExporter(redis.GetDatabase()).Start();

        }

        protected override void OnTick()
        {
            db.StringSet(
                key: StatusExporterConfiguration.OnlineKeyName,
                value: DateTime.Now.ToString("yyyy-MM-dd HH:mm:sszzz"),
                expiry: TimeSpan.FromSeconds(StatusExporterConfiguration.OnlineKeyExpiry),
                flags: CommandFlags.FireAndForget
            );
        }
    }
}
