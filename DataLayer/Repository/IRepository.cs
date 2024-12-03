using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(int pagenumber , int pagesize);
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter);

        Task<T?> GetById(int id);

        Task<T> Insert(T entity);

        Task<T> Update(T entity);

        Task Delete(T entity);

        void Detach(T entity);

        int GetCount();


    }
}
