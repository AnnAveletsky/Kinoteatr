﻿
namespace Cinema.Movie.Movie.Columns
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.IO;

    [ColumnsScript("Movie.Video")]
    [BasedOnRow(typeof(Entities.VideoRow))]
    public class VideoColumns
    {
        [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
        public Int64 VideoId { get; set; }
        [EditLink]
        public String Path { get; set; }
        public Int16? Player { get; set; }
        public String PathTorrent { get; set; }
        public String Name { get; set; }
        public Int16 Translation { get; set; }
        public Int16 Season { get; set; }
        public Int16 Serie { get; set; }
        public String Storyline { get; set; }
        public DateTime PlannePublishDate { get; set; }
        public DateTime ActualPublishDateTime { get; set; }
        public Int64 MovieId { get; set; }
        public Int16 ServiceId { get; set; }
        public String ServiceName { get; set; }
    }
}