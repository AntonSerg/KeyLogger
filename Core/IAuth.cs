using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyLogger.Core
{
    interface IAuth
    {
        string login { get; set; }
        string password { set; get; }
        string UID { set; get; }
        bool Auth();
    }
}
