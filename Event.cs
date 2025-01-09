using Life;
using Life.Network;
using Life.UI;
using Socket.Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static HueToolsEvent.HueTools.DiscordWebhook;
using System.IO;



namespace HueToolsEvent
{
    public class HueTools : Plugin
    {
        public HueTools(IGameAPI aPI) : base(aPI) { }




        public static string SuccessColors = "#85E085";
        public static string ErrorColors = "#DD4B4E";
        public static string WarningColors = "#FCBE86";
        public static string InfoColors = "#4287F9";
        public static string GreyColors = "#ADADAD";
        public static string PurpleColors = "#DB70DB";

        public class DiscordWebhook
        {
            private static readonly HttpClient client = new HttpClient();



            public class WebhookEmbed
            {
                public string Title { get; set; }
                public string Description { get; set; }
                public string Color { get; set; }
                public DateTime Timestamp { get; set; }
            }

            public static async Task SendWebhookAsyncEmbed(string webhookUrl, WebhookEmbed embed)
            {

                var payload = new
                {
                    embeds = new[] { new
            {
                title = embed.Title,
                description = embed.Description,
                color = int.Parse(embed.Color.Replace("#", ""), System.Globalization.NumberStyles.HexNumber),
                timestamp = embed.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ")
            }}
                };


                string jsonPayload = JsonConvert.SerializeObject(payload);


                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


                try
                {
                    var response = await client.PostAsync(webhookUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Message envoyé avec succès.");
                    }
                    else
                    {
                        Console.WriteLine("Erreur lors de l'envoi du message.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
        }
        public Config config;

        public class Config
        {
            public string WebhhookurlEvent;

            public int LevelAdminRequired;
        }

        public void CreateConfig()
        {
            string directoryPath = pluginsPath + "/HueTools";

            string configFilePath = directoryPath + "/EventsTools.json";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(configFilePath))
            {
                var defaultConfig = new Config
                {
                    WebhhookurlEvent = "",

                    LevelAdminRequired = 3,


                };

                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(defaultConfig, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(configFilePath, jsonContent);
            }

            config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFilePath));
        }



        public override void OnPluginInit()
        {
            base.OnPluginInit();

            new SChatCommand("/event", "event", "/event", (player, arg) =>
            {

                OnSlashEvent(player);



            }).Register();



            CreateConfig();

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("[HueTools V.1.0.0] initialized success");

            Console.ForegroundColor = ConsoleColor.White;
        }



        public void OnSlashEvent(Player player)
        {
            if (player.account.adminLevel >= config.LevelAdminRequired)
            {
                UIPanel panel = new UIPanel($"<color={ErrorColors}>Event</color>", UIPanel.PanelType.Tab);

                panel.AddTabLine("<b>Mute/UnMute All</b>", ui =>
                {
                    MuteOrUnmute(player);

                    player.ClosePanel(ui);

                });

                panel.AddTabLine("<b>Freeze/Unfreeze All</b>", ui =>
                {

                    FreezeOrUnfreeze(player);

                    player.ClosePanel(ui);

                    

                });

                panel.AddTabLine("<b>Teleport All</b>", ui =>
                {
                    TpAll(player);

                    player.ClosePanel(ui);

                });
               

                panel.AddTabLine("<b>Mégaphone Admin</b>", ui =>
                {
                    Megaphone(player);

                    player.ClosePanel(ui);

                });

                panel.AddTabLine("<b>UnSeat All</b>", ui =>
                {
                    UnSeatAll(player);

                    player.ClosePanel(ui);

                });

                panel.AddTabLine("<b>Event Alerter</b>", ui =>
                {

                    EventAlerter(player);
                });

                panel.AddButton("Fermer", ui => player.ClosePanel(ui));

                panel.AddButton("Valider", ui => ui.SelectTab());

                player.ShowPanelUI(panel);


            }
            else
            {
                player.Notify("Avertissement", $"Tu n'es pas administrateur niveau {config.LevelAdminRequired.ToString()} ou plus !", Life.NotificationManager.Type.Warning);
            }
        }

        public void Megaphone(Player player)
        {
            if (!player.setup.voice.Networkmegaphone) {

                
                player.SendText($"<color={ErrorColors}>[HueTools]</color> <color={SuccessColors}> Tu as bien activé ton mégaphone admin ! </color>");

                player.setup.voice.Networkmegaphone = true;
            } 
            else
            {
                player.SendText($"<color={ErrorColors}> [HueTools] </color> <color={SuccessColors}> Tu as bien désavtivé ton mégaphone admin ! </color>");

                player.setup.voice.Networkmegaphone = false;
            }
        }
        public void MuteOrUnmute(Player player)
        {
            foreach (var players in Nova.server.Players)
            {
                if (players.account.AdminLevel == 0)
                {
                    if (players.setup.voice.NetworkisMuted == true)
                    {
                        players.setup.voice.NetworkisMuted = false;

                        player.SendText($"<color={ErrorColors}>[HueTools]</color> <color={SuccessColors}> Tu as Unmute tous les joueurs du serveur avec succés (sauf les admins) ! </color>");

                        players.SendText($"<color={ErrorColors}>[HueTools]</color> <color={SuccessColors}> Tu as été Unmute comme tous les joueurs du serveur avec succés ! </color>");
                    }
                    else
                    {
                        players.setup.voice.NetworkisMuted = true;

                        player.SendText($"<color={ErrorColors}>[HueTools]</color> <color={SuccessColors}> Tu as Mute tous les joueurs du serveur avec succés (sauf les admins) ! </color>");

                        players.SendText($"<color={ErrorColors}>[HueTools]</color> <color={SuccessColors}> Tu as été Mute comme tous les joueurs du serveur avec succés ! </color>");
                    }
                }
            }

        }
        public void FreezeOrUnfreeze(Player player)
        {
            foreach (var players in Nova.server.Players)
            {
                if (players.account.AdminLevel == 0)
                {
                    if (players.setup.NetworkisFreezed)
                    {
                        players.setup.voice.NetworkisMuted = false;

                        player.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as Unfreeze tous les joueurs du serveur avec succés (sauf les admins) ! </color>");

                        players.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as été Unfreeze comme tous les joueurs du serveur avec succés ! </color>");
                    }
                    else
                    {
                        players.setup.NetworkisFreezed = true;

                        player.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as Freeze tous les joueurs du serveur avec succés (sauf les admins) ! </color>");

                        players.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as été Freeze comme tous les joueurs du serveur avec succés ! </color>");
                    }
                }
            }
        }

        public void TpAll(Player player)
        {
            foreach (var players in Nova.server.Players)
            {
                if (players.account.AdminLevel == 0)
                {

                    if (players.setup.driver)
                    {
                        players.setup.TargetExitVehicle();

                        players.setup.TargetSetPosition(player.setup.transform.position);

                        player.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as Téleportée tous les joueurs du serveur avec succés (sauf les admins) ! </color>");

                        players.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as été Téleportée comme tous les joueurs du serveur avec succés ! </color>");

                    }
                    else
                    {

                        players.setup.TargetSetPosition(player.setup.transform.position);

                        player.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as Téleportée tous les joueurs du serveur avec succés (sauf les admins) ! </color>");

                        players.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as été Téleportée comme tous les joueurs du serveur avec succés ! </color>");
                    }

                }
            }

        }

        public void UnSeatAll(Player player)
        {
            foreach (var players in Nova.server.Players)
            {

                if (players.setup.driver)
                {

                    players.setup.TargetExitVehicle();

                    player.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as sortis  tous les joueurs du serveur de leur voiture avec succés ! </color>");

                    players.SendText($"<color={ErrorColors}>[HueTools]</color><color={SuccessColors}> Tu as été sortis de ta voiture comme tous les joueurs étant du serveur avec succés ! </color>");
                }

            }


            }
        


        static async Task Main(string[] args, Config config)
        {
            string webhookUrl = config.WebhhookurlEvent;
            string message = "@everyone";

            await SendWebhookAsync(webhookUrl, message);
        }

        public static async Task SendWebhookAsync(string webhookUrl, string message)
        {
            using (HttpClient client = new HttpClient())
            {

                var payload = new
                {
                    content = message
                };


                string json = JsonConvert.SerializeObject(payload);

                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {

                    HttpResponseMessage response = await client.PostAsync(webhookUrl, content);


                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Message envoyé avec succès !");
                    }
                    else
                    {
                        Console.WriteLine($"Erreur lors de l'envoi : {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
                }
            }
        }



        public void EventAlerter(Player player)
        {
            UIPanel panel = new UIPanel($"<color={ErrorColors}> EventAlerter </color>", UIPanel.PanelType.Input);

            panel.SetInputPlaceholder("Entrez le type de l'évenement...");

            panel.AddButton("Fermer", ui => player.ClosePanel(ui));

            panel.AddButton("Valider", ui =>
            {
                UIPanel panel1 = new UIPanel($"<color={ErrorColors}> EventAlerter Gain </color>", UIPanel.PanelType.Tab);


                panel1.AddTabLine("Oui", ui1 =>
                {
                    UIPanel panel2 = new UIPanel($"<color={ErrorColors}> EventAlerter Date", UIPanel.PanelType.Input);

                    panel2.SetInputPlaceholder("Entrez la date de l'évent (JJ/MM HH:MM)...");

                    panel2.AddButton("Fermer", ui2 => player.ClosePanel(ui2));

                    panel2.AddButton("Valider", ui2 =>
                    {
                        UIPanel panel3 = new UIPanel($"<color={ErrorColors}> EventAlerter Description", UIPanel.PanelType.Input);

                        panel3.SetInputPlaceholder("Entrez La description de l'évenement....");

                        panel3.AddButton("Fermer", ui3 => player.ClosePanel(ui3));

                        panel3.AddButton("Valider", ui3 =>
                        {
                            UIPanel panel4 = new UIPanel($"<color={ErrorColors}>Mention Everyone</color>", UIPanel.PanelType.Tab);

                            panel4.AddTabLine("Mentionner", async ui4 =>
                            {
                                string webhookUrl = $"{config.WebhhookurlEvent}";
                                WebhookEmbed embed = new WebhookEmbed
                                {
                                    Title = "Evénement",
                                    Description = $"# Type D'évenement : {panel.inputText} \n" +
                                    $"# Gain : Oui \n" +
                                    $"# Date : {panel2.inputText} \n" +
                                    $"# Description : \n" +
                                    $"{panel3.inputText}",
                                    Color = "#42b0f5",
                                    Timestamp = DateTime.UtcNow
                                };

                                try
                                {

                                    await Task.Delay(1000);


                                    await SendWebhookAsyncEmbed(webhookUrl, embed);

                                    string message = "@everyone";

                                    await SendWebhookAsync(webhookUrl, message);
                                }
                                catch (Exception ex)
                                {

                                    Debug.Log($"Une erreur est survenue lors de l'envoi du webhook : {ex.Message}");
                                }

                                player.Notify("Succés", "Message Envoyer avec succés !", Life.NotificationManager.Type.Success);

                                player.ClosePanel(ui4);

                            });

                            panel4.AddTabLine("Ne pas Mentionner", async ui4 =>
                            {
                                string webhookUrl = $"{config.WebhhookurlEvent}";
                                WebhookEmbed embed = new WebhookEmbed
                                {
                                    Title = "Evénement",
                                    Description = $"# Type D'évenement : {panel.inputText} \n" +
                                    $"# Gain : Oui \n" +
                                    $"# Date : {panel2.inputText} \n" +
                                    $"# Description : \n" +
                                    $"{panel3.inputText}",
                                    Color = "#42b0f5",
                                    Timestamp = DateTime.UtcNow
                                };

                                try
                                {

                                    await Task.Delay(1000);


                                    await SendWebhookAsyncEmbed(webhookUrl, embed);
                                }
                                catch (Exception ex)
                                {

                                    Debug.Log($"Une erreur est survenue lors de l'envoi du webhook : {ex.Message}");
                                }

                                player.Notify("Succés", "Message Envoyer avec succés !", Life.NotificationManager.Type.Success);

                                player.ClosePanel(ui4);
                            });


                            panel4.AddButton("Fermer", ui4 => player.ClosePanel(ui4));

                            panel4.AddButton("Valider", ui4 => ui4.SelectTab());

                            player.ShowPanelUI(panel4);




                        });

                        player.ShowPanelUI(panel3);


                    });

                    player.ShowPanelUI(panel2);

                });



                panel1.AddTabLine("Non", ui1 =>
                {
                    UIPanel panel9 = new UIPanel($"<color={ErrorColors}> EventAlerter Date", UIPanel.PanelType.Input);

                    panel9.SetInputPlaceholder("Entrez la date de l'évent (JJ/MM HH:MM)...");

                    panel9.AddButton("Fermer", ui9 => player.ClosePanel(ui9));

                    panel9.AddButton("Valider", ui9 =>
                    {
                        UIPanel panel8 = new UIPanel($"<color={ErrorColors}> EventAlerter Descriptions </color>", UIPanel.PanelType.Input);

                        panel8.SetInputPlaceholder("Entrez La description de l'évenement....");

                        panel8.AddButton("Fermer", ui8 => player.ClosePanel(ui8));

                        panel8.AddButton("Valider", ui8 =>
                        {
                            UIPanel panel4 = new UIPanel($"<color={ErrorColors}>Mention Everyone</color>", UIPanel.PanelType.Tab);

                            panel4.AddTabLine("Mentionner", async ui4 =>
                            {
                                string webhookUrl = $"{config.WebhhookurlEvent}";
                                WebhookEmbed embed = new WebhookEmbed
                                {
                                    Title = "Evénement",
                                    Description = $"# Type D'évenement : {panel.inputText} \n" +
                                    $"# Gain : Non \n" +
                                    $"# Date : {panel9.inputText} \n" +
                                    $"# Description : \n" +
                                    $"{panel8.inputText}",
                                    Color = "#42b0f5",
                                    Timestamp = DateTime.UtcNow
                                };

                                try
                                {

                                    await Task.Delay(1000);


                                    await SendWebhookAsyncEmbed(webhookUrl, embed);

                                    string message = "@everyone";

                                    await SendWebhookAsync(webhookUrl, message);
                                }
                                catch (Exception ex)
                                {

                                    Debug.Log($"Une erreur est survenue lors de l'envoi du webhook : {ex.Message}");
                                }

                                player.Notify("Succés", "Message Envoyer avec succés !", Life.NotificationManager.Type.Success);

                                player.ClosePanel(ui4);

                            });

                            panel4.AddTabLine("Ne pas Mentionner", async ui4 =>
                            {
                                string webhookUrl = $"{config.WebhhookurlEvent}";
                                WebhookEmbed embed = new WebhookEmbed
                                {
                                    Title = "Evénement",
                                    Description = $"# Type D'évenement : {panel.inputText} \n" +
                                    $"# Gain : Non \n" +
                                    $"# Date : {panel9.inputText} \n" +
                                    $"# Description : \n" +
                                    $"{panel8.inputText}",
                                    Color = "#42b0f5",
                                    Timestamp = DateTime.UtcNow
                                };

                                try
                                {

                                    await Task.Delay(1000);


                                    await SendWebhookAsyncEmbed(webhookUrl, embed);
                                }
                                catch (Exception ex)
                                {

                                    Debug.Log($"Une erreur est survenue lors de l'envoi du webhook : {ex.Message}");
                                }

                                player.Notify("Succés", "Message Envoyer avec succés !", Life.NotificationManager.Type.Success);

                                player.ClosePanel(ui4);
                            });


                            panel4.AddButton("Fermer", ui4 => player.ClosePanel(ui4));

                            panel4.AddButton("Valider", ui4 => ui4.SelectTab());

                            player.ShowPanelUI(panel4);







                        });

                        player.ShowPanelUI(panel8);


                    });

                    player.ShowPanelUI(panel9);


                });

                panel1.AddButton("Fermer", ui1 => player.ClosePanel(ui1));

                panel1.AddButton("Valider", ui1 => ui1.SelectTab());

                player.ShowPanelUI(panel1);

            });

            player.ShowPanelUI(panel);

        }

    }


}
