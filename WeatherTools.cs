using Life;
using Life.Network;
using Life.UI;
using System;
using System.IO;
using UnityEngine;


namespace HueWeatherTools
{
    public class HueWeatherTools : Plugin
    {
        public HueWeatherTools(IGameAPI aPI) : base(aPI) { }

        public static string SuccessColors = "#85E085";
        public static string ErrorColors = "#DD4B4E";
        public static string WarningColors = "#FCBE86";
        public static string InfoColors = "#4287F9";
        public static string GreyColors = "#ADADAD";
        public static string PurpleColors = "#DB70DB";

        public Config config;

        public class Config
        {
            public bool MessageAll;

        }

        public void CreateConfig()
        {
            string directoryPath = pluginsPath + "/HueTools";

            string configFilePath = directoryPath + "/WeatherEvent.json";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(configFilePath))
            {
                var defaultConfig = new Config
                {
                   MessageAll = true,

                };

                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(defaultConfig, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(configFilePath, jsonContent);
            }

            config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFilePath));
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();

            Nova.server.OnHourPassedEvent += new Action(OnHourWeather);

            CreateConfig();
        }

        public void OnHourWeather()
        {
            System.Random rnd = new System.Random();

            int random = rnd.Next(1, 7);

            if(random == 1)
            {
                if(config.MessageAll)
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.ClearSky);

                    Nova.server.SendMessageToAll($"<color={ErrorColors}>[HueTools]</color> Le ciel s'éclaircie !");
                }
                else
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.ClearSky);
                }
            }
            if (random == 2)
            {
                if (config.MessageAll)
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.Cloudy1);

                    Nova.server.SendMessageToAll($"<color={ErrorColors}>[HueTools]</color> Le ciel s'éclaircie !");
                }
                else
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.Cloudy1);
                }
            }
            if (random == 3)
            {
                if (config.MessageAll)
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.HeavyRain);

                    Nova.server.SendMessageToAll($"<color={ErrorColors}>[HueTools]</color> La pluie s'abbat sur la ville!");
                }
                else
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.HeavyRain);
                }
            }
            if (random == 4)
            {
                if (config.MessageAll)
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.Storm);

                    Nova.server.SendMessageToAll($"<color={ErrorColors}>[HueTools]</color> Il y a une tempête de fort vent s'abbate sur la ville puis il y a de l'orage et de la pluie !");
                }
                else
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.Storm);
                }
            }
            if (random == 5)
            {
                if (config.MessageAll)
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.ClearSky);

                    Nova.server.SendMessageToAll($"<color={ErrorColors}>[HueTools]</color> Le ciel s'éclaircie !");
                }
                else
                {
                    Nova.server.world.SetWeather(Life.Network.Systems.SWorld.Weather.ClearSky);
                }

            }



        }
    }
}
