﻿using Serenity;
using Serenity.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cinema.Movie.Configuration
{
    public partial class SettingsEditorAttribute : CustomEditorAttribute
    {
        public const string Key = "Cinema.Movie.Configuration.SettingsEditor";

        public SettingsEditorAttribute()
            : base(Key)
        {
        }
    }
}

