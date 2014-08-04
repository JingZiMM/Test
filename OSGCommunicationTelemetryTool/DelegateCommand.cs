//  
// Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// 
// Use of this source code is subject to the terms of the Microsoft
// premium shared source license agreement under which you licensed
// this source code. If you did not accept the terms of the license
// agreement, you are not authorized to use this source code.
// For the terms of the license, please see the license agreement
// signed by you and Microsoft.
// THE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
//  

namespace System.Windows.Input
{
    /// <summary>
    /// A minimal implementation for ICommand interface that takes delegates for Execute() and CanExecute().
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> canExecute;

        private readonly Action execute;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute()
        {
            return this.canExecute == null || this.canExecute();
        }

        public void Execute()
        {
            this.execute();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute();
        }

        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
