﻿

[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Movie/ServicePath", typeof(Cinema.Movie.Pages.ServicePathController))]

namespace Cinema.Movie.Pages
{
    using Common.Init;
    using Entities;
    using Serenity;
    using Serenity.Services;
    using Serenity.Web;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Repository = Repositories.ServicePathRepository;
    using MyRow = Entities.ServicePathRow;
    using System.Xml;
    using System.Xml.Serialization;
    using Serenity.Data;

    [RoutePrefix("Movie/ServicePath"), Route("{action=index}")]
    public class ServicePathController : Controller
    {
        [PageAuthorize("Administration")]
        public ActionResult Index()
        {
            return View("~/Modules/Movie/ServicePath/ServicePathIndex.cshtml");
        }

        public static RetrieveResponse<MyRow> Find(ListRequest listRequest)
        {
            using (var connection = SqlConnections.NewFor<MyRow>())
            {
                return new Repository().Find(connection, listRequest);
            }
        }
        public static RetrieveResponse<MyRow> Retrieve(RetrieveRequest retrieveRequest)
        {
            using (var connection = SqlConnections.NewFor<MyRow>())
            {
                return new Repository().Retrieve(connection, retrieveRequest);
            }
        }
        public static ListResponse<MyRow> List(ListRequest listRequest)
        {
            using (var connection = SqlConnections.NewFor<MyRow>())
            {
                return new Repository().List(connection, listRequest);
            }
        }
        public static SaveResponse UpdateCreate(SaveRequest<MyRow> saveRequest)
        {
            using (var connection = SqlConnections.NewFor<MyRow>())
            using (var uow = new UnitOfWork(connection))
            {
                var result = new Repository().UpdateCreate(uow, saveRequest);
                uow.Commit();
                return result;
            }
        }
        [PageAuthorize("Administration")]
        public ActionResult Parse(Int32 idServicePath)
        {
            try
            {
                RetrieveResponse<MyRow> servicePath = Retrieve(new RetrieveRequest()
                {
                    EntityId = idServicePath,
                    IncludeColumns = new HashSet<string>() { MyRow.Fields.ServiceId.ToString() }
                });
                XmlSerializer serializer = new XmlSerializer(typeof(Content));
                using (XmlReader reader = XmlReader.Create(servicePath.Entity.Path))
                {
                    Content obj = (Content)serializer.Deserialize(reader);
                    foreach (MovieJson item in obj.All(servicePath.Entity.Kind).Entities)
                    {
                        if (item != null)
                        {
                            try
                            {
                                var movieRow = item.ToMovie();
                                var serviceRaiting = ServiceRatingController.Find(new ListRequest()
                                {
                                    IncludeColumns=new HashSet<string>()
                                    {
                                        ServiceRatingRow.Fields.MovieId.Name,
                                        ServiceRatingRow.Fields.ServiceId.Name,
                                        ServiceRatingRow.Fields.Id.Name
                                    },
                                    Criteria = new Criteria(ServiceRatingRow.Fields.Id.Name) == item.kinopoisk_id && new Criteria(ServiceRatingRow.Fields.ServiceId.Name) == (Int32)servicePath.Entity.ServiceId
                                });
                                if (item.kinopoisk_id != ""&& serviceRaiting!=null&& serviceRaiting.Entity!=null&& serviceRaiting.Entity.MovieId!=null)
                                {
                                    movieRow.MovieId = serviceRaiting.Entity.MovieId;
                                }
                                //movie
                                var movie = MovieController.UpdateCreate(new SaveRequest<MovieRow>()
                                {
                                    Entity = movieRow
                                });
                                //video
                                var video = VideoController.UpdateCreate(new SaveRequest<VideoRow>()
                                {
                                    Entity = item.ToVideo(movie, servicePath)
                                });
                                //persons and casts
                                foreach (var person in item.ToPersons())
                                {
                                    person.PersonId = Int64.Parse(PersonController.UpdateCreate(new SaveRequest<PersonRow>()
                                    {
                                        Entity = person
                                    }).EntityId.ToString());
                                    foreach (var cast in item.ToCast(movie, person))
                                    {
                                        CastController.UpdateCreate(new SaveRequest<CastRow>()
                                        {
                                            Entity = cast
                                        });
                                    }
                                }
                                //genres
                                foreach (var genre in item.ToGenres())
                                {
                                    MovieGenreController.UpdateCreate(new SaveRequest<MovieGenreRow>()
                                    {
                                        Entity = new MovieGenreRow()
                                        {
                                            MovieId = Int64.Parse(movie.EntityId.ToString()),
                                            GenreId = Int32.Parse(GenreController.UpdateCreate(new SaveRequest<GenreRow>()
                                            {
                                                Entity = genre
                                            }).EntityId.ToString())
                                        }
                                    });
                                }
                                //countries
                                foreach (var country in item.ToCountries())
                                {
                                    MovieCountryController.UpdateCreate(new SaveRequest<MovieCountryRow>()
                                    {
                                        Entity = new MovieCountryRow()
                                        {
                                            MovieId = Int64.Parse(movie.EntityId.ToString()),
                                            CountryId = Int32.Parse(CountryController.UpdateCreate(new SaveRequest<CountryRow>()
                                            {
                                                Entity = country
                                            }).EntityId.ToString())
                                        }
                                    });
                                }
                                ServiceRatingController.UpdateCreate(new SaveRequest<ServiceRatingRow>()
                                {
                                    Entity = item.ToServiceRating((Int32)servicePath.Entity.ServiceId, Int64.Parse(movie.EntityId.ToString()))
                                });
                            }
                            catch (Exception e)
                            {
                                e.Log();
                                SqlErrorStore.Setup(SqlErrorStore.ApplicationName, StackExchange.Exceptional.ErrorStore.Default);

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Log();
                SqlErrorStore.Setup(SqlErrorStore.ApplicationName, StackExchange.Exceptional.ErrorStore.Default);
            }
            return View("~/Modules/Movie/ServicePath/ServicePathIndex.cshtml");

            //var res = CreateMovieVideo(username,
            //        new SaveRequest<MovieRow>()
            //        {
            //            Entity = item.ToMovie(movieKind)
            //        },
            //        new SaveRequest<VideoRow>()
            //        {
            //            Entity = item.ToVideo()
            //        },
            //        new SaveRequest<ServiceRow>()
            //        {
            //            Entity = serviceKinopoisk,
            //            EntityId = serviceKinopoisk.ServiceId,
            //        },
            //        new SaveRequest<ServiceRow>()
            //        {
            //            Entity = service,
            //            EntityId = service.ServiceId
            //        },
            //        new SaveRequest<ServiceRatingRow>()
            //        {
            //            Entity = item.ToServiceRating(service)
            //        },
            //        item.ToGenres(),
            //        item.ToCountries());
            //foreach (var i in item.ToPersons())
            //{
            //    i.PersonId = Int64.Parse(Persons.UpdateCreate(new SaveRequest<PersonRow>() { Entity = i }).EntityId.ToString());
            //    foreach (var j in item.ToCast(res, i))
            //    {
            //        Casts.UpdateCreate(new SaveRequest<CastRow>()
            //        {
            //            Entity = j
            //        });
            //    }
            //}
        }
        //[PageAuthorize("Administration")]
        //public static SaveResponse CreateMovieVideo(string username, SaveRequest<MovieRow> movie, SaveRequest<VideoRow> video, SaveRequest<ServiceRow> serviceKinopoisk, SaveRequest<ServiceRow> service, SaveRequest<ServiceRatingRow> serviceRating, List<GenreRow> genres, List<CountryRow> countries)
        //{

        //    SaveResponse response = null;

        //    response = Movies.UpdateCreate(movie, serviceRating);
        //    video.Entity.MovieId = (Int64)response.EntityId;
        //    serviceRating.Entity.ServiceId = (Int32)serviceKinopoisk.EntityId;
        //    serviceRating.Entity.MovieId = (Int64)response.EntityId;
        //    //SaveResponse servicesRatingResponse = ServicesRatings.UpdateCreate(serviceRating);
        //    video.Entity.ServiceId = (Int32)service.EntityId;
        //    SaveResponse videoResponce = Videos.UpdateCreate(video);

        //    foreach (var i in genres)
        //    {
        //        MovieGenres.UpdateCreate(new SaveRequest<MovieGenreRow>()
        //        {
        //            Entity = new MovieGenreRow()
        //            {
        //                MovieId = Int64.Parse(response.EntityId.ToString()),
        //                GenreId = Int16.Parse(Genres.UpdateCreate(new SaveRequest<GenreRow>()
        //                {
        //                    Entity = i
        //                }).EntityId.ToString())
        //            }
        //        });

        //    }
        //    foreach (var j in countries)
        //    {
        //        MovieCountries.UpdateCreate(new SaveRequest<MovieCountryRow>()
        //        {
        //            Entity = new MovieCountryRow()
        //            {
        //                MovieId = Int64.Parse(response.EntityId.ToString()),
        //                CountryId = Int16.Parse(Countries.UpdateCreate(new SaveRequest<CountryRow>()
        //                {
        //                    Entity = j
        //                }).EntityId.ToString())
        //            }
        //        });
        //    }
        //    return response;
        //}
    }
}