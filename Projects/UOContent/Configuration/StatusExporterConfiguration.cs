/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2022 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: EmailConfiguration.cs                                           *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MimeKit;
using Server.Json;
using Server.Logging;

namespace Server.Configurations
{
    public static class StatusExporterConfiguration
    {
        private static readonly ILogger logger = LogFactory.GetLogger(typeof(StatusExporterConfiguration));

        private const string m_RelPath = "Configuration/status-exporter-settings.json";

        public static bool Enabled { get; private set; }

        public static string RedisHost { get; private set; }

        public static int RedisPort { get; private set; }

        public static string RedisPassword { get; private set; }

        public static float OnlineStatusDelay { get; private set; }

        public static float OnlineStatusInterval { get; private set; }

        public static string OnlineKeyName { get; private set; }

        public static double OnlineKeyExpiry { get; private set; }

        public static float DataExporterDelay { get; private set; }

        public static float DataExporterInterval { get; private set; }

        public static void Configure()
        {
            var path = Path.Join(Core.BaseDirectory, m_RelPath);

            Settings settings;

            if (File.Exists(path))
            {
                settings = JsonConfig.Deserialize<Settings>(path);

                if (settings == null)
                {
                    logger.Error("Failed reading status exporter configuration from {Path}", m_RelPath);
                    throw new JsonException($"Failed to deserialize {path}.");
                }

                logger.Information("Status Exporter configuration read from {Path}", m_RelPath);
            }
            else
            {
                settings = new Settings();
                JsonConfig.Serialize(path, settings);
                logger.Information("Status Exporter configuration saved to {}.", m_RelPath);
            }

            Enabled = settings.enabled;
            RedisHost = settings.redisHost;
            RedisPort = settings.redisPort;
            RedisPassword = settings.redisPassword;
            OnlineStatusDelay = settings.onlineStatusDelay;
            OnlineStatusInterval = settings.onlineStatusInterval;
            OnlineKeyName = settings.onlineKeyName;
            OnlineKeyExpiry = settings.onlineKeyExpiry;
            DataExporterDelay = settings.dataExporterDelay;
            DataExporterInterval = settings.dataExporterInterval;
        }

        public class Settings
        {
            [JsonPropertyName("enabled")]
            public bool enabled { get; set; } = false;

            [JsonPropertyName("redisHost")]
            public string redisHost { get; set; } = "localhost";

            [JsonPropertyName("redisPort")]
            public int redisPort { get; set; } = 6379;

            [JsonPropertyName("redisPassword")]
            public string redisPassword { get; set; } = "";

            [JsonPropertyName("onlineStatusDelay")]
            public float onlineStatusDelay { get; set; } = 20.0f;

            [JsonPropertyName("onlineStatusInterval")]
            public float onlineStatusInterval { get; set; } = 5.0f;

            [JsonPropertyName("onlineKeyName")]
            public string onlineKeyName { get; set; } = "online-status";

            [JsonPropertyName("onlineKeyExpiry")]
            public double onlineKeyExpiry { get; set; } = 30;

            [JsonPropertyName("dataExporterDelay")]
            public float dataExporterDelay { get; set; } = 120.0f;

            [JsonPropertyName("dataExporterInterval")]
            public float dataExporterInterval { get; set; } = 3600.0f;
        }
    }
}
