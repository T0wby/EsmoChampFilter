using EsmoChamps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace EsmoChamps.Services
{
    public class ConfirmationService : IConfirmationService
    {
        public bool Confirm(string message, string title)
        {
            return MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }
    }
}
