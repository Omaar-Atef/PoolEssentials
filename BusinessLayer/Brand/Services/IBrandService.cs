using DataLayer.Entities;

namespace BusinessLayer.Car.Services
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAll();

        Task<List<Brand>> GetAll(int pagenumber, int pagesize);

        Task<Brand> GetById(int id);
    }
}
