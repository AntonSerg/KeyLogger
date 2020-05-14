using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyLogger.Core
{
    class Authentification : IAuth
    {
        public string login { get; set; }
        public string password {
            set
            {
            }
            get
            {
                return password;
            }
        }
        public string UID
        {
            set
            {

            }
            get
            {
                return UID;
            }
        }
        public Authentification(string log, string pass)
        {
            this.login = log;
            this.password = pass;
            this.UID = Convert.ToString(Convert.ToInt32(pass) * Convert.ToInt32(pass));
        }
        public bool Auth()
        {
            return true;
        }
    }
}
