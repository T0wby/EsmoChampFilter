using System;
using System.Collections.Generic;
using System.Text;

namespace EsmoChamps.ViewModels
{
    public class StrengthSelectionItem : BaseViewModel
    {
        private bool _isSelected;
        private int _value;

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    if (!value)
                    {
                        Value = 0;
                    }
                }
            }
        }

        public int Value
        {
            get => _value;
            set
            {
                // Clamp value between 0 and 100
                int clampedValue = Math.Max(0, Math.Min(100, value));

                if (_value != clampedValue)
                {
                    _value = clampedValue;
                    OnPropertyChanged();
                }
            }
        }
    }
}
