using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Lab_Switching_Tool
{
    public class Lab
    {
        public Lab(string newURL, string newrawHTML, string newRefinedHTML, int[,] newSchedule)
        {
            this.URL = newURL;
            this.rawHTML = newrawHTML;
            this.refinedHTML = newRefinedHTML;
            this.Schedule = newSchedule;
        }

        public Lab(Lab lab)
        {
            this.URL = lab.URL;
            this.rawHTML = lab.rawHTML;
            this.refinedHTML = lab.refinedHTML;
            this.Schedule = lab.Schedule;
        }

        private string URL;
        private string rawHTML;
        private string refinedHTML;
        private int[,] Schedule;

        public void setURL (string newURL) // Sets URL and rawHTML
        {
            this.URL = newURL;
            using (WebClient client = new WebClient())
            {
                this.rawHTML = client.DownloadString(this.URL);
            }
        }
        
        public string getRawHTML()
        {
            return this.rawHTML;
        }

        public void setRefinedHTML(string newRefinedHTML)
        {
            this.refinedHTML = newRefinedHTML;
        }

        public string getRefinedHTML()
        {
            return this.refinedHTML;
        }

        public void setSchedule(int[,] newSchedule)
        {
            this.Schedule = newSchedule;
        }

        public int[,] GetSchedule()
        {
            return this.Schedule;
        }
    }
}
