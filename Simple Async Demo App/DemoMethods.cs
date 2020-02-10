using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple_Async_Demo_App
{
    public class DemoMethods
    {
        public static List<string> PrepData()
        {
            List<string> output = new List<string>
            {
                "https://www.yahoo.com",
                "https://www.google.com",
                "https://www.microsoft.com",
                "https://www.cnn.com",
                "https://www.stackoverflow.com",
                "https://www.facebook.com",
                "https://www.amazon.com",
                "https://www.twitter.com"
            };

            return output;
        }

        //First method run in each application instance will take longer due to application caching proxy settings
        public static List<WebsiteDataModel> RunDownloadSync()
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            foreach (string site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
            }

            return output;
        }

        public static List<WebsiteDataModel> RunDownloadParallelSync()
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            Parallel.ForEach<string>(websites, (site) =>
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
            });

            return output;
        }

        //Rename async voids to Task unless it is an event
        public static async Task<List<WebsiteDataModel>> RunDownloadASync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
                ProgressReportModel report = new ProgressReportModel();

            foreach (string site in websites)
            {
                WebsiteDataModel results = await DownloadWebsiteAsync(site);
                output.Add(results);

                //If cancellation token is activated, an exception is thrown
                cancellationToken.ThrowIfCancellationRequested();

                report.SitesDownloaded = output;
                report.PercentageComplete = (output.Count * 100 )/ websites.Count;
                progress.Report(report);

                //Await states to run this async but await the results to continue
                //The Task.Run is an example shown on when you aren't able to access a method
                //WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site));
            }

            return output;
        }

        //The downside of this method is that while it does run faster, the reporting still appears synchronous to the user. A best of both worlds method will be shown in a different example
        public static async Task <List<WebsiteDataModel>> RunParallelDownloadAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string site in websites)
            {
                //Tasks do not have to wait on the other to download now
                //Also using built in async download assuming we can now edit the method
                tasks.Add(DownloadWebsiteAsync(site));
            }

            //When all awaits a whole list of tasks are complete and casts them back into an object
            var results = await Task.WhenAll(tasks);

            return new List<WebsiteDataModel>(results);
        }

        //Allows us to run async while also reporting as it completes tasks
        public static async Task<List<WebsiteDataModel>> RunDownloadParallelAsyncV2(IProgress<ProgressReportModel> progress)
        {
            List<string> websites = PrepData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel();

            await Task.Run(() =>
            {
                Parallel.ForEach<string>(websites, (site) =>
                {
                    WebsiteDataModel results = DownloadWebsite(site);
                    output.Add(results);

                    report.SitesDownloaded = output;
                    report.PercentageComplete = (output.Count * 100) / websites.Count;
                    progress.Report(report);
                });
            });

            return output;
        }
        public static WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteURL = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);

            return output;
        }

        public static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteURL = websiteURL;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);

            return output;
        }
    }  
}
