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

using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace OSGCommunicationTelemetryTool
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private string inputFolder;

        private string outputFolder;

        public MainWindowViewModel()
        {
            InstrumentCommand = new DelegateCommand(this.Instrument);
            OutputFolder = ConfigurationManager.AppSettings["outputFolder"];
            InputFolder = ConfigurationManager.AppSettings["inputFolder"];
        }

        private void Instrument()
        {
            try
            {
                if (!Directory.Exists(inputFolder))
                {
                    MessageBox.Show("Input folder does not exist.");
                    return;
                }

                string message;
                if (Directory.Exists(outputFolder))
                {
                    message = "Files in output folder will be overwritten; files not in input folder will be deleted. Proceed?";
                }
                else
                {
                    message = "Output folder does not exist and will be created. Proceed?";
                }

                var result = MessageBox.Show(message, "Confirmation", MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                {
                    return;
                }

                var outputDir = new DirectoryInfo(outputFolder);
                if (outputDir.Exists)
                {
                    outputDir.Delete(true);
                }
                outputDir.Create();

                FileUtility.CopyDirectory(inputFolder, outputFolder, true);
                var htmlFiles = outputDir.GetFiles("*.htm*", SearchOption.AllDirectories);
                if (htmlFiles.Length == 0)
                {
                    MessageBox.Show("Files successfully copied but no html file was found.");
                    return;
                }

                message = "Files successfully copied. " + htmlFiles.Length + " html files are found and will be instrumented:\n" +
                          string.Join("\n", htmlFiles.Select(f => f.FullName));
                result = MessageBox.Show(message, "Confirmation", MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                {
                    return;
                }

                foreach (var file in htmlFiles)
                {
                    TelemetryUtility.Instrument(file.FullName, file.FullName, file.Name);
                }

                MessageBox.Show("Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected exception was thrown.\n" + ex);
            }
        }

        public string InputFolder
        {
            get { return this.inputFolder; }
            set { this.SetField("InputFolder", ref this.inputFolder, value); }
        }

        public string OutputFolder
        {
            get { return this.outputFolder; }
            set { this.SetField("OutputFolder", ref this.outputFolder, value); }
        }

        public DelegateCommand InstrumentCommand { get; private set; }
    }
}
