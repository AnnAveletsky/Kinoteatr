﻿namespace Cinema.Movie.Movie {
    export interface HistoryRow {
        HistoryId?: number;
        UserName?: string;
        Message?: string;
        EventDataTime?: string;
        Status?: boolean;
        CastId?: number;
        CountryId?: number;
        GenreId?: number;
        ServiceId?: number;
        MovieId?: number;
        PersonId?: number;
        ImageId?: number;
        MovieCountryId?: number;
        ServicePathId?: number;
        ServiceRatingId?: number;
        MovieTagId?: number;
        TagId?: number;
        VideoId?: number;
        MovieGenreId?: number;
    }

    export namespace HistoryRow {
        export const idProperty = 'HistoryId';
        export const nameProperty = 'UserName';
        export const localTextPrefix = 'Movie.History';

        export namespace Fields {
            export declare const HistoryId: string;
            export declare const UserName: string;
            export declare const Message: string;
            export declare const EventDataTime: string;
            export declare const Status: string;
            export declare const CastId: string;
            export declare const CountryId: string;
            export declare const GenreId: string;
            export declare const ServiceId: string;
            export declare const MovieId: string;
            export declare const PersonId: string;
            export declare const ImageId: string;
            export declare const MovieCountryId: string;
            export declare const ServicePathId: string;
            export declare const ServiceRatingId: string;
            export declare const MovieTagId: string;
            export declare const TagId: string;
            export declare const VideoId: string;
            export declare const MovieGenreId: string;
        }

        ['HistoryId', 'UserName', 'Message', 'EventDataTime', 'Status', 'CastId', 'CountryId', 'GenreId', 'ServiceId', 'MovieId', 'PersonId', 'ImageId', 'MovieCountryId', 'ServicePathId', 'ServiceRatingId', 'MovieTagId', 'TagId', 'VideoId', 'MovieGenreId'].forEach(x => (<any>Fields)[x] = x);
    }
}

