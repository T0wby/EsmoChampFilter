using EsmoChamps.Commands;
using EsmoChamps.Data;
using EsmoChamps.Models;
using EsmoChamps.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace EsmoChamps.ViewModels
{
    public class AddChampionViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        #region Properties
        public ObservableCollection<Role> Roles { get; private set; }
        public ObservableCollection<RangeType> RangeTypes { get; private set; }
        public ObservableCollection<ChampType> ChampTypes { get; private set; }

        // Strengths
        public ObservableCollection<StrengthSelectionItem> AllStrengths { get; set; }
        public ObservableCollection<StrengthSelectionItem> FilteredStrengths { get; set; }

        private string _strengthSearchText = string.Empty;
        public string StrengthSearchText
        {
            get => _strengthSearchText;
            set
            {
                _strengthSearchText = value;
                OnPropertyChanged();
                FilterStrengths();
            }
        }

        public int SelectedStrengthsCount => AllStrengths?.Count(s => s.IsSelected) ?? 0;

        public bool IsEditMode { get; }
        public int ChampionId { get; }

        public string Name { get; set; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        private int _selectedRoleId;
        public int SelectedRoleId
        {
            get => _selectedRoleId;
            set
            {
                _selectedRoleId = value;
                OnPropertyChanged();
            }
        }

        private Role _selectedRole;
        public Role SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
        }

        private int _selectedRangeTypeId;
        public int SelectedRangeTypeId
        {
            get => _selectedRangeTypeId;
            set
            {
                _selectedRangeTypeId = value;
                OnPropertyChanged();
            }
        }
        private RangeType _selectedRangeType;
        public RangeType SelectedRangeType
        {
            get => _selectedRangeType;
            set { _selectedRangeType = value; OnPropertyChanged(); }
        }

        private int _selectedChampTypeId;
        public int SelectedChampTypeId
        {
            get => _selectedChampTypeId;
            set
            {
                _selectedChampTypeId = value;
                OnPropertyChanged();
            }
        }
        private ChampType _selectedChampType;
        public ChampType SelectedChampType
        {
            get => _selectedChampType;
            set { _selectedChampType = value; OnPropertyChanged(); }
        }

        public int MechanicsMin { get; set; }
        public int MechanicsMax { get; set; }
        public int MacroMin { get; set; }
        public int MacroMax { get; set; }
        public int TacticalMin { get; set; }
        public int TacticalMax { get; set; }
        public int PowerCurveStart { get; set; }
        public int PowerCurveMid { get; set; }
        public int PowerCurveEnd { get; set; }
        #endregion

        public event Action? RequestClose;

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region Error Handling
        private readonly Dictionary<string, List<string>> _errors = new();
        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.SelectMany(e => e.Value);

            return _errors.ContainsKey(propertyName)
                ? _errors[propertyName]
                : null;
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructors
        public AddChampionViewModel(Action<Champion> onSave)
        {
            LoadTypes();
            LoadStrengths();

            SaveCommand = new RelayCommand(_ => Save(onSave));

            CancelCommand = new RelayCommand(_ =>
            {
                RequestClose?.Invoke();
            });
        }

        public AddChampionViewModel(Champion champ, Action<Champion> onSave)
        {
            LoadTypes();

            IsEditMode = true;
            ChampionId = champ.Id;

            Name = champ.Name;
            ImagePath = champ.ImagePath;
            SelectedRoleId = champ.RoleId;
            SelectedRangeTypeId = champ.RangeTypeId;
            SelectedChampTypeId = champ.ChampTypeId;

            MechanicsMin = champ.MechanicsMin;
            MechanicsMax = champ.MechanicsMax;
            MacroMin = champ.MacroMin;
            MacroMax = champ.MacroMax;
            TacticalMin = champ.TacticalMin;
            TacticalMax = champ.TacticalMax;

            PowerCurveStart = champ.PowerCurveStart;
            PowerCurveMid = champ.PowerCurveMid;
            PowerCurveEnd = champ.PowerCurveEnd;

            LoadStrengths(champ.Id);

            SaveCommand = new RelayCommand(_ => Save(onSave));
            CancelCommand = new RelayCommand(_ =>
            {
                RequestClose?.Invoke();
            });
        }
        #endregion

        private void LoadTypes()
        {
            using var db = new AppDbContext();
            Roles = new ObservableCollection<Role>(db.Roles.ToList());
            RangeTypes = new ObservableCollection<RangeType>(db.RangeTypes.OrderBy(n => n.Name).ToList());
            ChampTypes = new ObservableCollection<ChampType>(db.ChampTypes.OrderBy(n => n.Name).ToList());
        }

        private void LoadStrengths(int? championId = null)
        {
            using var db = new AppDbContext();

            // Get all strength titles
            var allTitles = db.StrengthTitles.OrderBy(s => s.Title).ToList();

            // Get selected strengths for this champion
            var selectedValues = championId.HasValue
                ? db.ChampionStrengths
                    .Where(cs => cs.ChampionId == championId.Value)
                    .ToDictionary(cs => cs.StrengthTitleId, cs => cs.Value)
                : new Dictionary<int, int>();

            // Create the list - each strength appears ONCE
            var strengths = allTitles.Select(t => new StrengthSelectionItem
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsSelected = selectedValues.ContainsKey(t.Id),
                Value = selectedValues.ContainsKey(t.Id) ? selectedValues[t.Id] : 0
            }).ToList();

            // Clear and repopulate to avoid duplicates
            if (AllStrengths != null)
            {
                foreach (var strength in AllStrengths)
                {
                    strength.PropertyChanged -= Strength_PropertyChanged;
                }
            }

            AllStrengths = new ObservableCollection<StrengthSelectionItem>(strengths);
            FilteredStrengths = new ObservableCollection<StrengthSelectionItem>();

            // Subscribe to property changes
            foreach (var strength in AllStrengths)
            {
                strength.PropertyChanged += Strength_PropertyChanged;
            }

            // Apply initial filter (which will populate FilteredStrengths)
            FilterStrengths();
        }

        private void Strength_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StrengthSelectionItem.IsSelected))
            {
                OnPropertyChanged(nameof(SelectedStrengthsCount));
            }
        }

        private void FilterStrengths()
        {
            // Debug: Check for duplicates in AllStrengths
            var duplicates = AllStrengths.GroupBy(s => s.Id).Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                System.Diagnostics.Debug.WriteLine($"WARNING: Found {duplicates.Count} duplicate IDs in AllStrengths!");
                foreach (var dup in duplicates)
                {
                    System.Diagnostics.Debug.WriteLine($"  ID {dup.Key}: {dup.First().Title} appears {dup.Count()} times");
                }
            }

            FilteredStrengths.Clear();

            if (string.IsNullOrWhiteSpace(StrengthSearchText))
            {
                foreach (var strength in AllStrengths)
                {
                    FilteredStrengths.Add(strength);
                }
            }
            else
            {
                var filtered = AllStrengths
                    .Where(s => s.Title.Contains(StrengthSearchText, StringComparison.OrdinalIgnoreCase) ||
                               s.Description.Contains(StrengthSearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var strength in filtered)
                {
                    FilteredStrengths.Add(strength);
                }
            }

            System.Diagnostics.Debug.WriteLine($"FilteredStrengths count: {FilteredStrengths.Count}");
        }

        private bool Validate()
        {
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(Name))
                AddError(nameof(Name), "Name is required");

            if (SelectedRoleId == 0)
                AddError(nameof(SelectedRole), "Role is required");

            if (SelectedRangeTypeId == 0)
                AddError(nameof(SelectedRangeType), "Range type is required");

            if (SelectedChampTypeId == 0)
                AddError(nameof(SelectedChampType), "Champion type is required");

            if (MechanicsMin > MechanicsMax)
                AddError(nameof(MechanicsMin), "Mechanics min must be ≤ max");

            if (MacroMin > MacroMax)
                AddError(nameof(MacroMin), "Macro min must be ≤ max");

            if (TacticalMin > TacticalMax)
                AddError(nameof(TacticalMin), "Tactical min must be ≤ max");

            return !HasErrors;
        }

        private void ShowValidationError()
        {
            var window = new NotificationWindow
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
        }

        private async void Save(Action<Champion> onSave)
        {
            if (!Validate())
            {
                ShowValidationError();
                return;
            }

            using var db = new AppDbContext();

            var champ = new Champion
            {
                Id = ChampionId,
                Name = Name,
                ImagePath = ImagePath,
                RoleId = SelectedRoleId,
                RangeTypeId = SelectedRangeTypeId,
                ChampTypeId = SelectedChampTypeId,
                MechanicsMin = MechanicsMin,
                MechanicsMax = MechanicsMax,
                MacroMin = MacroMin,
                MacroMax = MacroMax,
                TacticalMin = TacticalMin,
                TacticalMax = TacticalMax,
                PowerCurveStart = PowerCurveStart,
                PowerCurveMid = PowerCurveMid,
                PowerCurveEnd = PowerCurveEnd,
            };

            if (IsEditMode)
            {
                db.Champions.Update(champ);

                var existingStrengths = db.ChampionStrengths
                    .Where(cs => cs.ChampionId == ChampionId)
                    .ToList();
                db.ChampionStrengths.RemoveRange(existingStrengths);
            }
            else
            {
                db.Champions.Add(champ);
            }

            db.SaveChanges();

            var selectedStrengths = AllStrengths.Where(s => s.IsSelected).ToList();

            foreach (var strength in selectedStrengths)
            {
                var championStrength = new ChampionStrength
                {
                    ChampionId = champ.Id,  // Now we have the ID from SaveChanges
                    StrengthTitleId = strength.Id,  // Use the ID, not the object
                    Value = strength.Value
                };

                db.ChampionStrengths.Add(championStrength);
            }

            await db.SaveChangesAsync();

            onSave?.Invoke(champ);

            RequestClose?.Invoke();
        }
    }
}