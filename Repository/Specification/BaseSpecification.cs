using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specification
{
    public class BaseSpecification<T> : ISpecification<T> where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();

        public BaseSpecification(Expression<Func<T, bool>> exp) => this.Criteria = exp;
        public void AddInclude(Expression<Func<T, object>> include) => this.Includes.Add(include);

    }
}
