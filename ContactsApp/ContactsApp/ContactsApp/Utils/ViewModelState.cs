using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Utils
{
    public enum ViewModelState
    {
        Normal,
        Loading,
        Empty,
        Error,
        PermissionDenied,
    }
}
