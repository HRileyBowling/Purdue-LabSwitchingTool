using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;

namespace Lab_Switching_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LabDetect();
        }

        // Global constants
        public readonly string[] AvailabilityRef = new string[4] { "ERROR", "CLASS IN SESSION", "    AVAILABLE   ", "     CLOSED     " };
        public readonly string URLFront = "https://lslab.ics.purdue.edu/icsWeb/LabSchedules?week=", URLBack = "&labselect=";
        public readonly string switchTo = "\t\tSWITCH TO\t";
        public readonly int PadAmount = 53;
        public readonly char PadChar = '=';
        public readonly char headerChar = '*';
        public readonly Lab BlankLab = new Lab("", "", "", new int[7, 48]);
        
        // Glabal variables
        bool firstOpen = true;
        bool ItapMode = true;
        string lastSelected = "Unl!k3lyString☺";

        // Display functions
        private void cboLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRefresh.Focus();
            dtpSelectedDate.Update();
            lblLastUpdated.Visible = false; lblLastUpdated.Update();
            firstOpen = true;
            if ((string)cboLab.SelectedItem == "[Show ALL Labs]") SwitchToALL();
            if ((string)cboLab.SelectedItem == "[Show ITaP Labs]") SwitchToITAP();
            lastSelected = (string)cboLab.SelectedItem;
            if (cboLab.SelectedIndex == 0)
            {
                txtOutput.Clear();
                txtNow.Text = "No lab selected";
                lblLastUpdated.Visible = false;
                return;
            }
            txtOutput.Text = "Loading..."; txtOutput.Update(); txtOutput.Clear();
            txtNow.Text = "Loading..."; txtNow.Update(); txtNow.Clear();
            string[] Rooms = buildRooms();
            Display((string)cboLab.SelectedItem, Rooms);         
            string header = (string)cboLab.SelectedItem + " " + GetDay(dtpSelectedDate.Value);
            int pad = (PadAmount - header.Length) / 2 ;
            txtOutput.Text = " ".PadLeft(pad, headerChar) + header + " ".PadRight(pad, headerChar) + "\r\n" + txtOutput.Text;
            StandardizeTxtBox();

            //Last Updated
            lblLastUpdated.Visible = true;
            string preciseNow = GetNow();
            string MinutesS = DateTime.Now.Minute.ToString();
            if (MinutesS.Length == 1) MinutesS = "0" + MinutesS;
            preciseNow = preciseNow.Substring(0, preciseNow.IndexOf(":") + 1) + MinutesS + GetNow().Substring(GetNow().IndexOf("M") - 1, 2);
            lblLastUpdated.Text = "Last Updated     " + preciseNow;

            //Not Now
            if (dtpSelectedDate.Value.Day != DateTime.Now.Day || dtpSelectedDate.Value.Month != DateTime.Now.Month || dtpSelectedDate.Value.Year != DateTime.Now.Year)
            {
                txtNow.Clear();
                lblLastUpdated.Visible = false;
            }
            this.ActiveControl = txtOutput;
            txtOutput.SelectionStart = txtOutput.Text.Length;
        }

        public void Display(string building, string[] Rooms)
        {
            int number = Rooms.Length - 1;
            Lab[] Labs = new Lab[number + 1];
            string week = GetWeek(dtpSelectedDate.Value);
            for (int labNum = 0; labNum <= number; labNum++)
            {
                Labs[labNum] = new Lab(BlankLab);
                Labs[labNum].setURL(URLFront + week + URLBack + building + " " + Rooms[labNum]);
                Labs[labNum].setRefinedHTML(RefineHTML(Labs[labNum].getRawHTML()));
                Labs[labNum].setSchedule(BuildArray(Labs[labNum].getRefinedHTML()));
            }
            string[] TimeRef = TimeRefernce();
            int DayOfWeek = (int)dtpSelectedDate.Value.DayOfWeek;
            for (int hour = 0; hour < 288; hour++)
            {
                if (hour == 0)
                {
                    bool openAtStart = false;
                    for (int labnum = 0; labnum <= number; labnum++)
                        if (Labs[labnum].GetSchedule()[DayOfWeek, hour] != 3)
                            openAtStart = true;
                    if (openAtStart == true)
                    {
                        txtOutput.Text += " ".PadLeft((PadAmount - 6) / 2, PadChar) + "7:00AM" + " ".PadRight((PadAmount - 6) / 2, PadChar) + "\r\n";
                        for (int labnum = 0; labnum <= number; labnum++)
                            txtOutput.Text += Rooms[labnum] + switchTo + AvailabilityRef[Labs[labnum].GetSchedule()[DayOfWeek, hour]] + "\r\n";
                        firstOpen = false;
                    }
                }
                else
                {
                    bool beenChange = false;
                    for (int labnum = 0; labnum <= number; labnum++)
                        if (Labs[labnum].GetSchedule()[DayOfWeek, hour] != Labs[labnum].GetSchedule()[DayOfWeek, hour - 1])
                            beenChange = true;
                    if (beenChange == true)
                    {
                        txtOutput.Text += " ".PadLeft((PadAmount - TimeRef[hour].Length) / 2, PadChar) + TimeRef[hour] + " ".PadRight((PadAmount - TimeRef[hour].Length) / 2, PadChar) + "\r\n";
                        for (int labnum = 0; labnum <= number; labnum++)
                            if (Labs[labnum].GetSchedule()[DayOfWeek, hour] != Labs[labnum].GetSchedule()[DayOfWeek, hour - 1] || firstOpen == true)
                                txtOutput.Text += Rooms[labnum] + switchTo + AvailabilityRef[Labs[labnum].GetSchedule()[DayOfWeek, hour]] + "\r\n";
                        firstOpen = false;
                    }
                }
            }
            int nowIndex = Array.IndexOf(TimeRef, GetNow());
            txtNow.Clear();
            for (int labnum = 0; labnum <= number; labnum++)
                txtNow.Text += Rooms[labnum] + "\t" + AvailabilityRef[Labs[labnum].GetSchedule()[DayOfWeek, nowIndex]].Trim() + "\r\n";
        }

        public void StandardizeTxtBox()
        {
            string newText = "";
            string temp = "";
            while (txtOutput.Text.Length > 10)
            {
                temp = txtOutput.Text.Substring(0, txtOutput.Text.IndexOf("\r\n"));
                if (temp.Length > PadAmount - 1) temp = temp.Substring(0, PadAmount - 1);
                newText += temp + "\r\n";
                txtOutput.Text = txtOutput.Text.Substring(temp.Length + 2);
            }
            txtOutput.Text = newText.Substring(0, newText.Length - 2);
        }

        private string[] buildRooms()
        {
            string[] Rooms = new string[] { "" };
            if ((string)cboLab.SelectedItem == "AACC") Rooms = new string[] { "101", "204" };
            else if ((string)cboLab.SelectedItem == "BCC") Rooms = new string[] { "204" };
            else if ((string)cboLab.SelectedItem == "BRES") Rooms = new string[] { "201" };
            else if ((string)cboLab.SelectedItem == "BRNG") Rooms = new string[] { "B274", "B275", "B280", "B282", "B286", "B291" };
            else if ((string)cboLab.SelectedItem == "HAMP" && ItapMode == false) Rooms = new string[] { "2215", "3144" };
            else if ((string)cboLab.SelectedItem == "HAMP" && ItapMode == true) Rooms = new string[] { "3144" };
            else if ((string)cboLab.SelectedItem == "HEAV") Rooms = new string[] { "227" };
            else if ((string)cboLab.SelectedItem == "HIKS") Rooms = new string[] { "G950", "G980C", "G980D" };
            else if ((string)cboLab.SelectedItem == "KRAN") Rooms = new string[] { "204" };
            else if ((string)cboLab.SelectedItem == "LCC") Rooms = new string[] { "201" };
            else if ((string)cboLab.SelectedItem == "LILY") Rooms = new string[] { "2400", "G428" };
            else if ((string)cboLab.SelectedItem == "LCC") Rooms = new string[] { "1133D" };
            else if ((string)cboLab.SelectedItem == "MATH" && ItapMode == false) Rooms = new string[] { "311", "B010" };
            else if ((string)cboLab.SelectedItem == "MATH" && ItapMode == true) Rooms = new string[] { "B010" };
            else if ((string)cboLab.SelectedItem == "MCUT") Rooms = new string[] { "C216" };
            else if ((string)cboLab.SelectedItem == "MRDH") Rooms = new string[] { "146S" };
            else if ((string)cboLab.SelectedItem == "MTHW") Rooms = new string[] { "116", "301" };
            else if ((string)cboLab.SelectedItem == "NACC") Rooms = new string[] { "101", "204" };
            else if ((string)cboLab.SelectedItem == "NLSN") Rooms = new string[] { "1225" };
            else if ((string)cboLab.SelectedItem == "PHYS" && ItapMode == false) Rooms = new string[] { "022", "026", "116", "117", "290" };
            else if ((string)cboLab.SelectedItem == "PHYS" && ItapMode == true) Rooms = new string[] { "022", "026", "116", "117" };
            else if ((string)cboLab.SelectedItem == "POTR") Rooms = new string[] { "160", "B52", "B53", "B54", "B55" };
            else if ((string)cboLab.SelectedItem == "RHPH" && ItapMode == false) Rooms = new string[] { "272", "316" };
            else if ((string)cboLab.SelectedItem == "RHPH" && ItapMode == true) Rooms = new string[] { "316" };
            else if ((string)cboLab.SelectedItem == "SC") Rooms = new string[] { "179", "183", "189", "231", "246", "277", "283", "289", "G046", "G073" };
            else if ((string)cboLab.SelectedItem == "STEW" && ItapMode == false) Rooms = new string[] { "102", "135", "142", "153" };
            else if ((string)cboLab.SelectedItem == "STEW" && ItapMode == true) Rooms = new string[] { "102" };
            else if ((string)cboLab.SelectedItem == "TERM") Rooms = new string[] { "163" };
            else if ((string)cboLab.SelectedItem == "WTHR" && ItapMode == false) Rooms = new string[] { "114", "212", "214", "305", "307" };
            else if ((string)cboLab.SelectedItem == "WTHR" && ItapMode == true) Rooms = new string[] { "114", "212", "214" };
            return Rooms;
        }

        // Data manipulation functions
        public string RefineHTML(string RawHTML)
        {
            int cutIndex = RawHTML.IndexOf("Saturday");
            string RefinedHTML = "";
            RawHTML = RawHTML.Substring(cutIndex);
            cutIndex = RawHTML.IndexOf("<th");
            RawHTML = RawHTML.Substring(cutIndex);
            RawHTML = RawHTML.Substring(0, RawHTML.IndexOf("</table>"));
            string currentline = "";
            RawHTML = RawHTML.Replace("</tr><tr>", "*");
            RawHTML = RawHTML.Replace("</tr>", "*@");
            RawHTML = RawHTML.Replace("\t", "");
            while (RawHTML.IndexOf("**@") > 30)
            {
                RawHTML = RawHTML.Substring(RawHTML.IndexOf("<t"));
                currentline = RawHTML.Substring(0, RawHTML.IndexOf("</t") + 5);
                if (currentline.Contains("scope=\"row\"") == false)
                    RefinedHTML += currentline + "\r\n";
                RawHTML = RawHTML.Substring(currentline.Length);
            }
            RefinedHTML = RefinedHTML.Replace("*", "");
            RawHTML = "";
            return RefinedHTML;
        }

        public int[,] BuildArray(string RefinedHTML)
        {
            int span;
            int spanlength;
            int available; //0- not decided 1- class 2-open 3-closed
            string currentline;
            int currentDay = 0;
            int currentHour = 0;
            int[,] Schedule = new int[7, 288];
            while (RefinedHTML.IndexOf("<t") != -1)
            {
                currentline = RefinedHTML.Substring(RefinedHTML.IndexOf("<t"), RefinedHTML.IndexOf("</t") + 5);
                spanlength = currentline.IndexOf("colspan=") - currentline.IndexOf("rowspan=") - 9;
                span = int.Parse(currentline.Substring(currentline.IndexOf("rowspan=") + 8, spanlength));

                if (currentline.IndexOf("<th") == 0) available = 3;
                else if (currentline.Substring(currentline.IndexOf("<b>") + 3, 9) == "Available") available = 2;
                else available = 1;
                if (currentline.IndexOf("<th") == 0 && span < 6) available = 2;
                if ((currentDay >= 2 && currentHour >= 60) || currentDay >= 3)
                { }

                while (Schedule[currentDay, currentHour] != 0)
                {
                    currentDay += 1;
                    if (currentDay == 7)
                    {
                        currentHour += 1;
                        currentDay = 0;
                    }
                }

                while (span != 0)
                {
                    Schedule[currentDay, currentHour + span - 1] = available;
                    span += -1;
                }
                RefinedHTML = RefinedHTML.Substring(2);
                if (RefinedHTML.IndexOf("<t") != -1)
                    RefinedHTML = RefinedHTML.Substring(RefinedHTML.IndexOf("<t"));
            }
            RefinedHTML = "";
            return Schedule;
        }

        // Date Time functions
        public string GetWeek(DateTime Selected)
        {
            DateTime SelectedSunday = Selected.AddDays(-(int)Selected.DayOfWeek);
            string year = SelectedSunday.Year.ToString();
            int monthI = SelectedSunday.Month; monthI += -1; string month = monthI.ToString();
            string day = SelectedSunday.Day.ToString();
            if (month.Length == 1) month = "0" + month;
            if (day.Length == 1) day = "0" + day;
            string week = year + month + day;
            return week;
        }

        public string GetDay(DateTime Selected)
        {
            int DayI = (int)Selected.DayOfWeek;
            string Day = "ERROR";
            if (DayI == 0) Day = "Sunday";
            else if (DayI == 1) Day = "Monday";
            else if (DayI == 2) Day = "Tuesday";
            else if (DayI == 3) Day = "Wednesday";
            else if (DayI == 4) Day = "Thursday";
            else if (DayI == 5) Day = "Friday";
            else if (DayI == 6) Day = "Saturday";

            int MonthI = (int)Selected.Month;
            string Month = "ERROR";
            if (MonthI == 1) Month = "January";
            if (MonthI == 2) Month = "February";
            if (MonthI == 3) Month = "March";
            if (MonthI == 4) Month = "April";
            if (MonthI == 5) Month = "May";
            if (MonthI == 6) Month = "June";
            if (MonthI == 7) Month = "July";
            if (MonthI == 8) Month = "August";
            if (MonthI == 9) Month = "September";
            if (MonthI == 10) Month = "October";
            if (MonthI == 11) Month = "November";
            if (MonthI == 12) Month = "December";

            return Day + " " + Month + " " + Selected.Day.ToString() + ", " + Selected.Year.ToString();
        }

        public string GetNow()
        {
            DateTime RightNow = DateTime.Now;
            string AMPM = "AM";
            int minute = RightNow.Minute;
            minute = (minute / 5) * 5;
            int Hour = RightNow.Hour;
            if (Hour >= 12) AMPM = "PM";
            if (Hour >= 13) Hour = Hour - 12;
            string MinutesS = minute.ToString();
            if (MinutesS.Length == 1) MinutesS = "0" + MinutesS;
            string Now = Hour.ToString() + ":" + MinutesS + AMPM;
            if (Now == "12:00AM") Now = "Midnight";
            if (Now == "12:00PM") Now = "Noon";
            return Now;
        }
        
        public string[] TimeRefernce()
        {
            string[] TimeRef = new string[288];
            string input = "7:00AM", AMPM = "AM";
            for (int count = 0; count < 288; count++)
            {
                if (count == 72)
                    input = "1:00PM";
                if (count == 216)
                    input = "1:00AM";

                TimeRef[count] = input;
                if (count == 60) { AMPM = "PM"; TimeRef[count] = "Noon"; }
                if (count == 204) { AMPM = "AM"; TimeRef[count] = "Midnight"; }

                if (input.Substring(input.IndexOf(":") + 2, 1) == "0")
                    input = input.Substring(0, input.IndexOf(":") + 2) + "5" + AMPM;

                else
                    input = input.Substring(0, input.IndexOf(":") + 1) + (int.Parse(input.Substring(input.IndexOf(":") + 1, 1)) + 1).ToString() + "0" + AMPM;

                if (input.Substring(input.IndexOf(":") + 1, 1) == "6")
                    input = (int.Parse(input.Substring(0, input.IndexOf(":"))) + 1).ToString() + ":00" + AMPM;
            }
            return TimeRef;
        }

        // Drop Box switch methods
        public void SwitchToITAP()
        {
            cboLab.Items.Clear();
            cboLab.Items.Add("Select a lab");
            cboLab.Items.Add("BCC");
            cboLab.Items.Add("BRES");
            cboLab.Items.Add("BRNG");
            cboLab.Items.Add("HAMP");
            cboLab.Items.Add("HIKS");
            cboLab.Items.Add("MATH");
            cboLab.Items.Add("MCUT");
            cboLab.Items.Add("MRDH");
            cboLab.Items.Add("MTHW");
            cboLab.Items.Add("PHYS");
            cboLab.Items.Add("RHPH");
            cboLab.Items.Add("SC");
            cboLab.Items.Add("STEW");
            cboLab.Items.Add("WTHR");
            cboLab.Items.Add("[Show ALL Labs]");
            if (cboLab.Items.Contains(lastSelected))
                cboLab.SelectedItem = lastSelected;
            else cboLab.SelectedIndex = 0;
            ItapMode = true;
        }
        public void SwitchToALL()
        {
            cboLab.Items.Clear();
            cboLab.Items.Add("Select a lab");
            cboLab.Items.Add("AACC");
            cboLab.Items.Add("BCC");
            cboLab.Items.Add("BRES");
            cboLab.Items.Add("BRNG");
            cboLab.Items.Add("HAMP");
            cboLab.Items.Add("HEAV");
            cboLab.Items.Add("HIKS");
            cboLab.Items.Add("KRAN");
            cboLab.Items.Add("LCC");
            cboLab.Items.Add("LILY");
            cboLab.Items.Add("LYNN");
            cboLab.Items.Add("MATH");
            cboLab.Items.Add("MCUT");
            cboLab.Items.Add("MRDH");
            cboLab.Items.Add("MTHW");
            cboLab.Items.Add("NACC");
            cboLab.Items.Add("NLSN");
            cboLab.Items.Add("PHYS");
            cboLab.Items.Add("POTR");
            cboLab.Items.Add("RPHM");
            cboLab.Items.Add("SC");
            cboLab.Items.Add("STEW");
            cboLab.Items.Add("TERM");
            cboLab.Items.Add("WTHR");
            cboLab.Items.Add("[Show ITaP Labs]");
            cboLab.SelectedItem = lastSelected;
            ItapMode = false;
        }

        // Lab Auto detect funtion
        public void LabDetect()
        {
            SwitchToALL();
            string labname = "";
            string MachineName = Environment.MachineName;
            string currentLab;
            bool found = false;
            for (int count = 1; count <= cboLab.Items.Count - 2 && found == false; count++)
            {
                currentLab = cboLab.Items[count].ToString();
                if (MachineName.IndexOf(currentLab) != -1)
                { labname = currentLab; found = true; }
            }
            if (found)
            {
                SwitchToITAP();
                if (cboLab.Items.Contains(labname) == false)
                    SwitchToALL();
                cboLab.SelectedItem = labname;
            }
            else cboLab.SelectedIndex = 0;
        }

        // Print variables
        private Font printFont;
        private StreamReader streamToPrint;

        // Print function and event handler
        private void btnPrint_Click(Object sender, EventArgs e)
        {
            if (txtOutput.Text != "")
            {
                File.WriteAllText(Application.StartupPath + "LastPrinted.txt", txtOutput.Text);
                try
                {
                    streamToPrint = new StreamReader
                       (Application.StartupPath + "LastPrinted.txt");
                    try
                    {
                        printFont = new Font("Courier New", 12);
                        PrintDocument pd = new PrintDocument();
                        pd.DocumentName = (string)cboLab.SelectedItem + " " + GetDay(dtpSelectedDate.Value);
                        pd.PrintPage += new PrintPageEventHandler
                           (this.pd_PrintPage);

                        try
                        {
                            PrintDialog PrintD = new PrintDialog();
                            if (PrintD.ShowDialog() == DialogResult.OK)
                            {
                                pd.Print();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Exception Occured While Printing", ex);
                        }
                    }
                    finally
                    {
                        streamToPrint.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else MessageBox.Show("Detected blank document. Print Aborted");
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            while (count < linesPerPage && ((line = streamToPrint.ReadLine()) != null))
            {
                if (line != "")
                {
                    yPos = topMargin + (count * printFont.GetHeight(ev.Graphics));
                    StringFormat format = new StringFormat(StringFormatFlags.LineLimit);
                    float[] formatTabs = { 100.0f, 123.0f };
                    format.SetTabStops(0.0f, formatTabs);
                    ev.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, format);
                    count++;
                }
            }

            // If more lines exist, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }
    }
}
