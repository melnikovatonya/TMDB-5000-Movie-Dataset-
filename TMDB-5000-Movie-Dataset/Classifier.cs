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
        string[] genres;
        string[] keywords;
        int[,] frequencies;
        Classifier() { }

        public void LoadDataFromFile(string file_name)
        {
            List<string> genres = new List<string>();
            List<string> keywords = new List<string>();
            List<string> owerviews = new List<string>();
            StreamReader sr = new StreamReader(file_name);
			#region regex
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
			#endregion
			#region genres
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
			#endregion
			#region keywords
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
			#endregion
			#region owerviews
			List<string>[] owerviews_1 = new List<string>[owerviews.Count];
			i = 0;
			foreach(var item in owerviews)
			{
				owerviews_1[i] = TransformOfDescrip(item);
				i++;
			}
			#endregion
			sr.Close();
        }

		List<string> TransformOfDescrip(string description)
        {
			string file_name = "";
            char[] c = { ' ', ',', '.', '!', '?', ':', ';', '(', ')', '"', '-' };
            string[] words = description.Split(c); // массив слов из описания без знаков
            StreamReader sr = new StreamReader(file_name);
            string[] prep_and_conj = sr.ReadToEnd().Split('\r', '\n'); // массив предлогов и союзов из файла
            List<string> transform_descrip = new List<string>(); // лист для конечного результата
            foreach (string s in words)
            {
                if (Array.IndexOf(prep_and_conj, s) == -1) // проверка, что слова нет в массиве с союзами и предлогами
                {
                    transform_descrip.Add(s); // добавляем не союзы и предлоги
                }
            }
            sr.Close();
			return transform_descrip;
        }

		public void GetGenres(List<string>[] genres_1)
		{
			//string[] temp = 
		}
    }
}
