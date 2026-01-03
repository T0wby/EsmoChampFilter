using System;
using System.Collections.Generic;
using System.Text;

namespace EsmoChamps.Interfaces
{
    public interface IConfirmationService
    {
        bool Confirm(string message, string title);
    }
}
