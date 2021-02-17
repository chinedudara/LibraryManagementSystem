using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data
{
    public interface ICheckoutService
    {
        IEnumerable<Checkout> GetAll();
        Checkout GetById(int id);
        void Add(Checkout newCheckout);
        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId, int libraryCardId);

        void PlaceHold(int assetId, int libraryCardId);
        string GetCurrentHoldPatronName(int assetId);
        DateTime GetCurrentHoldDate(int assetId);
        IEnumerable<Hold> GetHolds(int assetId);

        void MarkFound(int assetId);
        void MarkLost(int assetId);
    }
}
