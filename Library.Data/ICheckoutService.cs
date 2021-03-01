using Library.Data.Models;
using System;
using System.Collections.Generic;

namespace Library.Data
{
    public interface ICheckoutService
    {
        IEnumerable<Checkout> GetAll();
        Checkout GetById(int id);
        void Add(Checkout newCheckout);
        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId, int libraryCardId);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        Checkout GetLatestCheckout(int assetId);
        string GetCurrentCheckoutPatronName(int assetId);

        void PlaceHold(int assetId, int libraryCardId);
        string GetCurrentHoldPatronName(int assetId);
        DateTime GetCurrentHoldDate(int assetId);
        IEnumerable<Hold> GetCurrentHolds(int assetId);

        void MarkFound(int assetId);
        void MarkLost(int assetId);
    }
}
