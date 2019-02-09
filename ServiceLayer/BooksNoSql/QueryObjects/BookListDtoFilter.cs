﻿// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.EfClassesNoSql;
using ServiceLayer.BooksCommon;

namespace ServiceLayer.BooksNoSql.QueryObjects
{
    public static class BookListDtoFilter
    {
        public const string AllBooksNotPublishedString = "Coming Soon";

        public static IQueryable<BookListNoSql> FilterBooksBy(
            this IQueryable<BookListNoSql> books, 
            BooksFilterBy filterBy, string filterValue)         
        {
            if (string.IsNullOrEmpty(filterValue))              
                return books;                                   

            switch (filterBy)
            {
                case BooksFilterBy.NoFilter:                    
                    return books;                               
                case BooksFilterBy.ByVotes:
                    var filterVote = int.Parse(filterValue);     
                    return books.Where(x => x.ReviewsAverageVotes > filterVote);   
                case BooksFilterBy.ByPublicationYear:             
                    if (filterValue == AllBooksNotPublishedString)
                        return books.Where(x => x.PublishedOn > DateTime.UtcNow);

                    var filterYear = int.Parse(filterValue);      
                    return books.Where(x => x.PublishedOn.Year == filterYear && x.PublishedOn <= DateTime.UtcNow);   
                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(filterBy), filterBy, null);
            }
        }
    }
}