﻿using Serenity;
using Serenity.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cinema.Movie.Movie
{
    public partial class CastMoviePersonEditorAttribute : CustomEditorAttribute
    {
        public const string Key = "Cinema.Movie.Movie.CastMoviePersonEditor";

        public CastMoviePersonEditorAttribute()
            : base(Key)
        {
        }
    }
}

