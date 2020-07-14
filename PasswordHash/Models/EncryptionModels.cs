using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordHash.Models
{
    public class Password
    {
        public string Algorithm { get; set; }
        public string CleartextPsw { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
    }
}
