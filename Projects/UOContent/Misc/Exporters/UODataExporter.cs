using System;
using System.Collections.Generic;
using Server.Configurations;
using Server.Json;
using Server.Network;
using StackExchange.Redis;

namespace Server.Misc.Exporters
{
    public class UODataExporter : Timer
    {
        private static ConnectionMultiplexer redis;

        private readonly IDatabase db;

        public UODataExporter(IDatabase getDatabase) : base(
            TimeSpan.FromSeconds(StatusExporterConfiguration.DataExporterDelay),
            TimeSpan.FromSeconds(StatusExporterConfiguration.DataExporterInterval)
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

            new UODataExporter(redis.GetDatabase()).Start();

        }

        protected override void OnTick()
        {
            var userCount = TcpServer.Instances.Count;
            var itemCount = World.Items.Count;
            var mobileCount = World.Mobiles.Count;

            var data = new
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:sszzz"),
                userCount = userCount,
                itemCount = itemCount,
                mobileCount = mobileCount
            };

            var dataJson = JsonConfig.Serialize(data);

            DateTimeOffset now = DateTime.UtcNow;
            now.ToUnixTimeSeconds();

            var key = StatusExporterConfiguration.DataExporterTimeline ?
                StatusExporterConfiguration.DataExporterKeyName  + "_" + now :
                StatusExporterConfiguration.DataExporterKeyName;

            db.StringSet(
                key: key,
                value: dataJson,
                flags: CommandFlags.FireAndForget
            );
        }
    }
}
