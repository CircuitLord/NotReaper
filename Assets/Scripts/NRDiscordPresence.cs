using System.Collections;
using System.Collections.Generic;
using NotReaper.Managers;
using UnityEngine;



namespace NotReaper {


    public class NRDiscordPresence : MonoBehaviour {

        public DifficultyManager difficultyManager;

        public bool showTime;

        public DiscordPresence presence;

        private void Start() {
            //Update the values
            //UpdateFields(presence);
            presence.details = "In Menus";
            presence.state = "";

            presence.startTime = showTime ? new DiscordTimestamp(Time.realtimeSinceStartup) : DiscordTimestamp.Invalid;

            float duration = 0;
            presence.endTime = duration > 0 ? new DiscordTimestamp(Time.realtimeSinceStartup + duration) : DiscordTimestamp.Invalid;



            //Register to a presence change
            DiscordManager.current.events.OnPresenceUpdate.AddListener((message) => {
                Debug.Log("Received a new presence! Current App: " + message.ApplicationID + ", " + message.Name);
                //UpdateFields(new DiscordPresence(message.Presence));
            });

            DiscordManager.current.SetPresence(presence);

        }

        public void SendPresence() {
            //UpdatePresence();
            DiscordManager.current.SetPresence(presence);
        }


        public void UpdatePresenceDifficulty(int diffIndex) {

            switch (diffIndex) {
                case 0:
                    presence.smallAsset = new DiscordAsset() {
                        image = "diffexpert",
                        tooltip = "Expert"
                    };
                    break;

                case 1:
                    presence.smallAsset = new DiscordAsset() {
                        image = "diffadvanced",
                        tooltip = "Advanced"
                    };
                    break;

                case 2:
                    presence.smallAsset = new DiscordAsset() {
                        image = "diffstandard",
                        tooltip = "Standard"
                    };
                    break;

                case 3:
                    presence.smallAsset = new DiscordAsset() {
                        image = "diffeasy",
                        tooltip = "Easy"
                    };
                    break;
            }

            DiscordManager.current.SetPresence(presence);
        }

        public void UpdatePresenceSongName(string name) {
            presence.details = "Mapping:";
            presence.state = name;
            DiscordManager.current.SetPresence(presence);
        }




        public void UpdatePresence() {
            presence.state = "NotReaper";
            presence.details = ("Mapping: ");
            presence.startTime = showTime ? new DiscordTimestamp(Time.realtimeSinceStartup) : DiscordTimestamp.Invalid;

            presence.largeAsset = new DiscordAsset() {
                image = "logosquare",
                tooltip = "NotReaper v0.3"
            };
            presence.smallAsset = new DiscordAsset() {
                image = "diffstandard",
                tooltip = "Standard"
            };

            //float duration = float.Parse(inputEndTime.text);
            float duration = 0;
            presence.endTime = duration > 0 ? new DiscordTimestamp(Time.realtimeSinceStartup + duration) : DiscordTimestamp.Invalid;
        }
    }


}