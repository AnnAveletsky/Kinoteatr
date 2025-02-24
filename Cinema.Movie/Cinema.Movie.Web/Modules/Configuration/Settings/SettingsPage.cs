﻿namespace Cinema.Movie.Configuration.Pages
{
    using Serenity;
    using Serenity.Web;
    using System.Web.Mvc;

    [RoutePrefix("Configuration/Settings"), Route("{action=index}")]
    public class SettingsController : Controller
    {
        [PageAuthorize("Administration")]
        public ActionResult Index()
        {
            return View("~/Modules/Configuration/Settings/SettingsIndex.cshtml");
        }
    }
}