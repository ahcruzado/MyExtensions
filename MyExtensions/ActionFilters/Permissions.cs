using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyExtensions.ActionFilters
{
    [Flags]
    public enum Permissions
    {
        View = (1 << 0),
        Add = (1 << 1),
        Edit = (1 << 2),
        Delete = (1 << 3),
        
        Write = Add | Edit | Delete,
        Admin = View | Add | Edit | Delete
    }
}