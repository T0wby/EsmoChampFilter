using EsmoChamps.Commands;
using EsmoChamps.Data;
using EsmoChamps.Interfaces;
using EsmoChamps.Models;
using EsmoChamps.Utility;
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
        public Champion? SelectedChampion
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
                ApplySorting();
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
                ApplySorting();
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
                ApplySorting();
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
                ApplySorting();
            }
        }

        private DefaultSortingOptions? _selectedSortingOption;
        public DefaultSortingOptions? SelectedSortingOption
        {
            get => _selectedSortingOption;
            set
            {
                _selectedSortingOption = value;
                OnPropertyChanged();
                ApplySorting();
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
        public ObservableCollection<DefaultSortingOptions> AvailableSortingOptions { get; set; }

        public int FilteredChampionCount => FilteredChampions?.Count ?? 0;
        #endregion

        #region Strength Filter and Sort Properties
        private StrengthTitle? _selectedStrengthSort;
        public StrengthTitle? SelectedStrengthSort
        {
            get => _selectedStrengthSort;
            set
            {
                _selectedStrengthSort = value;
                OnPropertyChanged();
                ApplySorting();
            }
        }

        private bool _sortStrengthAscending = false;
        public bool SortStrengthAscending
        {
            get => _sortStrengthAscending;
            set
            {
                _sortStrengthAscending = value;
                OnPropertyChanged();
                ApplySorting();
            }
        }

        private StrengthTitle? _selectedStrengthValueFilter;
        public StrengthTitle? SelectedStrengthValueFilter
        {
            get => _selectedStrengthValueFilter;
            set
            {
                _selectedStrengthValueFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private int? _strengthMinValueFilter;
        public int? StrengthMinValueFilter
        {
            get => _strengthMinValueFilter;
            set
            {
                _strengthMinValueFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private int? _strengthMaxValueFilter;
        public int? StrengthMaxValueFilter
        {
            get => _strengthMaxValueFilter;
            set
            {
                _strengthMaxValueFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ObservableCollection<StrengthTitle> AvailableStrengthsForSort { get; set; }
        public ObservableCollection<StrengthTitle> AvailableStrengthsForValueFilter { get; set; }
        #endregion

        #region Commands
        public ICommand OpenAddChampionCommand { get; }
        public ICommand EditChampionCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand OpenChampionDetailsCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand OpenImagesFolderCommand { get; }
        #endregion

        public MainViewModel()
        {
            _context = new AppDbContext();

            OpenAddChampionCommand = new RelayCommand(_ => OpenAddChampion());
            DeleteCommand = new RelayCommand(_ => DeleteChampion(), _ => SelectedChampion != null);
            EditChampionCommand = new RelayCommand(_ => EditChampion(), _ => SelectedChampion != null);
            OpenChampionDetailsCommand = new RelayCommand(_ => OpenChampionDetails(SelectedChampion),_ => SelectedChampion != null);
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            OpenImagesFolderCommand = new RelayCommand(_ => OpenImagesFolder());

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

            AvailableSortingOptions = new ObservableCollection<DefaultSortingOptions>
            {
                DefaultSortingOptions.None,
                DefaultSortingOptions.NameAsc,
                DefaultSortingOptions.NameDesc,
                DefaultSortingOptions.RoleAsc,
                DefaultSortingOptions.RoleDesc,
                DefaultSortingOptions.EarlyGameAsc,
                DefaultSortingOptions.EarlyGameDesc,
                DefaultSortingOptions.MidGameAsc,
                DefaultSortingOptions.MidGameDesc,
                DefaultSortingOptions.LateGameAsc,
                DefaultSortingOptions.LateGameDesc
            };

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

            var strengthTitlesForSort = _context.StrengthTitles.OrderBy(s => s.Title).ToList();
            AvailableStrengthsForSort = new ObservableCollection<StrengthTitle>(strengthTitlesForSort);
            AvailableStrengthsForSort.Insert(0, new StrengthTitle { Id = 0, Title = "No Strength Sort" });

            // Load strengths for value filtering
            var strengthTitlesForFilter = _context.StrengthTitles.OrderBy(s => s.Title).ToList();
            AvailableStrengthsForValueFilter = new ObservableCollection<StrengthTitle>(strengthTitlesForFilter);
            AvailableStrengthsForValueFilter.Insert(0, new StrengthTitle { Id = 0, Title = "No Strength Filter" });


            ReloadChampions();

            //AllChampions = new ObservableCollection<Champion>((AllChampions ?? new ObservableCollection<Champion>()).OrderBy(c => c.Name).ToList());

            // Set defaults
            SelectedRoleFilter = AvailableRoles[0];
            SelectedRangeTypeFilter = AvailableRangeTypes[0];
            SelectedChampTypeFilter = AvailableChampTypes[0];
            SelectedSortingOption = AvailableSortingOptions[0];
            SelectedStrengthSort = AvailableStrengthsForSort[0];
            SelectedStrengthValueFilter = AvailableStrengthsForValueFilter[0];
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
            using var db = new AppDbContext();

            AllChampions?.Clear();
            AllChampions = new ObservableCollection<Champion>(
                db.Champions
                    .Include(c => c.Role)
                    .Include(c => c.RangeType)
                    .Include(c => c.ChampType)
                    .Include(c => c.Strengths)
                    .ThenInclude(cs => cs.StrengthTitle)
                    .AsNoTracking()  // For better performance
                    .OrderBy(c => c.Name)
                    .ToList()
            );

            ApplyFilters();
        }

        #region Filter Methods
        private void StrengthFilter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StrengthSelectionItem.IsSelected))
            {
                ApplyFilters();
                ApplySorting();
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

        private void ApplySorting()
        {

            if (FilteredChampions == null || !FilteredChampions.Any()) return;

            if (SelectedStrengthSort != null && SelectedStrengthSort.Id != 0)
            {
                if (SortStrengthAscending)
                {
                    FilteredChampions = new ObservableCollection<Champion>(
                        FilteredChampions.OrderBy(c =>
                        {
                            var strength = c.Strengths.FirstOrDefault(s => s.StrengthTitleId == SelectedStrengthSort.Id);
                            return strength?.Value ?? -1; // Champions without this strength go to the bottom
                        })
                    );
                }
                else
                {
                    FilteredChampions = new ObservableCollection<Champion>(
                        FilteredChampions.OrderByDescending(c =>
                        {
                            var strength = c.Strengths.FirstOrDefault(s => s.StrengthTitleId == SelectedStrengthSort.Id);
                            return strength?.Value ?? -1; // Champions without this strength go to the bottom
                        })
                    );
                }
            }
            else if (SelectedSortingOption != null)
            {
                switch (SelectedSortingOption)
                {
                    case DefaultSortingOptions.None:
                    case DefaultSortingOptions.NameAsc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderBy(c => c.Name));
                        break;
                    case DefaultSortingOptions.NameDesc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderByDescending(c => c.Name));
                        break;
                    case DefaultSortingOptions.RoleAsc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderBy(c => c.Role.Name));
                        break;
                    case DefaultSortingOptions.RoleDesc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderByDescending(c => c.Role.Name));
                        break;
                    case DefaultSortingOptions.EarlyGameAsc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderBy(c => c.PowerCurveStart));
                        break;
                    case DefaultSortingOptions.EarlyGameDesc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderByDescending(c => c.PowerCurveStart));
                        break;
                    case DefaultSortingOptions.MidGameAsc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderBy(c => c.PowerCurveMid));
                        break;
                    case DefaultSortingOptions.MidGameDesc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderByDescending(c => c.PowerCurveMid));
                        break;
                    case DefaultSortingOptions.LateGameAsc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderBy(c => c.PowerCurveEnd));
                        break;
                    case DefaultSortingOptions.LateGameDesc:
                        FilteredChampions = new ObservableCollection<Champion>(FilteredChampions.OrderByDescending(c => c.PowerCurveEnd));
                        break;
                    default:
                        break;
                }
            }

            
            OnPropertyChanged(nameof(FilteredChampions));
            OnPropertyChanged(nameof(FilteredChampionCount));
        }

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

            if (SelectedStrengthValueFilter != null && SelectedStrengthValueFilter.Id != 0)
            {
                filtered = filtered.Where(c =>
                {
                    var strength = c.Strengths.FirstOrDefault(s => s.StrengthTitleId == SelectedStrengthValueFilter.Id);
                    if (strength == null) return false;

                    bool matchesMin = !StrengthMinValueFilter.HasValue || strength.Value >= StrengthMinValueFilter.Value;
                    bool matchesMax = !StrengthMaxValueFilter.HasValue || strength.Value <= StrengthMaxValueFilter.Value;

                    return matchesMin && matchesMax;
                });
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
            SelectedSortingOption = AvailableSortingOptions[0];
            SelectedStrengthSort = AvailableStrengthsForSort[0];
            SelectedStrengthValueFilter = AvailableStrengthsForValueFilter[0];
            SortStrengthAscending = false;

            MechanicsMinFilter = null;
            MechanicsMaxFilter = null;
            MacroMinFilter = null;
            MacroMaxFilter = null;
            TacticalMinFilter = null;
            TacticalMaxFilter = null;
            StrengthMinValueFilter = null;
            StrengthMaxValueFilter = null;

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
                SelectedChampion = null;
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
                    SelectedChampion = null;
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

        private void OpenImagesFolder()
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", ImageManager.UserImagesFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open folder: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
