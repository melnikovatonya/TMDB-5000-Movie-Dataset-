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
			learningButton_Click(sender, e);
        }

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			List<string> owerviews_new = new List<string>();
			owerviews_new = classifier.TransformOfDescrip(OwerviewsText.Text);
			owerviews_new.AddRange(classifier.TransformOfDescrip(FilmNameText.Text));
			List<string> genres_new = classifier.GetGenresOfFilm(owerviews_new.ToArray());
			list_genres.Clear();
			foreach (var item in genres_new)
			{
				list_genres.Add(item);
			}
		}

		private void learningButton_Click(object sender, RoutedEventArgs e)
		{
			classifier = new Classifier(int.Parse(filmNumberTextBox.Text));
			learningTimeTextBlock.Text = string.Format("{0} сек.", classifier.time);
		}
	}
}
