using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.DTO
{
    public class PasswordHash
    {

        public string Password { get; set; }

        public string Salt { get; set; }

    }
}
