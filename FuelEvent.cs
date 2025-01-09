using Life;
using Life.Network;
using Life.UI;
using System;
using System.IO;
using UnityEngine;

namespace HueToolsFuelEvent
{
    public class HueToolsFuelEvent : Plugin
    {
        public HueToolsFuelEvent(IGameAPI aPI) : base(aPI) { }

        public Config3 config3;

        public class Config3
        {
            public float PrixMinimum;

            public float PrixMaximum;

            public bool Message;

            public bool PanelOnSlashFuel;
        }

        public void CreateConfig3()
        {
            string directoryPath3 = pluginsPath + "/HueTools";

            string configFilePath3 = directoryPath3 + "/FuelEventTools.json";

            if (!Directory.Exists(directoryPath3))
            {
                Directory.CreateDirectory(directoryPath3);
            }

            if (!File.Exists(configFilePath3))
            {
                var defaultConfig3 = new Config3
                {
                    PrixMinimum = 0.90f,

                    PrixMaximum = 2.50f,

                    Message = true,

                    PanelOnSlashFuel = true,
                };

                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(defaultConfig3, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(configFilePath3, jsonContent);
            }

            config3 = Newtonsoft.Json.JsonConvert.DeserializeObject<Config3>(File.ReadAllText(configFilePath3));
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();

            CreateConfig3();

            new SChatCommand("/fuel", "fuel", "/fuel", (player, arg) =>
            {

                OnSlashFuel(player);



            }).Register();

            Nova.server.OnHourPassedEvent += new Action(OnHour);

        }

        public void OnHour()
        {
            System.Random random = new System.Random();

            double randomChiffre = random.NextDouble() * (config3.PrixMaximum - config3.PrixMinimum) + config3.PrixMinimum;

            float convertFloat = (float)randomChiffre;

            if (config3.Message)
            {

                Nova.server.SendMessageToAll($"<color=#d10f29>[HueTools] </color> Le prix D'essence a été modifié à <color=#0fd13c><b>{Nova.server.config.roleplayConfig.fuelPrice}</color></b> € !");



                Nova.server.config.roleplayConfig.fuelPrice = convertFloat;

                Nova.server.config.roleplayConfig.Load();

                Nova.server.config.roleplayConfig.Save();
            }
            else
            {

                Nova.server.config.roleplayConfig.fuelPrice = convertFloat;

                Nova.server.config.roleplayConfig.Load();

                Nova.server.config.roleplayConfig.Save();
            }



        }

        public void OnSlashFuel(Player player)
        {
            if (config3.PanelOnSlashFuel)
            {

                UIPanel panel = new UIPanel($"<color={ErrorColors}>Fuel</color>", UIPanel.PanelType.Text);

                panel.SetText($"L'essence est actuellement à <color=#0fd13c>{Nova.server.config.roleplayConfig.fuelPrice.ToString()} </color>€ ! ");

                panel.AddButton("Fermer", ui => player.ClosePanel(ui));

                player.ShowPanelUI(panel);

                player.SendText($"<color=#d10f29>[HueTools] </color>L'essence est actuellement à <color=#0fd13c>{Nova.server.config.roleplayConfig.fuelPrice.ToString()} </color>€ ! ");

 
            }
            else
            {
                player.SendText($"<color=#d10f29>[HueTools] </color>L'essence est actuellement à <color=#0fd13c>{Nova.server.config.roleplayConfig.fuelPrice.ToString()} </color>€ ! ");
            }
        }
    }
}
