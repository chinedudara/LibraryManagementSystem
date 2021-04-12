using Library.Data;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Services
{
    public class CheckoutService : ICheckoutService
    {
        private LibraryContext _context;

        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public DateTime GetCurrentHoldDate(int assetId)
        {
            return _context.Holds
                .Include(x => x.LibraryAsset)
                .OrderByDescending(a => a.HoldPlaced)
                .FirstOrDefault(y => y.Id == assetId).HoldPlaced;
        }

        public string GetCurrentHoldPatronName(int assetId)
        {
            var hold = _context.Holds
                .Include(x => x.LibraryCard)
                .Include(x => x.LibraryAsset)
                .OrderByDescending(a => a.HoldPlaced)
                .FirstOrDefault(y => y.Id == assetId);

            var cardId = hold?.LibraryCard.Id;
            return GetPatronNameByLibraryCardId(cardId);
        }

        public IEnumerable<Hold> GetCurrentHolds(int assetId)
        {
            return _context.Holds
                .Include(x => x.LibraryAsset)
                .Where(z => z.LibraryAsset.Id == assetId)
                .OrderByDescending(a => a.HoldPlaced);
        }

        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;

            UpdateAssetStatus(assetId, "available");
            RemoveExistingCheckouts(assetId);
            CloseExistingCheckoutHistory(assetId, now);

            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string status)
        {
            var asset = _context.LibraryAssets.FirstOrDefault(x => x.Id == assetId);
            _context.Update(asset);
            asset.Status = _context.Statuses.FirstOrDefault(x => x.Name.ToLower() == status);
        }

        private void CloseExistingCheckoutHistory(int assetId, DateTime now)
        {
            var checkoutHistory = _context.CheckoutHistories
                .FirstOrDefault(x => x.LibraryAsset.Id == assetId && x.CheckedIn == null);
            if (checkoutHistory != null)
            {
                _context.Update(checkoutHistory);
                checkoutHistory.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int assetId)
        {
            var checkout = _context.Checkouts.FirstOrDefault(x => x.LibraryAsset.Id == assetId);
            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "lost");
            _context.SaveChanges();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;
            var asset = _context.LibraryAssets.FirstOrDefault(x => x.Id == assetId);
            var card = _context.LibraryCards
                .Include(x => x.Checkouts)
                .FirstOrDefault(x => x.Id == libraryCardId);
            var hold = new Hold
            {
                LibraryAsset = asset,
                LibraryCard = card,
                HoldPlaced = now,
            };
            UpdateAssetStatus(assetId, "On Hold");
            _context.Add(hold);
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId, int libraryCardId)
        {
            var asset = _context.LibraryAssets.FirstOrDefault(x => x.Id == assetId);
            var now = DateTime.Now;
            //Remove checkout
            RemoveExistingCheckouts(assetId);

            //Update asset status
            UpdateAssetStatus(assetId, "Available");

            //Close checkout history
            CloseExistingCheckoutHistory(assetId, now);

            //Check holds
            var currentHolds = _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryAsset.Id == assetId);

            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
            }
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var hold = currentHolds.OrderBy(x => x.HoldPlaced).FirstOrDefault();
            var libraryCard = hold.LibraryCard;
            _context.Remove(hold);
            CheckOutItem(assetId, libraryCard.Id);
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            //Check if asset is available
            if (IsCheckedOut(assetId))
            {
                return;
            }

            //Create checkout
            var asset = _context.LibraryAssets.FirstOrDefault(x => x.Id == assetId);
            var card = _context.LibraryCards
                .Include(x => x.Checkouts)
                .FirstOrDefault(x => x.Id == libraryCardId);
            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = asset,
                LibraryCard = card,
                Since = now,
                Until = SetDefaultCheckInDate(now),
            };
            _context.Add(checkout);

            //Update asset status
            UpdateAssetStatus(assetId, "Checked Out");

            //Create checkout history
            var history = new CheckoutHistory
            {
                LibraryAsset = asset,
                LibraryCard = card,
                CheckedOut = now,
            };
            _context.Add(history);
            _context.SaveChanges();
        }

        private DateTime SetDefaultCheckInDate(DateTime now)
        {
            return now.AddDays(30);
        }

        private bool IsCheckedOut(int assetId)
        {
            return _context.Checkouts
                .Include(x => x.LibraryAsset)
                .Where(x => x.LibraryAsset.Id == assetId)
                .Any();
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(y => y.LibraryAsset.Id == id);
        }

        public Checkout GetLatestCheckout(int assetId)
        {
            return _context.Checkouts.Where(x => x.LibraryAsset.Id == assetId)
                .OrderByDescending(x => x.Since)
                .FirstOrDefault();
        }

        public string GetCurrentCheckoutPatronName(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            if (checkout == null)
            {
                return "";
            }
            var cardId = checkout?.LibraryCard.Id;
            return GetPatronNameByLibraryCardId(cardId);
        }

        private string GetPatronNameByLibraryCardId(int? cardId)
        {
            var patron = _context.Patrons
                .Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.LibraryCard.Id == cardId);
            return patron?.FirstName + " " + patron?.LastName;
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.LibraryAsset.Id == assetId);
        }
    }
}
