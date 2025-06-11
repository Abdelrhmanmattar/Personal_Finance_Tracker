using Core.Specification;
using Microsoft.EntityFrameworkCore;

namespace Repository.Specification
{
    public class SpecificationHandler<T> where T : class
    {
        public static IQueryable<T> ApplySpecification(IQueryable<T> query, ISpecification<T> specs)
        {
            var res_query = query;
            if (specs.Criteria != null)
                res_query = res_query.Where(specs.Criteria);

            if (specs.Includes != null)
                foreach (var inc in specs.Includes)
                    res_query = res_query.Include(inc);
            return res_query.AsNoTracking();
        }

    }

}
