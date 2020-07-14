using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordHash.Models
{
    public class User
    {
        public string Password { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
    }
}
