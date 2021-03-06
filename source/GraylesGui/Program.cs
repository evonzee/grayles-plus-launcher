﻿using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using GraylesGui.ViewModels;
using GraylesGui.Views;

namespace GraylesGui
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            var model = new MainWindowViewModel();
            var window = new MainWindow
            {
                DataContext = model,
            };
            model.Window = window;
            

            app.Run(window);
        }
    }
}
