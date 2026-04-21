using AutoMapper;
using Lab3.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lab2.Repository
{
    public class GenericRepo<T> where T : class
    {
        private readonly ITIDbContext _context;
       
        public GenericRepo(ITIDbContext _context)
        {
            this._context = _context;
           
        }
        public IQueryable<T> GetAllQueryable(params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable().AsNoTracking();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }
        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }
        public async Task<T> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<T> Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null) return false;

            _context.Set<T>().Remove(entity);
            return true;
        }
        public void Update(T entity)
        {
            //_context.Entry(entity).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.Set<T>().Update(entity);
        }

    }
}
