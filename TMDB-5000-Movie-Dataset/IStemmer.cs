﻿/*
 *  Port of Snowball stemmers on C#
 *  Original stemmers can be found on http://snowball.tartarus.org
 *  Licence still BSD: http://snowball.tartarus.org/license.php
 *  
 *  Most of stemmers are ported from Java by Iveonik Systems ltd. (www.iveonik.com)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMDB_5000_Movie_Dataset
{
    public interface IStemmer
    {
        string Stem(string s);
    }
}
