using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public bool RegisterUser(string username, string password, bool isAdmin)
        {
            var userExists = _context.Users.Any(u => u.Username == username);
            if (userExists) return false;

            var user = new User
            {
                Username = username,
                Password = password,
                Role = isAdmin
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public User AuthenticateUser(string username, string password)
        {
            return _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public bool DeleteUser(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }
    }
}