using System;
using ReactiveUI;
using g = GraylesPlus;

namespace GraylesGui.ViewModels
{
    public class DownloadAdviceViewModel : ViewModelBase
    {
        public DownloadAdviceViewModel()
        {
        }

        private g.Mods _mods;
        public g.Mods Mods
        {
            get => this._mods;
            set => this.RaiseAndSetIfChanged(ref _mods, value);
        }
    }
}
