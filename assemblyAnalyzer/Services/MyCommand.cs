﻿using assemblyAnalyze;
using assemblyAnalyze.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace assemblyAnalyzer
{
    public abstract class MyCommand : ICommand
    {
        protected Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public MyCommand(Func<object, bool> canExecute = null)
        {
            this.canExecute = canExecute;
        }

        public MyCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public abstract void Execute(object parameter)
        ;
    }

    public class SimpleCommand : MyCommand
    {
        public SimpleCommand(Action<object> execute, Func<object, bool> canExecute = null) : base(execute, canExecute)
        {

        }

        public override void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

    public class SimpleCommandAsync : MyCommand
    {
        public SimpleCommandAsync(Action<object> execute, Func<object, bool> canExecute = null) : base(execute, canExecute)
        {

        }

        public override async void Execute(object parameter)
        {
            await Task.Run( ()=>execute(parameter) );
        }
    }

    public class OpenDialogWindowCommand: MyCommand
    {
        AnalyzerViewModel mainViewModel;

        public OpenDialogWindowCommand(AnalyzerViewModel mainVM, Action<object> execute, Func<object, bool> canExecute = null) : base (execute, canExecute)
        {
            mainViewModel = mainVM;
        }

        public override async void Execute(object parameter)
        {
            var displayRootRegistry = (Application.Current as App).displayRootRegistry;

            var savePartViewModel = new SavePartViewModel();
            //await Task.Run(() => displayRootRegistry.ShowPresentation(savePartViewModel));
            await displayRootRegistry.ShowModalPresentation(savePartViewModel);
            
            this.execute(savePartViewModel);
        }
    }
}
