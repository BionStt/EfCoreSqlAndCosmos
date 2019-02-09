﻿// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ServiceLayer.BooksCommon;
using ServiceLayer.BooksSql.Dtos;

namespace ServiceLayer.BooksSql.QueryObjects
{


    public static class BookListDtoSort
    {
        public static IQueryable<BookListDto> OrderBooksBy
            (this IQueryable<BookListDto> books, OrderByOptions orderByOptions)
        {
            switch (orderByOptions)
            {              
                case OrderByOptions.ByVotes:              
                    return books.OrderByDescending(x =>   
                        x.ReviewsAverageVotes);           
                case OrderByOptions.ByPublicationDate:    
                    return books.OrderByDescending(       
                        x => x.PublishedOn);              
                case OrderByOptions.ByPriceLowestFirst:   
                    return books.OrderBy(x => x.ActualPrice);
                case OrderByOptions.ByPriceHigestFirst:   
                    return books.OrderByDescending(       
                        x => x.ActualPrice);              
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(orderByOptions), orderByOptions, null);
            }
        }

        /************************************************************
        #A Because of paging we always need to sort. I default to showing latest entries first
        #B This orders the book by votes. Books without any votes (null return) go at the bottom
        #C Order by publication date - latest books at the top
        #D Order by actual price, which takes into account any promotional price - both lowest first and highest first
         * ********************************************************/
    }

}