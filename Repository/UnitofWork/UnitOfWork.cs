using Core.Interfaces;
using Repository.MODELS.DATA;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitofWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBcontext context;

        public UnitOfWork(DBcontext _context)
        {
            context = _context;
        }
        public void Dispose()
        {
            context.Dispose();
        }

        public IBaseRepo<T> Repository<T>() where T : class
        {
            return new BaseRepo<T>(context);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }
    }
}
