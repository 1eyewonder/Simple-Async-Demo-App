using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simple_Async_Demo_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        //>>>>>>>>>>>>>>>>>
        //Code is placed here for simplicity purposes. Code should not be going in the main window class
        //>>>>>>>>>>>>>>>>>

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = DemoMethods.RunDownloadSync();
            PrintResults(results);

            watch.Stop();
            var elapsedMS = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMS}";
        }

        private async void executeASync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;

            executeASync.IsEnabled = false;
            executeASync.Content = "Running...";

            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var results = await DemoMethods.RunDownloadASync(progress, cts.Token);
                PrintResults(results);
            }

            catch (OperationCanceledException)
            {
                resultsWindow.Text += $"The async download was cancelled. {Environment.NewLine} ";
                cts = new CancellationTokenSource();
            }
            

            watch.Stop();
            var elapsedMS = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMS}";

            executeASync.IsEnabled = true;
            executeASync.Content = "Async Execute";
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            dashboardProgress.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }

        private async void executeParallelASync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;

            executeParallelASync.IsEnabled = false;
            executeParallelASync.Content = "Running...";

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = await DemoMethods.RunDownloadParallelAsyncV2(progress);
            PrintResults(results);

            watch.Stop();
            var elapsedMS = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMS}";

            executeParallelASync.IsEnabled = true;
            executeParallelASync.Content = "Parallel Async Execute";
        }

       private void PrintResults(List<WebsiteDataModel> results)
        {
            resultsWindow.Text = "";

            foreach(var item in results)
            {
                resultsWindow.Text += $"{item.WebsiteURL} downloaded: {item.WebsiteData.Length} characters long. {Environment.NewLine}";
            }
        }

        private void cancelOperation_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
    }
}
