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
        public Classifier()
		{
			LoadDataFromFile("../../database/tmdb_5000_movies.csv");
		}

        public void LoadDataFromFile(string file_name)
        {
            List<string> _genres = new List<string>();
            List<string> _keywords = new List<string>();
            List<string> _owerviews = new List<string>();
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
                        _genres.Add(m.Groups["genres"].ToString());
                        _keywords.Add(m.Groups["keywords"].ToString());
                        _owerviews.Add(m.Groups["owerview"].ToString());
                    }
                }
            }
			#endregion
			#region genres
			List<string>[] genres_1 = new List<string>[_genres.Count];
            var rx1 = new Regex("\"\"id\"\": [0-9]+, \"\"name\"\": \"\"(?<name>[a-zA-Z0-9 ]*)\"\"");
            int p, i = 0;
            foreach (var item in _genres)
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
			GetGenres(genres_1);
			#endregion
			#region keywords
			List<string>[] keywords_1 = new List<string>[_keywords.Count];
            i = 0;
            foreach (var item in _keywords)
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
			List<string>[] owerviews_1 = new List<string>[_owerviews.Count];
			i = 0;
			foreach(var item in _owerviews)
			{
				owerviews_1[i] = TransformOfDescrip(item);
				i++;
			}
			GetKeywords(keywords_1, owerviews_1);
			#endregion
			sr.Close();
        }

		List<string> TransformOfDescrip(string description)
        {
			string file_name = "../../Предлоги и союзы.txt";
            char[] c = { ' ', ',', '.', '!', '?', ':', ';', '(', ')', '"', '-', '\'', '_' };
            string[] words = description.Split(c); // массив слов из описания без знаков
            StreamReader sr = new StreamReader(file_name);
            string[] prep_and_conj = sr.ReadToEnd().Split('\r', '\n'); // массив предлогов и союзов из файла
            List<string> transform_descrip = new List<string>(); // лист для конечного результата
            foreach (string s in words)
            {
                if (Array.IndexOf(prep_and_conj, s) == -1) // проверка, что слова нет в массиве с союзами и предлогами
                {
					if (s.Length == 1)
					{
						if (!Char.IsLetter(s[0]))
							continue;
					}
					else
					{
						bool t = false;
						foreach (var i in s)
						{
							if (!Char.IsLetter(i))
								t = true;
						}
						if(!t)
							transform_descrip.Add(s.ToLower());
					}
                    //transform_descrip.Add(s.ToLower()); // добавляем не союзы и предлоги
                }
            }
            sr.Close();
			return transform_descrip;
        }

		public void GetGenres(List<string>[] genres_1)
		{
			List<string> temp = new List<string>();
			foreach (var item in genres_1)
			{
				temp.AddRange(item);
			}
			List<string> temp_1 = new List<string>();
			foreach (var item in temp)
			{
				if (temp_1.IndexOf(item) == -1)
					temp_1.Add(item);
			}
			temp_1.Sort();
			genres = temp_1.ToArray();
		}
		public void GetKeywords(List<string>[] keywords_1, List<string>[] owerviews_1)
		{
			List<string> temp = new List<string>();
			foreach(var item in keywords_1)
			{
				temp.AddRange(item);
			}
			foreach (var item in owerviews_1)
			{
				temp.AddRange(item);
			}
			List<string> temp_1 = temp.Distinct().ToList();
			temp_1.Sort();
			keywords = temp_1.ToArray();
		}
    }
}
