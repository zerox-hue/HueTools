using Life;
using Life.Network;
using System;
using System.IO;
using Life.UI;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace HueToolsDiscord
{
    public class HueToolsDiscord : Plugin
    {
        public HueToolsDiscord(IGameAPI aPI) : base(aPI) { }

        public static string SuccessColors = "#85E085";
        public static string ErrorColors = "#DD4B4E";
        public static string WarningColors = "#FCBE86";
        public static string InfoColors = "#4287F9";
        public static string GreyColors = "#ADADAD";
        public static string PurpleColors = "#DB70DB";

        public Config config;

        public class Config
        {
            public string DiscordServerUrl;
        }

        public void CreateConfig()
        {
            string directoryPath = pluginsPath + "/HueTools";

            string configFilePath = directoryPath + "/DiscordTools.json";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(configFilePath))
            {
                var defaultConfig = new Config
                {
                    DiscordServerUrl = "https://discord.gg/TonUrl",


                };

                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(defaultConfig, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(configFilePath, jsonContent);
            }

            config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFilePath));
        }


        public override void OnPluginInit()
        {
            base.OnPluginInit();

            CreateConfig();

            new SChatCommand("/discord", "discord", "/discord", (player, arg) =>
            {
                OnSlahDiscord(player);


            }).Register();

        }

        public void OnSlahDiscord(Player player)
        {
            UIPanel panel = new UIPanel($"<color={ErrorColors}>Discord</color>", UIPanel.PanelType.Text);

            panel.SetText($"Le Discord de <color={PurpleColors}> {Nova.serverInfo.serverListName} </color> est <color={SuccessColors}> {config.DiscordServerUrl} </color> !");

            panel.AddButton("Fermer", ui => player.ClosePanel(ui));

            player.ShowPanelUI(panel);

            player.SendText($"Le Discord de <color={PurpleColors}> {Nova.serverInfo.serverListName} </color> est <color={SuccessColors}> {config.DiscordServerUrl} </color> !");
            
        }
    }

}
