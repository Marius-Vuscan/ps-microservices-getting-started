using GloboTicket.Grpc;
using GloboTicket.Web.Models;
using GloboTicket.Web.Models.View;
using GloboTicket.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GloboTicket.Web.Controllers
{
    public class EventCatalogController : Controller
    {
        //private readonly IEventCatalogService eventCatalogService;
        private readonly IShoppingBasketService shoppingBasketService;
        private readonly Settings settings;

        private readonly Events.EventsClient eventCatalogService;

        public EventCatalogController(
            //IEventCatalogService eventCatalogService, 
            IShoppingBasketService shoppingBasketService,
            Settings settings,
            Events.EventsClient eventCatalogService)
        {
            //this.eventCatalogService = eventCatalogService;
            this.shoppingBasketService = shoppingBasketService;
            this.settings = settings;
            this.eventCatalogService = eventCatalogService;
        }

        public async Task<IActionResult> Index(Guid categoryId)
        {
            var getCategories = eventCatalogService.GetAllCategoriesAsync(new GetAllCategoriesRequest());
            var getEvents = categoryId == Guid.Empty ? eventCatalogService.GetAllAsync(new GetAllEventsRequest()) :
                eventCatalogService.GetAllByCategoryIdAsync(new GetAllEventsByCategoryIdRequest { CategoryId = categoryId.ToString() });
            await Task.WhenAll(new Task[] { getCategories.ResponseAsync, getEvents.ResponseAsync });

            return View(
                new EventListModel
                {
                    Events = getEvents.ResponseAsync.Result.Events,
                    Categories = getCategories.ResponseAsync.Result.Categories,
                    SelectedCategory = categoryId
                }
            );
        }

        [HttpPost]
        public IActionResult SelectCategory([FromForm] Guid selectedCategory)
        {
            return RedirectToAction("Index", new { categoryId = selectedCategory });
        }

        public async Task<IActionResult> Detail(Guid eventId)
        {
            var ev = await eventCatalogService.GetByEventIdAsync(new GetByEventIdRequest { EventId = eventId.ToString() });
            return View(ev);
        }
    }
}
