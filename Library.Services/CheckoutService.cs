using Library.Data;
using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //return _context.Holds
            //    .Include(x => x.LibraryCard)
            //    .OrderByDescending(a => a.HoldPlaced)
            //    .FirstOrDefault(y => y.Id == assetId)
            //    .LibraryCard.;
            throw new NotImplementedException();
        }

        public IEnumerable<Hold> GetHolds(int assetId)
        {
            throw new NotImplementedException();
        }

        public void MarkFound(int assetId)
        {
            throw new NotImplementedException();
        }

        public void MarkLost(int assetId)
        {
            throw new NotImplementedException();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public void CheckInItem(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }
    }
}
