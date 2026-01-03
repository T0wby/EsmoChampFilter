using EsmoChamps.Commands;
using EsmoChamps.Data;
using EsmoChamps.Interfaces;
using EsmoChamps.Models;
using EsmoChamps.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace EsmoChamps.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties
        private readonly AppDbContext _context;
        public ObservableCollection<Champion> Champions { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private Role _selectedRole;
        public Role SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
        }

        private RangeType _selectedRangeType;
        public RangeType SelectedRangeType
        {
            get => _selectedRangeType;
            set { _selectedRangeType = value; OnPropertyChanged(); }
        }

        private ChampType _selectedChampType;
        public ChampType SelectedChampType
        {
            get => _selectedChampType;
            set { _selectedChampType = value; OnPropertyChanged(); }
        }

        private Champion _selectedChampion;
        public Champion SelectedChampion
        {
            get => _selectedChampion;
            set { _selectedChampion = value; OnPropertyChanged(); }
        }

        private ICollectionView _championsView;
        public ICollectionView ChampionsView
        {
            get => _championsView;
            private set
            {
                _championsView = value;
                OnPropertyChanged();
            }
        }

        private readonly IConfirmationService _confirmationService = new Services.ConfirmationService();
        #endregion

        #region Filter Properties
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private string _strengthSearchText = string.Empty;
        public string StrengthSearchText
        {
            get => _strengthSearchText;
            set
            {
                _strengthSearchText = value;
                OnPropertyChanged();
                FilterStrengthsForFilter();
            }
        }

        private Role? _selectedRoleFilter;
        public Role? SelectedRoleFilter
        {
            get => _selectedRoleFilter;
            set
            {
                _selectedRoleFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private RangeType? _selectedRangeTypeFilter;
        public RangeType? SelectedRangeTypeFilter
        {
            get => _selectedRangeTypeFilter;
            set
            {
                _selectedRangeTypeFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private ChampType? _selectedChampTypeFilter;
        public ChampType? SelectedChampTypeFilter
        {
            get => _selectedChampTypeFilter;
            set
            {
                _selectedChampTypeFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public int? MechanicsMinFilter { get; set; }
        public int? MechanicsMaxFilter { get; set; }
        public int? MacroMinFilter { get; set; }
        public int? MacroMaxFilter { get; set; }
        public int? TacticalMinFilter { get; set; }
        public int? TacticalMaxFilter { get; set; }

        public ObservableCollection<Champion> AllChampions { get; set; }
        public ObservableCollection<Champion> FilteredChampions { get; set; }
        public ObservableCollection<Role> AvailableRoles { get; set; }
        public ObservableCollection<RangeType> AvailableRangeTypes { get; set; }
        public ObservableCollection<ChampType> AvailableChampTypes { get; set; }
        public ObservableCollection<StrengthSelectionItem> AllStrengthsForFilter { get; set; }
        public ObservableCollection<StrengthSelectionItem> FilteredStrengthsForFilter { get; set; }

        public int FilteredChampionCount => FilteredChampions?.Count ?? 0;
        #endregion

        #region Commands
        public ICommand OpenAddChampionCommand { get; }
        public ICommand EditChampionCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand OpenChampionDetailsCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        #endregion

        public MainViewModel()
        {
            _context = new AppDbContext();

            //Roles = new ObservableCollection<Role>(_context.Roles.ToList());
            //RangeTypes = new ObservableCollection<RangeType>(_context.RangeTypes.ToList());
            //ChampTypes = new ObservableCollection<ChampType>(_context.ChampTypes.ToList());

            //Champions = new ObservableCollection<Champion>(
            //    _context.Champions
            //      .Include(c => c.Role)
            //      .Include(c => c.RangeType)
            //      .Include(c => c.ChampType)
            //      .AsNoTracking()
            //      .OrderBy(c => c.Name)
            //      .ToList()
            //);

            //ChampionsView = CollectionViewSource.GetDefaultView(Champions);
            //ChampionsView.Filter = FilterChampions;

            OpenAddChampionCommand = new RelayCommand(_ => OpenAddChampion());
            DeleteCommand = new RelayCommand(_ => DeleteChampion(), _ => SelectedChampion != null);
            EditChampionCommand = new RelayCommand(_ => EditChampion(), _ => SelectedChampion != null);
            OpenChampionDetailsCommand = new RelayCommand(_ => OpenChampionDetails(SelectedChampion),_ => SelectedChampion != null);
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());

            LoadData();
        }

        private void LoadData()
        {
            // Load filter options
            AvailableRoles = new ObservableCollection<Role>(_context.Roles.ToList());
            AvailableRoles.Insert(0, new Role { Id = 0, Name = "All Roles" });

            AvailableRangeTypes = new ObservableCollection<RangeType>(_context.RangeTypes.ToList());
            AvailableRangeTypes.Insert(0, new RangeType { Id = 0, Name = "All Ranges" });

            AvailableChampTypes = new ObservableCollection<ChampType>(_context.ChampTypes.OrderBy(c => c.Name).ToList());
            AvailableChampTypes.Insert(0, new ChampType { Id = 0, Name = "All Types" });

            // Load strengths for filtering
            var strengths = _context.StrengthTitles
                .OrderBy(s => s.Title)
                .Select(s => new StrengthSelectionItem
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    IsSelected = false
                }).ToList();

            AllStrengthsForFilter = new ObservableCollection<StrengthSelectionItem>(strengths);
            FilteredStrengthsForFilter = new ObservableCollection<StrengthSelectionItem>(strengths);

            foreach (var strength in AllStrengthsForFilter)
            {
                strength.PropertyChanged += StrengthFilter_PropertyChanged;
            }

            ReloadChampions();

            // Set defaults
            SelectedRoleFilter = AvailableRoles[0];
            SelectedRangeTypeFilter = AvailableRangeTypes[0];
            SelectedChampTypeFilter = AvailableChampTypes[0];
        }

        private void StrengthFilter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StrengthSelectionItem.IsSelected))
            {
                ApplyFilters();
            }
        }

        private void FilterStrengthsForFilter()
        {
            FilteredStrengthsForFilter.Clear();

            var filtered = string.IsNullOrWhiteSpace(StrengthSearchText)
                ? AllStrengthsForFilter
                : AllStrengthsForFilter.Where(s =>
                    s.Title.Contains(StrengthSearchText, StringComparison.OrdinalIgnoreCase) ||
                    s.Description.Contains(StrengthSearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var strength in filtered)
            {
                FilteredStrengthsForFilter.Add(strength);
            }
        }

        private void DeleteChampion()
        {
            if (!_confirmationService.Confirm($"Delete '{SelectedChampion.Name}'?","Confirm Delete"))
            {
                return;
            }
            using var db = new AppDbContext();
            db.Champions.Remove(SelectedChampion);
            db.SaveChanges();

            Champions.Remove(SelectedChampion);
        }

        private void ReloadChampions()
        {
            AllChampions = new ObservableCollection<Champion>(
                _context.Champions
                    .Include(c => c.Role)
                    .Include(c => c.RangeType)
                    .Include(c => c.ChampType)
                    .Include(c => c.Strengths)
                    .ThenInclude(cs => cs.StrengthTitle)
                    .ToList()
            );

            ApplyFilters();
        }

        #region Filter Methods
        private void ApplyFilters()
        {
            var filtered = AllChampions.AsEnumerable();

            // Text search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(c =>
                    c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Role filter
            if (SelectedRoleFilter != null && SelectedRoleFilter.Id != 0)
            {
                filtered = filtered.Where(c => c.RoleId == SelectedRoleFilter.Id);
            }

            // Range Type filter
            if (SelectedRangeTypeFilter != null && SelectedRangeTypeFilter.Id != 0)
            {
                filtered = filtered.Where(c => c.RangeTypeId == SelectedRangeTypeFilter.Id);
            }

            // Champion Type filter
            if (SelectedChampTypeFilter != null && SelectedChampTypeFilter.Id != 0)
            {
                filtered = filtered.Where(c => c.ChampTypeId == SelectedChampTypeFilter.Id);
            }

            // Difficulty filters
            if (MechanicsMinFilter.HasValue)
                filtered = filtered.Where(c => c.MechanicsMax >= MechanicsMinFilter.Value);
            if (MechanicsMaxFilter.HasValue)
                filtered = filtered.Where(c => c.MechanicsMin <= MechanicsMaxFilter.Value);

            if (MacroMinFilter.HasValue)
                filtered = filtered.Where(c => c.MacroMax >= MacroMinFilter.Value);
            if (MacroMaxFilter.HasValue)
                filtered = filtered.Where(c => c.MacroMin <= MacroMaxFilter.Value);

            if (TacticalMinFilter.HasValue)
                filtered = filtered.Where(c => c.TacticalMax >= TacticalMinFilter.Value);
            if (TacticalMaxFilter.HasValue)
                filtered = filtered.Where(c => c.TacticalMin <= TacticalMaxFilter.Value);

            // Strength filters
            var selectedStrengthIds = AllStrengthsForFilter.Where(s => s.IsSelected).Select(s => s.Id).ToList();
            if (selectedStrengthIds.Any())
            {
                filtered = filtered.Where(c =>
                    selectedStrengthIds.All(strengthId =>
                        c.Strengths.Any(cs => cs.StrengthTitleId == strengthId)));
            }

            FilteredChampions = new ObservableCollection<Champion>(filtered);
            OnPropertyChanged(nameof(FilteredChampions));
            OnPropertyChanged(nameof(FilteredChampionCount));
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedRoleFilter = AvailableRoles[0];
            SelectedRangeTypeFilter = AvailableRangeTypes[0];
            SelectedChampTypeFilter = AvailableChampTypes[0];
            MechanicsMinFilter = null;
            MechanicsMaxFilter = null;
            MacroMinFilter = null;
            MacroMaxFilter = null;
            TacticalMinFilter = null;
            TacticalMaxFilter = null;

            foreach (var strength in AllStrengthsForFilter)
            {
                strength.IsSelected = false;
            }

            ApplyFilters();
        }
        #endregion

        #region Open extra windows
        private void OpenAddChampion()
        {
            var vm = new AddChampionViewModel(champ =>
            {
                ReloadChampions();
            });
            var window = new AddChampionWindow(vm);
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void EditChampion()
        {
            var vm = new AddChampionViewModel(
                SelectedChampion,
                updated =>
                {
                    ReloadChampions();
                });

            var window = new AddChampionWindow(vm);
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void OpenChampionDetails(Champion? champion)
        {
            try
            {
                if (champion == null) return;

                SelectedChampion = champion;
                var vm = new ChampionDetailViewModel(SelectedChampion);
                var window = new ChampionDetailWindow(vm);
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening detail window: {ex.Message}\n\n{ex.StackTrace}");
            }
        }
        #endregion
    }
}
