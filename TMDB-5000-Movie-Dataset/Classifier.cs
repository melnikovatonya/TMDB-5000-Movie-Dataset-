using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TMDB_5000_Movie_Dataset
{
    class Classifier
    {
        List<string> genres = new List<string>();
        List<string> keywords = new List<string>();
        int[] frequencies;
        Classifier() { }

        public void LoadDataFromFile(string file_name)
        {
            List<string> genres = new List<string>();
            List<string> keywords = new List<string>();
            List<string> owerviews = new List<string>();
            StreamReader sr = new StreamReader(file_name);
            var rx = new Regex("[0-9]+,(?<genres>\"*\\[.*\\]\"*),(?<site>.*),(?<id>[0-9]+),(?<keywords>\"*\\[.*\\]\"*),(?<lang>[a-z]+),(?<title>[^,]*),(?<owerview>\"*.*\"*),[0-9]+(\\.)*[0-9]*,\"*\\[.*\\]\"*,\"*\\[.*\\]\"*");
            {
                string s;
                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();
                    var m = rx.Match(s);
                    if (m.Success)
                    {
                        genres.Add(m.Groups["genres"].ToString());
                        keywords.Add(m.Groups["keywords"].ToString());
                        owerviews.Add(m.Groups["owerview"].ToString());
                    }
                }
            }
            List<string>[] genres_1 = new List<string>[genres.Count];
            var rx1 = new Regex("\"\"id\"\": [0-9]+, \"\"name\"\": \"\"(?<name>[a-zA-Z0-9 ]*)\"\"");
            int p, i = 0;
            foreach (var item in genres)
            {
                p = 0;
                genres_1[i] = new List<string>();
                while (p < item.Length)
                {
                    var m = rx1.Match(item, p);
                    if (!m.Success)
                    {
                        p += 1;
                        continue;
                    }
                    if (m.Success)
                    {
                        genres_1[i].Add(m.Groups["name"].ToString());
                    }
                    p = m.Index + m.Length;
                }
                i++;
            }
            List<string>[] keywords_1 = new List<string>[keywords.Count];
            i = 0;
            foreach (var item in keywords)
            {
                p = 0;
                keywords_1[i] = new List<string>();
                while (p < item.Length)
                {
                    var m = rx1.Match(item, p);
                    if (!m.Success)
                    {
                        p += 1;
                        continue;
                    }
                    if (m.Success)
                    {
                        keywords_1[i].Add(m.Groups["name"].ToString());
                    }
                    p = m.Index + m.Length;
                }
                i++;
            }
            sr.Close();
        }
    }
}
