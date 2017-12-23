using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TMDB_5000_Movie_Dataset
{
    public class Classifier
    {
        public string[] genres;
        string[] keywords;
        int[,] frequencies;
		double[,] probability;
		double[] prob_genres;
		const double p = 0.5;
        public Classifier()
		{
			LoadDataFromFile("../../database/tmdb_5000_movies.csv");
			PrintData();
		}
		void PrintData()
		{
			var sw = new StreamWriter("genres.txt");
			for (int i = 0; i < genres.Length; i++)
			{
				sw.WriteLine(genres[i]);
			}
			sw.Close();
			sw = new StreamWriter("keywords.txt");
			for (int i = 0; i < keywords.Length; i++)
			{
				sw.WriteLine(keywords[i]);
			}
			sw.Close();
			sw = new StreamWriter("frequencies.txt");
			for (int i = 0; i < frequencies.GetLength(0); i++)
			{
				string s = "";
				for (int j = 0; j < frequencies.GetLength(1); j++)
				{
					s += string.Format("{0,3} ", frequencies[i, j]);
				}
				sw.WriteLine(s);
			}
			sw.Close();
			sw = new StreamWriter("probability.txt");
			for (int i = 0; i < probability.GetLength(0); i++)
			{
				string s = "";
				for (int j = 0; j < probability.GetLength(1); j++)
				{
					s += string.Format("{0,8} ", probability[i, j]);
				}
				sw.WriteLine(s);
			}
			sw.Close();
			sw = new StreamWriter("prob_genres.txt");
			for (int i = 0; i < prob_genres.Length; i++)
			{
				sw.WriteLine(prob_genres[i]);
			}
			sw.Close();
		}
        public void LoadDataFromFile(string file_name)
			//метод загрузки данных и их преобразования
        {
            List<string> _genres = new List<string>();
            List<string> _keywords = new List<string>();
            List<string> _owerviews = new List<string>();
            StreamReader sr = new StreamReader(file_name);
			#region regex
			var rx = new Regex("[0-9]+,(?<genres>\"*\\[.*\\]\"*),(?<site>.*),(?<id>[0-9]+),(?<keywords>\"*\\[.*\\]\"*),(?<lang>[a-z]+),(?<title>[^,]*),(?<owerview>\"*.*\"*),[0-9]+(\\.)*[0-9]*,\"*\\[.*\\]\"*,\"*\\[.*\\]\"*");
            {
                string s;
				int t = 1;
                while (t != 2000)
                {
					t++;
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
			GetGenresProbability(genres_1);
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
			GetFrequencies(genres_1, keywords_1, owerviews_1);
			GetProbability();
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
			//метод выделения массива жанров
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
			//метод выделения массива ключевых слов без повторений
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

		public void GetFrequencies(List<string>[] genres_1, List<string>[] keywords_1, List<string>[] owerviews_1)
		{
			frequencies = new int[genres.Length, keywords.Length];
			
			for (int t = 0; t < genres_1.Length; t++)
			{
				foreach(var item2 in genres_1[t])
				{
					int indexGenres = Array.IndexOf(genres, item2);
					 foreach(var item3 in keywords_1[t])
					{
						int indexKeywords = Array.IndexOf(keywords, item3);
						frequencies[indexGenres, indexKeywords] += 1;
					}

					foreach (var item3 in owerviews_1[t])
					{
						int indexOwerviews = Array.IndexOf(keywords, item3);
						frequencies[indexGenres, indexOwerviews] += 1;
					}
				}
			}

		}
		public void GetProbability()
		{
			probability = new double[frequencies.GetLength(0), frequencies.GetLength(1)];
			for (int i = 0; i < genres.Length; i++)
			{
				int sum = 0;
				for (int j = 0; j < frequencies.GetLength(1); j++)
				{
					sum += frequencies[i, j];
				}
				for (int j = 0; j < frequencies.GetLength(1); j++)
				{
					probability[i, j] = (frequencies[i, j] + 1) / (sum + keywords.Length);
				}
			}
		}
		public List<string> GetGenresOfFilm(string[] _keywords) // метод классификации 
		{
			List<string> _genres = new List<string>();

			for (int i = 0; i < genres.Length; i++)
			{
				double _p = Math.Log(prob_genres[i]); // логарифм вероятности жанра
				for (int j = 0; j < _keywords.Length; j++) // сумма логарифмов вероятностей жанров для каждого ключевого слова
				{
					int k = Array.IndexOf(keywords, _keywords[j]);
					if (k != -1)
					{
						if (probability[i,k] != 0)
						{
							_p += Math.Log(probability[i,k]);
						}
					}
				}
				if (_p >= p) // если вероятность больше константы то жанр подходит
					_genres.Add(genres[i]);
			}

			return _genres;
		}

		public void GetGenresProbability(List<string>[] _genres) // метод получения вероятностей жанров
		{
			prob_genres = new double[genres.Length];
			for (int i = 0; i < genres.Length; i++)
			{
				int k = 0;
				foreach (var item in _genres) // количество фильмов для каждого жанра
				{
					if (item.IndexOf(genres[i]) != -1)
						k += 1;
				}
				prob_genres[i] = (double)k / _genres.Length; //деленое на общее количество  фильмов
			}
		}

	}
}
