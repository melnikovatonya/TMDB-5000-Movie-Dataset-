using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TMDB_5000_Movie_Dataset
{
    public class Classifier
    {
        string[] genres;
        string[] keywords;
        int[,] frequencies;
		double[,] probability;
		double[] prob_genres;
		public double time;
		EnglishStemmer en_stem = new EnglishStemmer();
		int film_num;
        public Classifier(int film_number, int mode)
		{
			film_num = film_number;
			if (mode == 0)
				LoadDataFromFile("../../database/tmdb_5000_movies.csv", film_num);
			else
				LoadDataFromFile1();
			Task task = PrintData();
		}
		Task PrintData()
		{
			Task task = new Task(() =>
			{
				var sw = new StreamWriter("genres.txt");
				sw.WriteLine(genres.Length);
				for (int i = 0; i < genres.Length; i++)
				{
					sw.WriteLine(genres[i]);
				}
				sw.Close();
				sw = new StreamWriter("keywords.txt");
				sw.WriteLine(keywords.Length);
				for (int i = 0; i < keywords.Length; i++)
				{
					sw.WriteLine(keywords[i]);
				}
				sw.Close();
				sw = new StreamWriter("frequencies.txt");
				for (int i = 0; i < frequencies.GetLength(1); i++)
				{
					string s = "";
					for (int j = 0; j < frequencies.GetLength(0); j++)
					{
						s += string.Format("{0} ", frequencies[j, i].ToString());
					}
					sw.WriteLine(s);
				}
				sw.Close();
				////var bw = new BinaryWriter(File.OpenWrite("prob_genres.bin"));
				////for (int i = 0; i < prob_genres.Length; i++)
				////{
				////	bw.Write(prob_genres[i]);
				////}
				////bw.Close();
				////bw = new BinaryWriter(File.OpenWrite("probability.bin"));
				////for (int i = 0; i < probability.GetLength(0); i++)
				////{
				////	string s = "";
				////	for (int j = 0; j < probability.GetLength(1); j++)
				////	{
				////		s += string.Format("{0,8} ", probability[i, j]);
				////	}
				////	bw.Write(s);
				////}
				////bw.Close();
				sw = new StreamWriter("prob_genres.txt");
				for (int i = 0; i < prob_genres.Length; i++)
				{
					sw.WriteLine(prob_genres[i].ToString());

				}
				sw.Close();
				sw = new StreamWriter("probability.txt");
				for (int i = 0; i < probability.GetLength(1); i++)
				{
					string s = "";
					for (int j = 0; j < probability.GetLength(0); j++)
					{
						s += string.Format("{0} ", probability[j, i]);
					}
					sw.WriteLine(s);

				}
				sw.Close();
			});
			task.Start();
			return task;
		}
		public void LoadDataFromFile1()
		{
			Stopwatch _time = new Stopwatch();
			_time.Start();
			try
			{
				int g_l = 0, k_l = 0;
				string[] temp = File.ReadAllLines("genres.txt");
				string[] temp1 = File.ReadAllLines("keywords.txt");
				g_l = int.Parse(temp[0]);
				k_l = int.Parse(temp1[0]);
				if (g_l == temp.Length - 1 && k_l == temp1.Length - 1)
				{
					var temp_ = temp.ToList();
					temp_.RemoveAt(0);
					genres = temp_.ToArray();
					temp_ = temp1.ToList();
					temp_.RemoveAt(0);
					keywords = temp_.ToArray();

					temp = File.ReadAllLines("frequencies.txt");
					var temp_1 = temp[0].Split(new char[] { ' ' });
					temp1 = File.ReadAllLines("probability.txt");
					var temp_2 = temp1[0].Split(new char[] { ' ' });
					if(temp.Length == k_l && temp_1.Length - 1 == g_l && temp1.Length == k_l && temp_2.Length - 1 == g_l)
					{
						frequencies = new int[g_l, k_l];
						probability = new double[g_l, k_l];
						for (int i = 0; i < k_l; i++)
						{
							temp_1 = temp[i].Split(new char[] { ' ' });
							temp_2 = temp1[i].Split(new char[] { ' ' });
							for (int j = 0; j < g_l; j++)
							{
								frequencies[j, i] = int.Parse(temp_1[j]);
								probability[j, i] = double.Parse(temp_2[j]);
							}
						}

						temp = File.ReadAllLines("prob_genres.txt");
						if(temp.Length == g_l)
						{
							prob_genres = new double[g_l];
							for (int i = 0; i < g_l; i++)
							{
								prob_genres[i] = double.Parse(temp[i]);
							}
						}
					}
				}
				else
				{
					MessageBox.Show("Ошибка данных!");
					LoadDataFromFile("../../database/tmdb_5000_movies.csv", film_num);
				}
			}
			catch
			{
				MessageBox.Show("Ошибка данных!");
				LoadDataFromFile("../../database/tmdb_5000_movies.csv", film_num);
			}
			_time.Stop();
			time = _time.ElapsedMilliseconds / 1000.0;
		}

		public void LoadDataFromFile(string file_name, int film_number)
			//метод загрузки данных и их преобразования
        {
			Stopwatch _time = new Stopwatch();
			_time.Start();

            List<string> _genres = new List<string>();
            List<string> _keywords = new List<string>();
            List<string> _owerviews = new List<string>();
            StreamReader sr = new StreamReader(file_name);
			#region regex
			var rx = new Regex("[0-9]+,(?<genres>\"*\\[.*\\]\"*),(?<site>.*),(?<id>[0-9]+),(?<keywords>\"*\\[.*\\]\"*),(?<lang>[a-z]+),(?<title>[^,]*),(?<owerview>\"*.*\"*),[0-9]+(\\.)*[0-9]*,\"*\\[.*\\]\"*,\"*\\[.*\\]\"*");
            {
                string s;
				int t = 1;
                while (t != film_number)
                {
					t++;
                    s = sr.ReadLine();
                    var m = rx.Match(s);
                    if (m.Success)
                    {
                        _genres.Add(m.Groups["genres"].ToString().ToLower());
                        _keywords.Add(m.Groups["keywords"].ToString().ToLower());
                        _owerviews.Add(m.Groups["owerview"].ToString().ToLower());
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
                        keywords_1[i].AddRange(TransformOfDescrip(m.Groups["name"].ToString()));
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

			_time.Stop();
			time = _time.ElapsedMilliseconds / 1000.0;
        }

		public List<string> TransformOfDescrip(string description)
        {
			string file_name = "../../Предлоги и союзы.txt";
            char[] c = { ' ', ',', '.', '!', '?', ':', ';', '(', ')', '"', '-', '\'', '_' };
            string[] words = description.Split(c); // массив слов из описания без знаков
												   //StreamReader sr = new StreamReader(file_name);
												   //string[] prep_and_conj = sr.ReadToEnd().Split('\r', '\n'); // массив предлогов и союзов из файла
			string[] prep_and_conj = File.ReadAllLines(file_name);
            List<string> transform_descrip = new List<string>(); // лист для конечного результата
            foreach (string s in words)
            {
                if (Array.IndexOf(prep_and_conj, s) == -1) // проверка, что слова нет в массиве с союзами и предлогами
                {
					if (s.Length == 0)
						continue;
					else if (s.Length == 1)
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

			string[] temp = transform_descrip.ToArray();
			for (int i = 0; i < temp.Length; i++)
			{
				temp[i] = en_stem.Stem(temp[i]);
			}
			transform_descrip = temp.Distinct().ToList();
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
				foreach (var item1 in item)
				{
					temp.Add(en_stem.Stem(item1));
				}
				//temp.AddRange(item);
			}
			foreach (var item in owerviews_1)
			{
				foreach (var item1 in item)
				{
					temp.Add(en_stem.Stem(item1));
				}
				//temp.AddRange(item);
			}
			List<string> temp_1 = temp.Distinct().ToList();
			temp_1.Sort();
			keywords = temp_1.ToArray();
		}

		private int BinarySearch(string x)
		{
			int first = 0;
			int last = keywords.Length;

			while (first < last)
			{
				int mid = first + (last - first) / 2;

				if (String.Compare(x, keywords[mid]) <= 0)
					last = mid;
				else
					first = mid + 1;
			}
			if (keywords[last] == x)
				return last;
			else
				return 0;
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
						//int indexKeywords = Array.IndexOf(keywords, item3);
						int indexKeywords = BinarySearch(item3);
						frequencies[indexGenres, indexKeywords] += 1;
					}

					foreach (var item3 in owerviews_1[t])
					{
						//int indexOwerviews = Array.IndexOf(keywords, item3);
						int indexOwerviews = BinarySearch(item3);
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
					probability[i, j] = (double)(frequencies[i, j] + 1) / (sum + keywords.Length);
				}
			}
		}
		public List<KeyValuePair<double, string>> GetGenresOfFilm(string[] _keywords) // метод классификации 
		{
			List<KeyValuePair<double,string>> _genres = new List<KeyValuePair<double, string>>();
			List<KeyValuePair<double, string>> temp = new List<KeyValuePair<double, string>>();
			for (int i = 0; i < genres.Length; i++)
			{
				double _p = prob_genres[i]; // вероятность жанра
				for (int j = 0; j < _keywords.Length; j++) // произведение вероятностей жанров для каждого ключевого слова
				{
					int k = Array.IndexOf(keywords, _keywords[j]);
					if (k != -1)
					{
						_p *= probability[i,k];
					}
				}
				temp.Add(new KeyValuePair<double, string>(_p, genres[i]));
			}
			double ss = 0;
			foreach (var item in temp)
			{
				string s = item.Key.ToString();
				if(s.IndexOf('E') != -1)
				{
					var s_ = Math.Abs(int.Parse(s.Substring(s.IndexOf('E') + 1)));
					ss += s_;
				}
				else
				{
					var s_ = s.Substring(s.IndexOf('.'));
					int i = 0;
					for (int k = 0; k < s_.Length; k++)
					{
						if (s_[k] == 0)
							k++;
						else
							break;
					}
					ss += i + 1;
				}
			}
			ss /= (double)genres.Length;
			double d = 5 * Math.Pow(10, -(int)ss); 
			foreach (var item in temp)
			{
				if (item.Key >= d)
					_genres.Add(item);
			}
			_genres.Sort(Compare1);
			return _genres;
		}
		static int Compare1(KeyValuePair<double, string> b, KeyValuePair<double, string> a)
		{
			return a.Key.CompareTo(b.Key);
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
			var j = prob_genres.Sum();
		}

	}
}
