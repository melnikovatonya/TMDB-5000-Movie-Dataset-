using System;
using System.Collections.Generic;
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
        }
		Classifier classifier;
		

		private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			classifier = new Classifier();
			foreach (var s in classifier.genres)
			{
				GenresBlock.Text += s + "\n";
			}
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			List<string> owerviews_new = new List<string>();
			owerviews_new = classifier.TransformOfDescrip(OwerviewsText.Text);
			owerviews_new.AddRange(classifier.TransformOfDescrip(FilmNameText.Text));
			List<string> genres_new = classifier.GetGenresOfFilm(owerviews_new.ToArray());
		}
	}
}
