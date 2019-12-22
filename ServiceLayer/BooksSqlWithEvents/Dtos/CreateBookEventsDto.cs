﻿// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.EfClassesSql;
using DataLayerEvents.EfClasses;
using GenericServices;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.BooksSqlWithEvents.Dtos
{
    public class CreateBookEventsDto : ILinkToEntity<BookWithEvents>
    {
        public CreateBookEventsDto()
        {
            PublishedOn = DateTime.Today;
        }

        //This will be populated with the primary key of the created book
        public Guid BookId{ get; set; }

        //I would normally have the Required attribute to catch this at the front end
        //But to show how the static create method catches that error I have commented it out
        //[Required(AllowEmptyStrings = false)]
        public string Title { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishedOn { get; set; }

        public string Publisher { get; set; }

        [Range(0,1000)]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<Author> Authors { get; set; }

        public List<KeyName> AllPossibleAuthors { get; private set; }

        public List<int> BookAuthorIds { get; set; } = new List<int>();

        public void BeforeDisplay(DbContext context)
        {
            AllPossibleAuthors = context.Set<Author>().Select(x => new KeyName(x.AuthorId, x.Name))
                .OrderBy(x => x.Name).ToList();
        }

        public void BeforeSave(DbContext context)
        {
            Authors = BookAuthorIds.Select(x => context.Find<Author>(x)).Where(x => x != null).ToList();
        }

        //---------------------------------------------------------
        //Now the data for the front end

        public struct KeyName
        {
            public KeyName(Guid authorId, string name)
            {
                AuthorId = authorId;
                Name = name;
            }

            public Guid AuthorId { get; }
            public string Name { get; }
        }
    }
}