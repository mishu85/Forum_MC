﻿using ForumMCBackend.Db;
using ApplicationCore.Entities;

namespace ForumMCBackend.Repositories
{
    public class SQLiteCategoriesRepository : ICategoriesRepository
    {
        readonly SQLiteContext _context = SQLiteContext.Default;

        public List<Category> GetAll()
        {
            var categories = _context?.Categories?.ToList() ?? new List<Category>();
            return categories;
        }

        public Category? GetByID(int id)
        {
            var category = _context?.Categories?.FirstOrDefault(category => category.Id == id);
            return category;
        }
    }
}
