using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TollFeeSystem.Core.Models;

namespace TollFeeSystem.Core.Repository
{
    
    public interface IRepository<TEntity> where TEntity : class
    {
        public Task<IEnumerable<TEntity>> GetAll();

    }
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly TfsContext _context;

        public Repository(TfsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            var result = await _context.Set<TEntity>().Where(x => x != null).ToListAsync();
            return result;
        }
    }
}
