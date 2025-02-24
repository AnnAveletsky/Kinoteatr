﻿
namespace Cinema.Movie.Administration.Forms
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.IO;

    [FormScript("Administration.DataBase")]
    [BasedOnRow(typeof(Entities.DataBaseRow))]
    public class DataBaseForm
    {
        public String Name { get; set; }
        public String ConnectionString { get; set; }
        public String ProviderName { get; set; }
        public Boolean Active { get; set; }
        public String TagDataBaseMovie { get; set; }
    }
}