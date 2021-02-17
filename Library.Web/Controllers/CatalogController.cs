using Library.Models.Catalog;
using Library.Web.Models.Catalog;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _assets;

        public CatalogController(ILibraryAsset assets)
        {
            _assets = assets;
        }

        public IActionResult index()
        {
            var result = _assets.GetAll();
            var listingResult = result.Select(x => new AssetIndexListingModel
            {
                Id = x.Id,
                ImageUrl = x.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(x.Id),
                DeweyCallNumber = _assets.GetDeweyIndex(x.Id),
                Title = x.Title,
                Type = _assets.GetType(x.Id)
            });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };

            return View(model);
        }

        public IActionResult Detail(int Id)
        {
            var result = _assets.GetById(Id);
            var model = new AssetDetailModel()
            {
                AssetId = result.Id,
                Title = result.Title,
                Type = _assets.GetType(Id),
                AuthorOrDirector = _assets.GetAuthorOrDirector(Id),
                Year = result.Year,
                ISBN = _assets.GetIsbn(Id),
                DeweyCAllNumber = _assets.GetDeweyIndex(Id),
                Status = result.Status.Name,
                Cost = result.Cost,
                CurrentLocation = result.Location.Name,
                ImageUrl = result.ImageUrl
            };
            return View(model);
        }
    }
}
