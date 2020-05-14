using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyLogger.Core
{
    interface IKeyLogger
    {
        string UID { get; set; }
        void Start();
        void Loging();
    }
}
