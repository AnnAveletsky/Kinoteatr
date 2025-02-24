﻿
namespace Cinema.Movie.Forms
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.IO;

    [FormScript("Movie.ServiceRating")]
    [BasedOnRow(typeof(Entities.ServiceRatingRow))]
    public class ServiceRatingForm
    {
        public Double Rating { get; set; }
        public Int64 Id { get; set; }
        public Int64 MovieId { get; set; }
        public Int32 ServiceId { get; set; }
    }
}