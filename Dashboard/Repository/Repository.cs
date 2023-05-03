
using Dashboard.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dashboard.Repository
{
    public class Repository<T> where T : class
    {
        protected Context c;
        private DbSet<T> dbset;
        private readonly CurrentUser currentUser;

        public Repository(Context context, CurrentUser currentUser)
        {
            c = context;
            dbset = c.Set<T>();
            this.currentUser = currentUser;
        }

        public void Add(T t)
        {
            switch (t)
            {
                case BaseCreated e:
                    {
                        e.CreatedAt = DateTime.Now;
                        e.CreatedById = currentUser.Id;
                    }
                    break;
                default:
                    break;
            }
            dbset.Add(t);
        }



        public void AddRange(IList<T> ts)  // TODO: refactor using : dbset.AddRange(ts);
        {
            foreach (var t in ts)
                Add(t);
        }

        public IList<T> GetAll()
        {
            if (dbset.Any())
            {
                return dbset.AsNoTracking().AsSplitQuery().ToList();
            }
            else
            {
                return new List<T> { };
            }
        }

        public IQueryable<T> GetFilterBy(Expression<Func<T, bool>> conditionsLamda)
        {
            return dbset.Where(conditionsLamda);
        }


        public bool Any(Expression<Func<T, bool>> conditionsLamda)
        {
            return dbset.AsNoTracking().Where(conditionsLamda).Any();
        }

        public PageData<T> GetFilterByPage(Expression<Func<T, bool>> conditionsLamda, int pageNo)
        {
            if (pageNo < 1) pageNo = 1;
            return new PageData<T>()
            {
                
                PageNo = pageNo,
                NrOfRecs = dbset.Count(),
                pageData = dbset.AsNoTracking().Where(conditionsLamda).Skip((pageNo - 1) * AppConstants.AppInfo.MaxRecsPerPage).Take(AppConstants.AppInfo.MaxRecsPerPage).AsNoTracking().ToList()
            };
        }

        public T GetById(int id)
        {
            return dbset.Find(id);
        }


        public void Remove(int id)
        {
            dbset.Remove(GetById(id));
        }

        public void Remove(T t)
        {
            dbset.Remove(t);
        }

        public void Remove(IList<T> ts)
        {
            dbset.RemoveRange(ts);
        }

        public void Remove(Expression<Func<T, bool>> conditionsLamda)
        {
            var recorsToDelete = dbset.Where(conditionsLamda).ToList();
            Remove(recorsToDelete);
        }

        
        public void Update(T t)
        {

            switch (t)
            {
                case BaseModified e:
                    {
                        e.ModifiedAt = DateTime.Now;
                        if (currentUser is not null)
                            e.ModifiedById = currentUser.Id;
                    }
                    break;
                default:
                    break;
            }
            dbset.Update(t);
        }

        public PageData<T> GetPage(int pageNo)
        {
            if (pageNo < 1) pageNo = 1;
            return new PageData<T>()
            {
                PageNo = pageNo,
                NrOfRecs = dbset.Count(),
                pageData = dbset.Skip((pageNo - 1) * AppConstants.AppInfo.MaxRecsPerPage).Take(AppConstants.AppInfo.MaxRecsPerPage).AsNoTracking().ToList()
            };
        }

    }
}
