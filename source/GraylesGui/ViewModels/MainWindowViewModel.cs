using System;
using g = GraylesPlus;
using ReactiveUI;
using System.Reactive;

namespace GraylesGui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel()
        {
            this.Config = g.Config.Load();
            this.SetupCommands();
        }

        private g.Config _config;
        public g.Config Config { 
            get  => this._config;
            private set { 
                this.RaiseAndSetIfChanged(ref _config, value);
                this.Mods = new g.Mods(value);
                this.Starbound = new g.Starbound(value);
            }
        }

        private g.Mods _mods;
        public g.Mods Mods {
            get => this._mods;
            private set => this.RaiseAndSetIfChanged(ref _mods, value);
        }

        private g.Starbound _starbound;
        public g.Starbound Starbound {
            get => this._starbound;
            private set => this.RaiseAndSetIfChanged(ref this._starbound, value);
        }


        public string StarboundFound { get { return this.Starbound.StarboundExecutableFolder != null ? "Yes" : "No"; }}

        public string GraylesSet { get { return this.Config.GraylesRoot != null ? $"Yes" : "No"; }}

        public string ModsDownloaded { get { return this.Mods.Downloaded ? $"Yes, {Mods.TargetVersion}" : "No"; }}

        public string ModsInstalled { get { return this.Mods.Installed ? $"Yes, {Mods.InstalledVersion}" : "No"; }}

        private void SetupCommands(){
            CheckForUpdates = ReactiveCommand.Create(RunCheckForUpdates);
            Launch = ReactiveCommand.Create(RunLaunch);
            Install = ReactiveCommand.Create(RunInstall);
            FindStarbound = ReactiveCommand.Create(RunFindStarbound);
            FindGrayles = ReactiveCommand.Create(RunFindGrayles);
        }

        public ReactiveCommand<Unit, Unit> CheckForUpdates { get; private set; }
        public ReactiveCommand<Unit, Unit> Launch { get; private set; }
        public ReactiveCommand<Unit, Unit> Install { get; private set; }
        public ReactiveCommand<Unit, Unit> FindStarbound { get; private set; }
        public ReactiveCommand<Unit, Unit> FindGrayles { get; private set; }

        void RunCheckForUpdates(){
            Console.WriteLine("Checking for updates..");
            this.Mods = this.Mods.With(targetVersion: g.Mods.GetLatestVersion());
        }

        void RunLaunch()
        {
            Console.WriteLine("Launching..");

        }
        
        void RunInstall()
        {
            Console.WriteLine("Installing..");

        }

        void RunFindStarbound()
        {
            Console.WriteLine("Finding Starbound..");

        }

        void RunFindGrayles()
        {
            Console.WriteLine("Finding Grayles..");

        }


    }
}
