using Dashboard.Models;

namespace Dashboard.Repository
{
    public class UnitOfWork
    {
        private Context c;
        public EventRepository eventRepository { get; private set; }
        // other repositories might needed to be created here..


        public UnitOfWork(Context c, CurrentUser currentUser)
        {
            this.c = c;
            eventRepository = new EventRepository(this.c, currentUser);
        }


        public int Complete()
        {
            return c.SaveChanges();
        }

        public void Dispose()
        {
            c.Dispose();
        }
    }
}
