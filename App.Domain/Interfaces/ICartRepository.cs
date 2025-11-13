using App.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Cart?> GetCartWithItemsAsync(int userId, CancellationToken cancellationToken = default);
    }
}
