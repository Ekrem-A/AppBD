using App.Domain.Entities;
using App.Domain.Interfaces;
using App.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly Dictionary<Type, object> _repositories = new();


        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public IUserRepository Users { get; }
        public ICartRepository Carts { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Payment> Payments { get; }
        public IRepository<Address> Addresses { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Orders = new OrderRepository(_context);
            Users = new UserRepository(_context);
            Carts = new CartRepository(_context);
            Categories = new Repository<Category>(_context);
            Payments = new Repository<Payment>(_context);
            Addresses = new Repository<Address>(_context);
        }

        public IRepository<T> GetRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);

            if (_repositories.TryGetValue(type, out var repo))
            {
                return (IRepository<T>)repo;
            }

            var newRepo = new Repository<T>(_context);
            _repositories[type] = newRepo;

            return newRepo;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _transaction?.CommitAsync(cancellationToken)!;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction?.RollbackAsync(cancellationToken)!;
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
