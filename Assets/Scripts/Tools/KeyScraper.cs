using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using NotReaper.Models;

namespace NotReaper.Tools
{
    static class KeyScraper
    {

        public static Dictionary<string, string> pitchEventDict = new Dictionary<string, string>
        {
            {"A", "A" },
            {"A#", "A#" },
            {"B\u266D", "A#" },
            {"B", "B" },
            {"C", "C" },
            {"C#", "C#" },
            {"D\u266D", "C#" },
            {"D", "D" },
            {"D#", "D#" },
            {"E\u266D", "D#" },
            {"E", "E" },
            {"F", "F" },
            {"F#", "F#" },
            {"G\u266D", "F#" },
            {"G", "G" },
            {"G#", "G#" },
            {"A\u266D", "G#" },
        };

        static int scrapeCount = 0;

        public static string GetSongEndEvent(string artist, string songName)
        {
            artist = Regex.Replace(artist, @"\s+\(.+\)", "", RegexOptions.IgnoreCase); //Remove anything within parenthesees
            artist = Regex.Replace(artist, @"\s+\(?Feat\..+", "", RegexOptions.IgnoreCase); //Remove feat.
            songName = Regex.Replace(songName, @"\s+\(.+\)", "", RegexOptions.IgnoreCase); //Remove anything within parenthesees, such as (TV Size) or (Cut ver.)

            string key = GetKey($@"{artist} {songName}");

            if (key == "") return "event:/song_end/song_end_C#";
            else
            {
                string pitch = key.Split(' ')[0];

                if (pitchEventDict.ContainsKey(pitch)) return "event:/song_end/song_end_" + pitchEventDict[pitch];
                else return "event:/song_end/song_end_C#";
            }
        }

        public static string GetKey(string search)
        {
            if (scrapeCount >= 14) return "";
            string content = "";
            try
            {
                WebClient client = new WebClient();
                string query = HttpUtility.UrlEncode(search);

                scrapeCount++;
                content = client.DownloadString($@"https://tunebat.com/Search?q={query}");
            }
            catch (Exception ex)
            {
                return "";
            }

            Match match = Regex.Match(content, @">.{1,2} (minor|major)", RegexOptions.IgnoreCase);
            if (!match.Success) return "";
            return match.Value.Replace(">", "");
        }
    }
}
