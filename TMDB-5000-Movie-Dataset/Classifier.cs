using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMDB_5000_Movie_Dataset
{
    class Classifier
    {
        List<string> genres = new List<string>();
        List<string> keywords = new List<string>();
        int[] frequencies;
        Classifier() { }
    }
}
