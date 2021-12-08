using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Models.Repos
{
    public interface IRepo<TEntity>
    {
        List<TEntity> List(int pageNumber, int pageSize);
        List<TEntity> List();
        TEntity Find(int id);
        bool Add(TEntity entity);
        bool Add(TEntity entity, List<IFormFile> files);
        bool Update(TEntity entity);
        bool Update(TEntity entity, List<IFormFile> files);
        bool Delete(int id);
        bool IsExist(int id);
    }
}
