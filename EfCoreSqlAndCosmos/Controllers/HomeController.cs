﻿using System.Collections.Generic;
using EfCoreInAction.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceLayer.Logger;

namespace EfCoreSqlAndCosmos.Controllers
{
    public class HomeController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public HomeController(EfCoreContext context)
        {                                           
            _context = context;                     
        }

        public IActionResult Index
        (NoSqlSortFilterPageOptions options,
            [FromServices] INoSqlCreators creator,
            [FromServices] ILogger<RavenStore> ravenLogger)         
        {

            List<BookListNoSql> bookList;
            using (var context = creator.CreateNoSqlAccessor())
            {
                var listService = new ListBooksNoSqlService(context.BookListQuery());
                bookList = listService
                    .SortFilterPage(options)
                    .ToList();
                context.Command = options.ToString();
            }
                 
            SetupTraceInfo();           //REMOVE THIS FOR BOOK as it could be confusing

            return View(new BookListNoSqlCombinedDto
                (options, bookList));              
        }
        /*******************************************************
        #A I have to make the Index action method async, by using the async keyword and the returned type has to be wrapped in a generic task
        #B I have to await the result of the ToListAsync method, which is an async command
        #C Because my SortFilterPage method returned IQueryable<T> I can change is to async simply by replacing the .ToList() by the .ToListAsync() method 
        * *****************************************************/

        /// <summary>
        /// This provides the filter search dropdown content
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFilterSearchContent    //#A
            (SortFilterPageOptions options)         //#B
        {
            var service = new                       //#C
                BookFilterDropdownService(_context);//#C

            var traceIdent = HttpContext.TraceIdentifier; //REMOVE THIS FOR BOOK as it could be confusing

            return Json(                            //#D
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                traceIdent,
                service.GetFilterDropDownValues(    //#E
                    options.FilterBy)));            //#E
        }
        /****************************************************
        #A This method is called by the URL Home/GetFilterSearchContent
        #B It also gets the sort, filter, page options from the URL
        #C We create the BookFilterDropdownService using the applications's DbContext provided by ASP.NET Core
        #D This converts normal .NET objects into JSON format to send back to the AJAX Get call
        #E The GetFilterDropDownValues method calculates the right data needed for the chosen filter type 
         * **************************************************/


        public IActionResult About()
        {
            var isLocal = Request.IsLocal();
            return View(isLocal);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
