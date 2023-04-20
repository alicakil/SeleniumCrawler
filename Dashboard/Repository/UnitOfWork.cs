using Dashboard.Models;

namespace Dashboard.Repository
{
    public class UnitOfWork
    {
        private Context c;
        public StoryRepository storyRepository { get; private set; }
        public AccountRepository accountRepository { get; private set; }
        // other repositories might needed to be created here..


        public UnitOfWork(Context c, CurrentUser currentUser)
        {
            this.c = c;
            storyRepository = new StoryRepository(this.c, currentUser);
            accountRepository = new AccountRepository(this.c, currentUser);
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
