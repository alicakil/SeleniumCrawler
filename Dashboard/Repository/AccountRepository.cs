using Dashboard.Dtos;
using Dashboard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Linq.Expressions;

namespace Dashboard.Repository
{
    public class AccountRepository : Repository<Account>
    {
        public readonly Context c;
        public AccountRepository(Context context, CurrentUser currentUser) : base(context, currentUser)
        {
            c = context;
        }


        public ResponseDto<Account> login(string email, string password)
        {
            // Check mail with password
            var account = c.Accounts.Where(x => (x.Email.Equals(email)) && x.Password.Equals(password)).FirstOrDefault();

            if (account is null)
                return new ResponseDto<Account> { Result = false, Msg = "Email or password is incorrect!" };

            // Login Successfull
            c.Accounts.Update(account);
            c.SaveChanges();

            return new ResponseDto<Account> 
            { 
                Result = true, 
                Payload = account 
            };
        }

    }
}

