using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Async_Demo_App
{
    public class ProgressReportModel
    {
        public int PercentageComplete { get; set; } = 0;
        public List<WebsiteDataModel> SitesDownloaded { get; set; } = new List<WebsiteDataModel>();
    }
}
