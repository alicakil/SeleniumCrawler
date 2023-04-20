using Dashboard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Dashboard.Repository
{
    public class StoryRepository : Repository<Story>
    {
        public readonly Context c;
        public StoryRepository(Context context, CurrentUser currentUser) : base(context, currentUser)
        {
            c = context;
        }

        public Story GetById(int id, params string[] includings)
        {            
            var query = c.Stories.AsQueryable();

            foreach (var including in includings)
            {
                query = query.Include(including);
            }

            return query.FirstOrDefault(x => x.Id == id);
        }


        public List<Keyword> GetKeyWords()
        {
            return c.Keywords.ToList();
        }

    }
}

