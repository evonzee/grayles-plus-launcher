using System;
using System.Collections.Generic;
using System.Text;
using g = GraylesPlus;
using ReactiveUI;
using System.Reactive;
using System.ComponentModel;

namespace GraylesGui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel() {
            this.Config = g.Config.Load();
            this.SetupCommands();
        }



        private g.Config config;
        public g.Config Config { 
            get  => this.config;
            private set { 
                this.RaiseAndSetIfChanged(ref config, value);
                this.Mods = new g.Mods(value);
                this.Starbound = new g.Starbound(value);
            }
        }

        private g.Mods mods;
        public g.Mods Mods {
            get => this.Mods;
            private set => this.RaiseAndSetIfChanged(ref mods, value);
        }

        private g.Starbound starbound;
        public g.Starbound Starbound {
            get => this.starbound;
            private set => this.RaiseAndSetIfChanged(ref starbound, value);
        }

        public string StarboundFound { get { return this.Starbound.StarboundExecutableFolder != null ? "Yes" : "No"; }}

        public string GraylesSet { get { return  this.Config.GraylesRoot != null ? $"Yes" : "No"; }}

        public string ModsDownloaded { get { return  this.Mods.Downloaded ? $"Yes, {Mods.TargetVersion}" : "No"; }}

        public string ModsInstalled { get { return  this.Mods.Installed ? $"Yes, {Mods.InstalledVersion}" : "No"; }}

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
            this.Mods = this.Mods.With(targetVersion: g.Mods.GetLatestVersion());
        }

        void RunLaunch(){

        }
        
        void RunInstall(){

        }

        void RunFindStarbound() {

        }

        void RunFindGrayles() {

        }


    }
}
