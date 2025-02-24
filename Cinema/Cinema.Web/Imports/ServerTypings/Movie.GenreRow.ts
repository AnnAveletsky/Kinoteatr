﻿namespace Cinema.Movie {
    export interface GenreRow {
        GenreId?: number;
        Name?: string;
        Icon?: string;
        Style?: string;
    }

    export namespace GenreRow {
        export const idProperty = 'GenreId';
        export const nameProperty = 'Name';
        export const localTextPrefix = 'Movie.Genre';
        export const lookupKey = 'Movie.Genre';

        export function getLookup(): Q.Lookup<GenreRow> {
            return Q.getLookup<GenreRow>('Movie.Genre');
        }

        export namespace Fields {
            export declare const GenreId: string;
            export declare const Name: string;
            export declare const Icon: string;
            export declare const Style: string;
        }

        ['GenreId', 'Name', 'Icon', 'Style'].forEach(x => (<any>Fields)[x] = x);
    }
}

