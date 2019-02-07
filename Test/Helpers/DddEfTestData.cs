﻿// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using DataLayer.EfClassesSql;
using DataLayer.EfCode;

namespace Test.Helpers
{
    public static class DddEfTestData
    {
        public const string DummyUserId = "UnitTestUserId";
        public static readonly DateTime DummyBookStartDate = new DateTime(2010, 1, 1);

        public static void SeedDatabaseDummyBooks(this SqlDbContext context, int numBooks = 10)
        {
            context.Books.AddRange(CreateDummyBooks(numBooks));
            context.SaveChanges();
        }

        public static Book CreateDummyBookOneAuthor()
        {

            var book = Book.CreateBook
            (
                "Book Title",
                "Book Description",
                DummyBookStartDate,
                "Book Publisher",
                123,
                null,
                new[] { new Author { Name = "Test Author"} }
            );

            return book.Result;
        }

        public static List<Book> CreateDummyBooks(int numBooks = 10, bool stepByYears = false)
        {
            var result = new List<Book>();
            var commonAuthor = new Author { Name = "CommonAuthor"};
            for (int i = 0; i < numBooks; i++)
            {
                var book = Book.CreateBook
                (
                    $"Book{i:D4} Title",
                    $"Book{i:D4} Description",
                    stepByYears ? DummyBookStartDate.AddYears(i) : DummyBookStartDate.AddDays(i),
                    "Publisher",
                    (short)(i + 1),
                    $"Image{i:D4}",
                    new[] { new Author { Name = $"Author{i:D4}"}, commonAuthor}
                ).Result;
                for (int j = 0; j < i; j++)
                {
                    book.AddReview((j % 5) + 1, null, j.ToString());
                }

                result.Add(book);
            }

            return result;
        }

        public static void SeedDatabaseFourBooks(this SqlDbContext context)
        {
            context.Books.AddRange(CreateFourBooks());
            context.SaveChanges();
        }

        public static List<Book> CreateFourBooks()
        {
            var martinFowler = new Author { Name = "Martin Fowler"};

            var books = new List<Book>();

            var book1 = Book.CreateBook
            (
                "Refactoring",
                "Improving the design of existing code",
                new DateTime(1999, 7, 8),
                null,
                40,
                null,
                new[] { martinFowler }
            ).Result;
            books.Add(book1);

            var book2 = Book.CreateBook
            (
                "Patterns of Enterprise Application Architecture",
                "Written in direct response to the stiff challenges",
                new DateTime(2002, 11, 15),
                null,
                53,
                null,
                new []{martinFowler}
            ).Result;
            books.Add(book2);

            var book3 = Book.CreateBook
            (
                "Domain-Driven Design",
                 "Linking business needs to software design",
                 new DateTime(2003, 8, 30),
                 null,
                56,
                null,
                new[] { new Author { Name = "Eric Evans"}}
            ).Result;
            books.Add(book3);

            var book4 = Book.CreateBook
            (
                "Quantum Networking",
                "Entangled quantum networking provides faster-than-light data communications",
                new DateTime(2057, 1, 1),
                "Future Published",
                220,
                null,
                new[] { new Author { Name = "Future Person"} }
            ).Result;
            book4.AddReview(5,
                "I look forward to reading this book, if I am still alive!", "Jon P Smith");
            book4.AddReview(5,
                "I write this book if I was still alive!", "Albert Einstein"); book4.AddPromotion(219, "Save $1 if you order 40 years ahead!");
            book4.AddPromotion(219, "Save 1$ by buying 40 years ahead");

            books.Add(book4);

            return books;
        }
    }
}