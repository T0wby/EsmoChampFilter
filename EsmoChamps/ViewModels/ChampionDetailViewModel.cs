using EsmoChamps.Commands;
using EsmoChamps.Models;
using System.Windows.Input;

namespace EsmoChamps.ViewModels
{
    public class ChampionDetailViewModel : BaseViewModel
    {
        #region Properties
        public Champion Champion { get; }

        public string Name => Champion.Name;
        public string ImagePath => Champion.ImagePath;
        public string Role => Champion.Role?.Name;
        public string Range => Champion.RangeType?.Name;
        public string Type => Champion.ChampType?.Name;

        public int MechanicsMin => Champion.MechanicsMin;
        public int MechanicsMax => Champion.MechanicsMax;
        public int MacroMin => Champion.MacroMin;
        public int MacroMax => Champion.MacroMax;
        public int TacticalMin => Champion.TacticalMin;
        public int TacticalMax => Champion.TacticalMax;

        public int PowerStart => Champion.PowerCurveStart;
        public int PowerMid => Champion.PowerCurveMid;
        public int PowerEnd => Champion.PowerCurveEnd;
        #endregion
        public event Action? RequestClose;

        #region Commands

        public ICommand CloseCommand { get; }
        #endregion

        public ChampionDetailViewModel(Champion champion)
        {
            Champion = champion;
            CloseCommand = new RelayCommand(_ => RequestClose?.Invoke());
        }
    }
}
