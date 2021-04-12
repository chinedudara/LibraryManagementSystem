using Library.Data;
using Library.Models.Catalog;
using Library.Web.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _assetsService;
        private ICheckoutService _checkoutService;

        public CatalogController(ILibraryAsset assetsService, ICheckoutService checkoutService)
        {
            _assetsService = assetsService;
            _checkoutService = checkoutService;
        }

        public IActionResult index()
        {
            var result = _assetsService.GetAll();
            var listingResult = result.Select(x => new AssetIndexListingModel
            {
                Id = x.Id,
                ImageUrl = x.ImageUrl,
                AuthorOrDirector = _assetsService.GetAuthorOrDirector(x.Id),
                DeweyCallNumber = _assetsService.GetDeweyIndex(x.Id),
                Title = x.Title,
                Type = _assetsService.GetType(x.Id)
            });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };

            return View(model);
        }

        public IActionResult Detail(int Id)
        {
            var result = _assetsService.GetById(Id);
            var hold = _checkoutService.GetCurrentHolds(Id)
                .Select(x => new AssetHoldModel
                {
                    PatronName = _checkoutService.GetCurrentHoldPatronName(Id),
                    HoldPlaced = _checkoutService.GetCurrentHoldDate(Id).ToString("d")
                });

            var model = new AssetDetailModel()
            {
                AssetId = result.Id,
                Title = result.Title,
                Type = _assetsService.GetType(Id),
                AuthorOrDirector = _assetsService.GetAuthorOrDirector(Id),
                Year = result.Year,
                ISBN = _assetsService.GetIsbn(Id),
                DeweyCAllNumber = _assetsService.GetDeweyIndex(Id),
                Status = result.Status.Name,
                Cost = result.Cost,
                CurrentLocation = _assetsService.GetCurrentLocation(Id).Name,
                ImageUrl = result.ImageUrl,
                PatronName = _checkoutService.GetCurrentCheckoutPatronName(Id),
                Checkout = _checkoutService.GetLatestCheckout(Id),
                CheckoutHistory = _checkoutService.GetCheckoutHistory(Id),
                CurrentHolds = hold
            };
            return View(model);
        }
    }
}
