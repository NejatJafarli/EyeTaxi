using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTaxi.Models
{
    public class User
    {
        public User(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
            History= new List<History>();
        }
        public List<History> History { get; set; }
        public string Username { get; set; }
        public string Password {  get; set; }
        public string Email {  get; set; }


    }
}
