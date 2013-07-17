using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyExtensions
{
    [Flags]
    public enum Role
    {
        Admin = 1,
        Agent = 2,
        User = 4
    }
}