﻿using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface ILibraryAsset
    {
        IEnumerable<LibraryAsset> GetAll();
        LibraryAsset GetById(int Id);
        void Add(LibraryAsset Asset);
        string GetAuthorOrDirector(int Id);
        string GetType(int Id);
        string GetIsbn(int Id);
        string GetTitle(int Id);
        string GetDeweyIndex(int Id);

        LibraryBranch GetCurrentLocation(int Id);
    }
}
