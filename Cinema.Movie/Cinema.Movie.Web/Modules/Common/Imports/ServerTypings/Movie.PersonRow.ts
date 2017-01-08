﻿namespace Cinema.Movie.Movie {
    export interface PersonRow {
        PersonId?: number;
        Name?: string;
        FullName?: string;
        NameOther?: string;
        FullNameOther?: string;
        Url?: string;
        BirthDate?: string;
        DeathDate?: string;
        Gender?: Gender;
        About?: string;
        PathImage?: string;
    }

    export namespace PersonRow {
        export const idProperty = 'PersonId';
        export const nameProperty = 'Name';
        export const localTextPrefix = 'Movie.Person';
        export const lookupKey = 'Movie.Person';

        export function getLookup(): Q.Lookup<PersonRow> {
            return Q.getLookup<PersonRow>('Movie.Person');
        }

        export namespace Fields {
            export declare const PersonId: string;
            export declare const Name: string;
            export declare const FullName: string;
            export declare const NameOther: string;
            export declare const FullNameOther: string;
            export declare const Url: string;
            export declare const BirthDate: string;
            export declare const DeathDate: string;
            export declare const Gender: string;
            export declare const About: string;
            export declare const PathImage: string;
        }

        ['PersonId', 'Name', 'FullName', 'NameOther', 'FullNameOther', 'Url', 'BirthDate', 'DeathDate', 'Gender', 'About', 'PathImage'].forEach(x => (<any>Fields)[x] = x);
    }
}

