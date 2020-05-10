using assemblyAnalyze.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace assemblyAnalyze
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public DisplayRootRegistry displayRootRegistry = new DisplayRootRegistry();
        AnalyzerViewModel mainWindowViewModel;

        public App()
        {
            displayRootRegistry.RegisterWindowType<AnalyzerViewModel, MainWindow>();
            displayRootRegistry.RegisterWindowType<SavePartViewModel, SavePart>();
            displayRootRegistry.RegisterWindowType<ConfirmActionViewModel, ConfirmAction>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            mainWindowViewModel = new AnalyzerViewModel();
            await displayRootRegistry.ShowModalPresentation(mainWindowViewModel);
            Shutdown();
        }
    }
}
