using App.Domain.Entities;
using App.Domain.Interfaces;
using App.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Repositories
{ 
        public class ProductRepository : Repository<Product>, IProductRepository
        {
            public ProductRepository(ApplicationDbContext context) : base(context)
            {
            }

            public async Task<IEnumerable<Product>> GetByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .Include(p => p.Category)
                    .Where(p => p.CategoryId == categoryId && p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync(cancellationToken);
            }

            public async Task<IEnumerable<Product>> SearchAsync(
                string searchTerm,
                CancellationToken cancellationToken = default)
            {
                var lowerSearchTerm = searchTerm.ToLower();

                return await _dbSet
                    .Include(p => p.Category)
                    .Where(p => p.IsActive &&
                           (p.Name.ToLower().Contains(lowerSearchTerm) ||
                            p.Description.ToLower().Contains(lowerSearchTerm) ||
                            p.SKU.ToLower().Contains(lowerSearchTerm)))
                    .OrderBy(p => p.Name)
                    .ToListAsync(cancellationToken);
            }

            public async Task<IEnumerable<Product>> GetActiveProductsAsync(
                CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .Include(p => p.Category)
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync(cancellationToken);
            }

            public async Task<IEnumerable<Product>> GetLowStockProductsAsync(
                int threshold,
                CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && p.StockQuantity <= threshold)
                    .OrderBy(p => p.StockQuantity)
                    .ToListAsync(cancellationToken);
            }

            public override async Task<Product?> GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            }
    }
}
