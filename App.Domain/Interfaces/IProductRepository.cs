using App.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    }
}
