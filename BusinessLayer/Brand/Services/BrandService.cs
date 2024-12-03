using DataLayer.Context;
using DataLayer.Entities;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Car.Services
{
    public class BrandService : IBrandService
    {
        private readonly PoolsContext _context;

        public BrandService(PoolsContext context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetAll()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<List<Brand>> GetAll(int pagenumber, int pagesize)
        {
            return await _context.Brands.Skip((pagenumber -1) * pagesize).Take(pagesize).ToListAsync();
        }

        public async Task<Brand> GetById(int id)
        {
            return await _context.Brands.FirstOrDefaultAsync();
        }
    }
}
