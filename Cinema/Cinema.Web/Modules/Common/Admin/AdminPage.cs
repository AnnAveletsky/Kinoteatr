﻿
namespace Cinema.Common.Pages
{
    using Serenity;
    using System;
    using System.Web.Mvc;

    [RoutePrefix("Admin"), Route("~/admin/{action=index}")]
    public class AdminController : Controller
    {
        PageModel model = new PageModel();
        [HttpGet, Route("~/admin/")]
        public ActionResult Index(int count = 10, int page = 1, string movie = "", string year1 = "", string year2 = "", string year3 = "")
        {
            model.Movies = PageController.GetPageMovies(count, page, movie, year1, year2, year3);
            model.Genres = PageController.GetPageGenres();
            model.PopularMovies = PageController.GetPagePopularMovies();
            model.PopularSeries = PageController.GetPagePopularSeries();
            model.NewSeries = PageController.GetPageNewSeries();
            ViewData["Page"] = page;
            ViewData["Count"] = count;
            ViewData["Url"] = (movie != "" ? "&&movie=" : "") + movie + (year1 != "" ? "&&year1=" + year1 : "") + (year2 != "" ? "&&year2=" + year2 : "") + (year3 != "" ? "&&year3=" + year3 : "");
            ViewData["Title"] = LocalText.Get("Site.Dashboard.ResultSearch");
            ViewData["Footer"] = "";
            ViewData["TypePage"] = TypePage.Movies;
            ViewData["PageId"] = "Dashboard/Dashboard";
            ViewData["Admin"] = TypePage.Admin;
            return View(MVC.Views.Common.Pages.Index, model);
        }
        
    }
}
