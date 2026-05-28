using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repositories
{
    public class MemoryRepository<T> : IRepository<T>
        where T : Entity
    {
        private List<T> values = [];

        public Task<T> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            values.Add(entity);
            return Task.FromResult(entity);
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            values = values.Where(x => x.Id != id).ToList();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<T>>(values);
        }

        public Task<T> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.values.First(x => x.Id == id));
        }

        public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            var index = values.FindIndex(x => x.Id == entity.Id);
            if (index >= 0)
            {
                values[index] = entity;
            }
            return Task.FromResult(entity);
        }
    }
}
