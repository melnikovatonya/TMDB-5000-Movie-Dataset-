using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TMDB_5000_Movie_Dataset
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
			list_genres = new ObservableCollection<string>();
			genresListBox.ItemsSource = list_genres;
        }
		Classifier classifier;
		ObservableCollection<string> list_genres;

		private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			var message = MessageBox.Show("Загрузить готовый классификатор?", "", MessageBoxButton.YesNo);
			if(message == MessageBoxResult.Yes)
			{
				learningButton1_Click(sender, e);
			}
			else
			{
				learningButton_Click(sender, e);
			}
        }

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			List<string> owerviews_new = new List<string>();
			owerviews_new = classifier.TransformOfDescrip(OwerviewsText.Text);
			owerviews_new.AddRange(classifier.TransformOfDescrip(FilmNameText.Text));
			List<KeyValuePair<double, string>> genres_new = classifier.GetGenresOfFilm(owerviews_new.ToArray());
			list_genres.Clear();
			foreach (var item in genres_new)
			{
				string temp = string.Format("{0} {1}", item.Value, item.Key);
				list_genres.Add(temp);
			}
		}

		private void learningButton_Click(object sender, RoutedEventArgs e)
		{
			classifier = new Classifier(int.Parse(filmNumberTextBox.Text), 0);
			learningTimeTextBlock.Text = string.Format("{0} сек.", classifier.time);
		}

		private void learningButton1_Click(object sender, RoutedEventArgs e)
		{
			classifier = new Classifier(int.Parse(filmNumberTextBox.Text), 1);
			learningTimeTextBlock.Text = string.Format("{0} сек.", classifier.time);
		}
	}
}
