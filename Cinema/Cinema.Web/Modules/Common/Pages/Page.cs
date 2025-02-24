﻿
namespace Cinema.Common.Pages
{
    using Serenity;
    using Serenity.Data;
    using System;
    using System.Web.Mvc;
    using Serenity.Services;
    using Movie.Entities;
    using System.Collections.Generic;
    using Movie.Pages;
    using Movie;
    using Administration.Entities;
    using Administration.Pages;

    public class PageController : Controller
    {
        #region set public metod
        public static SaveResponse SetPageMovie(RetrieveResponse<MovieRow> movie)
        {
            return MovieController.ViewsAdd(new SaveRequest<MovieRow>() { Entity = movie.Entity, EntityId = movie.Entity.MovieId });
        }
        #endregion

        public static RetrieveResponse<SiteRow> GetSite(string url)
        {
            return TwoLevelCache.GetLocalStoreOnly("Site"+ url, TimeSpan.FromMinutes(5),
                SiteRow.Fields.GenerationKey, () =>
                {
                    return SiteController.Find(new ListRequest() { Criteria = new Criteria("Url") == url });
                });
        }
        #region get public metod
        public static RetrieveResponse<MovieRow> GetPageMovie(Int64? id, string movie = "")
        {
            var _movie = GetMovie(id, movie);
            if (movie != null)
            {
                _movie.Entity.ServiceRatings = GetServiceRatings(_movie.Entity);
                _movie.Entity.MovieGenres = GetMovieGenres(_movie.Entity);
                _movie.Entity.MovieCountries = GetMovieCountries(_movie.Entity);
                _movie.Entity.Casts = GetCasts(_movie.Entity);
                _movie.Entity.MovieTags = GetTags(_movie.Entity);
                _movie.Entity.Services = GetServicesVideos(_movie.Entity);
            }
            return _movie;
        }
        public static ListResponse<MovieRow> GetPageMovies(int count = 10, int page = 1, string movie = "", string year1 = "", string year2 = "", string year3 = "")
        {
            var movies = GetMovies(count, page, movie, year1, year2, year3);
            movies.Entities.ForEach(i =>
            {
                i.ServiceRatings = GetServiceRatings(i);
                i.MovieGenres = GetMovieGenres(i);
                i.MovieCountries = GetMovieCountries(i);
            });
            return movies;
        }
        public static ListResponse<GenreRow> GetPageGenres()
        {
            return TwoLevelCache.GetLocalStoreOnly("Genres", TimeSpan.FromMinutes(5),
                GenreRow.Fields.GenerationKey, () =>
                {
                    return GenreController.List(new ListRequest());
                });
        }
        
       public static ListResponse<MovieRow> GetPagePopularMovies()
        {
            return MovieController.List(new ListRequest()
            {
                Take = 20,
                Criteria = new Criteria("Kind") == MovieKind.Film,
                ExcludeColumns = new HashSet<string>() { "Description", "Runtime", "CreateDateTime", "PublishDateTime", "Mpaa", "Nice", "ContSeason", "Tagline", "Budget", "GenreList", "GenreListName", "TagList", "TagListName", "UpdateDateTime" },
                Sort = new[] {
                    new SortBy("Rating", true)
                }
            });
        }
        public static ListResponse<MovieRow> GetPageNewSeries() {
            return MovieController.List(new ListRequest()
            {
                Take = 20,
                Criteria = new Criteria("Kind") == MovieKind.TvSeries,
                ExcludeColumns = new HashSet<string>() { "Description", "Runtime", "CreateDateTime", "PublishDateTime", "Mpaa", "Nice", "ContSeason", "Tagline", "Budget", "GenreList", "GenreListName", "TagList", "TagListName", "Rating" },
                Sort = new[] {
                    new SortBy("UpdateDateTime", true)
                }
            });
        }
        public static ListResponse<MovieRow> GetPagePopularSeries()
        {
            return MovieController.List(new ListRequest()
            {
                Take = 20,
                Criteria = new Criteria("Kind") == MovieKind.TvSeries,
                ExcludeColumns = new HashSet<string>() { "Description", "ReleaseWorldDate", "ReleaseOtherDate", "ReleaseDvd", "Runtime", "CreateDateTime", "PublishDateTime", "Mpaa", "Nice", "ContSeason", "Tagline", "Budget", "GenreList", "GenreListName", "TagList", "TagListName", "UpdateDateTime" },
                Sort = new[] {
                    new SortBy("Rating", true)
                }
            });
        }
        public static ListResponse<MovieRow> GetPageSimilarMovies(MovieRow movie) {
            return MovieController.List(new ListRequest
            {
                Take = 6,
                IncludeColumns = new HashSet<string>() { "GenreList" },
                Criteria = new Criteria("Kind") == MovieKind.Film,
                ExcludeColumns = new HashSet<string>() { "Description", "ReleaseWorldDate", "ReleaseOtherDate", "ReleaseDvd", "Runtime", "CreateDateTime", "PublishDateTime", "Mpaa", "Nice", "ContSeason", "Tagline", "Budget", "GenreList", "GenreListName", "TagList", "TagListName" },
                Sort = new[] {
                        new SortBy("UpdateDateTime", true),
                        new SortBy("PublishDateTime", true),
                        new SortBy("Rating", true),
                        new SortBy("TitleOriginal"),
                        new SortBy("TitleTranslation")
                    }
            });
        }
        public static ListResponse<MovieRow> GetPageSimilarSeries(MovieRow movie)
        {
            return MovieController.List(new ListRequest
            {
                Take = 6,
                IncludeColumns = new HashSet<string>() { "GenreList" },
                Criteria = new Criteria("Kind") == MovieKind.TvSeries,
                ExcludeColumns = new HashSet<string>() { "Description", "ReleaseWorldDate", "ReleaseOtherDate", "ReleaseDvd", "Runtime", "CreateDateTime", "PublishDateTime", "Mpaa", "Nice", "ContSeason", "Tagline", "Budget", "GenreList", "GenreListName", "TagList", "TagListName" },
                Sort = new[] {
                        new SortBy("UpdateDateTime", true),
                        new SortBy("PublishDateTime", true),
                        new SortBy("Rating", true),
                        new SortBy("TitleOriginal"),
                        new SortBy("TitleTranslation")
                    }
            });
        }
        #endregion
        #region get private metod
        static ListResponse<MovieRow> GetMovies(int count = 10, int page = 1, string movie = "", string year1 = "", string year2 = "", string year3 = "")
        {
            var lr = new ListRequest
            {
                Skip = (page - 1) * count,
                Take = count,
                Sort = new[] {
                    new SortBy("UpdateDateTime", true),
                    new SortBy("PublishDateTime", true),
                    new SortBy("Rating", true),
                    new SortBy("TitleOriginal"),
                    new SortBy("TitleTranslation") }
            };
            if (year1 != "" || year2 != "" || year3 != "" || movie != "")
            {
                lr.Criteria = null;
                int y1 = 0, y2 = 0, y3 = 0;
                List<int> list = new List<int>();
                int.TryParse(year1, out y1);
                int.TryParse(year2, out y2);
                int.TryParse(year3, out y3);
                if (y1 != 0)
                {
                    list.Add(y1);
                }
                if (y2 != 0)
                {
                    list.Add(y2);
                }
                if (y3 != 0)
                {
                    list.Add(y3);
                }
                if (list.Count != 0)
                {
                    lr.Criteria = new Criteria("YearStart").In(list.ToArray()) || new Criteria("YearEnd").In(list.ToArray());
                    if (movie != "")
                    {
                        lr.Criteria = lr.Criteria && (new Criteria("TitleOriginal").Contains(movie) || new Criteria("TitleTranslation").Contains(movie));
                    }
                }
                else if (movie != "" && list.Count == 0)
                {
                    lr.Criteria = new Criteria("TitleOriginal").Contains(movie) || new Criteria("TitleTranslation").Contains(movie);
                }
            }
            return TwoLevelCache.GetLocalStoreOnly("Movies_" + count + "_" + page + "_" + year1 + "_" + year2 + "_" + year3, TimeSpan.FromMinutes(5),
                MovieRow.Fields.GenerationKey, () =>
                {
                    return MovieController.List(lr);
                });
        }
        static ListResponse<ServiceRatingRow> GetServiceRatings(MovieRow movie)
        {
            return TwoLevelCache.GetLocalStoreOnly("ServiceRatings_" + movie.MovieId, TimeSpan.FromMinutes(5), ServiceRatingRow.Fields.GenerationKey, () =>
            {
                return ServiceRatingController.List(new ListRequest()
                {
                    IncludeColumns = new HashSet<string>()
                        {
                            ServiceRatingRow.Fields.MovieId.Name,
                            ServiceRatingRow.Fields.ServiceId.Name,
                            ServiceRatingRow.Fields.ServiceUrl.Name,
                            ServiceRatingRow.Fields.ServiceName.Name,
                            ServiceRatingRow.Fields.Rating.Name,
                            ServiceRatingRow.Fields.Id.Name
                        },
                    Criteria = new Criteria(ServiceRatingRow.Fields.MovieId.Name) == (Int64)movie.MovieId
                });
            });
        }
        static ListResponse<MovieGenreRow> GetMovieGenres(MovieRow movie)
        {
            return TwoLevelCache.GetLocalStoreOnly("Genres_" + movie.MovieId, TimeSpan.FromMinutes(5), MovieGenreRow.Fields.GenerationKey, () =>
            {
                return MovieGenreController.List(new ListRequest()
                {
                    IncludeColumns = new HashSet<string>()
                    {
                        MovieGenreRow.Fields.MovieId.Name,
                        MovieGenreRow.Fields.GenreName.Name
                    },
                    Criteria = new Criteria(MovieGenreRow.Fields.MovieId.Name) == (Int64)movie.MovieId
                });
            });
        }
        static ListResponse<MovieCountryRow> GetMovieCountries(MovieRow movie)
        {
            return TwoLevelCache.GetLocalStoreOnly("Countries_" + movie.MovieId, TimeSpan.FromMinutes(5), MovieCountryRow.Fields.GenerationKey, () =>
            {
                return MovieCountryController.List(new ListRequest()
                {
                    IncludeColumns = new HashSet<string>()
                     {
                        MovieCountryRow.Fields.MovieId.Name,
                        MovieCountryRow.Fields.CountryName.Name,
                        MovieCountryRow.Fields.CountryNameOther.Name,
                        MovieCountryRow.Fields.CountryCode.Name
                     },
                    Criteria = new Criteria(MovieCountryRow.Fields.MovieId.Name) == (Int64)movie.MovieId
                });
            });
        }
        static ListResponse<ServiceRow> GetServices()
        {
            return TwoLevelCache.GetLocalStoreOnly("Services", TimeSpan.FromMinutes(5), ServiceRow.Fields.GenerationKey, () =>
            {
                return ServiceController.List(new ListRequest() {});
            });
        }
        static ListResponse<ServiceRow> GetServicesVideos(MovieRow movie)
        {
           return TwoLevelCache.GetLocalStoreOnly("GetServicesVideos_" + movie.MovieId, TimeSpan.FromMinutes(5), VideoRow.Fields.GenerationKey, () =>
            {
                var services = new ListResponse<ServiceRow>();
                services.Entities = new List<ServiceRow>();
                var all= GetServices();
                all.Entities.ForEach(i =>
                {
                    i.Videos = GetVideos(movie, i);
                    if (i.Videos.Entities.Count > 0)
                    {
                        services.Entities.Add(i);
                    }
                });
                return services;
            });
        }
        static ListResponse<VideoRow> GetVideos(MovieRow movie, ServiceRow service)
        {
            return TwoLevelCache.GetLocalStoreOnly("Video_" + movie.MovieId+"_"+ service.ServiceId, TimeSpan.FromMinutes(5), VideoRow.Fields.GenerationKey, () =>
            {
                return VideoController.List(new ListRequest()
                {
                    IncludeColumns = new HashSet<string>()
                    {
                        VideoRow.Fields.MovieId.Name,
                        VideoRow.Fields.ServiceId.Name,
                    },
                    Sort=new SortBy[] {
                        new SortBy(VideoRow.Fields.Season.Name),
                        new SortBy(VideoRow.Fields.Serie.Name)
                    },
                    Criteria = new Criteria(VideoRow.Fields.MovieId.Name) == (Int64)movie.MovieId && new Criteria(VideoRow.Fields.ServiceId.Name)==(Int32)service.ServiceId
                });
            });
        }
        static ListResponse<CastRow> GetCasts(MovieRow movie)
        {
            return TwoLevelCache.GetLocalStoreOnly("Casts_" + movie.MovieId, TimeSpan.FromMinutes(5), CastRow.Fields.GenerationKey, () =>
            {
                return CastController.List(new ListRequest()
                {
                    Sort = new SortBy[] { new SortBy(CastRow.Fields.CharacterOther.Name) },
                    IncludeColumns = new HashSet<string>()
                    {
                        CastRow.Fields.MovieId.Name,
                        CastRow.Fields.PersonName.Name,
                       CastRow.Fields.PersonNameOther.Name,
                       CastRow.Fields.PersonUrl.Name
                    },
                    Criteria = new Criteria(CastRow.Fields.MovieId.Name) == (Int64)movie.MovieId
                });
            });
        }
        static ListResponse<MovieTagRow> GetTags(MovieRow movie)
        {
            return TwoLevelCache.GetLocalStoreOnly("Tags_" + movie.MovieId, TimeSpan.FromMinutes(5), MovieTagRow.Fields.GenerationKey, () =>
            {
                return MovieTagController.List(new ListRequest()
                {
                    IncludeColumns = new HashSet<string>()
                    {
                        MovieTagRow.Fields.MovieId.Name,
                        MovieTagRow.Fields.TagName.Name
                    },
                    Criteria = new Criteria(MovieTagRow.Fields.MovieId.Name) == (Int64)movie.MovieId
                });
            });
        }
        static RetrieveResponse<MovieRow> GetMovie(Int64? id, string movie = "")
        {
            if (id != null)
            {
                return TwoLevelCache.GetLocalStoreOnly("Movie_" + id, TimeSpan.FromMinutes(5), MovieRow.Fields.GenerationKey, () =>
                {
                    return MovieController.Retrieve(new RetrieveRequest()
                    {
                        EntityId = (Int64)id
                    });
                });
            }
            else if (movie != "")
            {
                return TwoLevelCache.GetLocalStoreOnly("Movie_" + movie, TimeSpan.FromMinutes(5), MovieRow.Fields.GenerationKey, () =>
                {
                    return MovieController.Find(new ListRequest()
                    {
                        Criteria = MovieController.Criteria(new SaveRequest<MovieRow>()
                        {
                            Entity = new MovieRow() { Url = movie }
                        })
                    });
                });
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
