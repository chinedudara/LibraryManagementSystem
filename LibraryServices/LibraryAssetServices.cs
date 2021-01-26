using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetServices : ILibraryAsset
    {
        private LibraryContext _context;

        public LibraryAssetServices(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(x => x.Status)
                .Include(y => y.Location);
        }

        public string GetAuthorOrDirector(int Id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>().Any(x => x.Id == Id);
            var isVideo = _context.LibraryAssets.OfType<Video>().Any(x => x.Id == Id);
            return isBook ? _context.LibraryAssets.OfType<Book>().FirstOrDefault(x => x.Id == Id).Author
                : _context.LibraryAssets.OfType<Book>().FirstOrDefault(x => x.Id == Id).Author;
        }

        public LibraryAsset GetById(int Id)
        {
            return _context.LibraryAssets
                .Include(x => x.Status)
                .Include(y => y.Location)
                .FirstOrDefault(x => x.Id == Id);
        }

        public LibraryBranch GetCurrentLocation(int Id)
        {
            return GetById(Id).Location;
        }

        public string GetDeweyIndex(int Id)
        {
            if(_context.Books.Any(x => x.Id == Id))
            {
                return _context.Books.FirstOrDefault(y => y.Id == Id).DeweyIndex;
            }
            return "";
        }

        public string GetIsbn(int Id)
        {
            if (_context.Books.Any(x => x.Id == Id))
            {
                return _context.Books.FirstOrDefault(y => y.Id == Id).ISBN;
            }
            return "";
        }

        public string GetTitle(int Id)
        {
            return _context.LibraryAssets.FirstOrDefault(y => y.Id == Id).Title;
        }

        public string GetType(int Id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>().Any(x => x.Id == Id);
            var isVideo = _context.LibraryAssets.OfType<Video>().Any(x => x.Id == Id);
            return isBook ? "Book" : "Video";
        }
    }
}
