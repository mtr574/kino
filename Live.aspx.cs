using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TctrlLogger;

public partial class Live : System.Web.UI.Page
{
    protected static int[] IsRunning = new int[6];
    protected SqlCommand sqlCmd;
    protected Label ElapsedLabel, EndtimeLabel, FormatLabel, TheatreIDLabel, Box, CredLabel;
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set page title
        Page.Title = GlobalVar.AppTitle;
    }

    #region Timer
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        ListView1.DataBind();

        // Replace format text with interactive logo
        foreach (ListViewItem item in ListView1.Items)
        {
            FormatLabel = (Label)item.FindControl("FormatLabel");
            if (FormatLabel != null)
            {
                switch (FormatLabel.Text)
                {
                    case "רגיל":
                        FormatLabel.Text = "<img src='assets/icons/format/reg_s.png' width='40px'>";
                        break;
                    case "תלת-מימד":
                        FormatLabel.Text = "<img src='assets/icons/format/3d_s.png' width='50px'>";
                        break;
                    case "אירוע":
                        FormatLabel.Text = "<img src='assets/icons/format/event_s.png' width='40px'>";
                        break;
                }
            }
        }

        // Current time and date label
        TheTime.Text = DateTime.Now.ToString("HH:mm:ss" + " | ");
        TheDate.Text = DateTime.Today.ToString("dd-MM-yyyy");

        foreach (ListViewItem item in ListView1.Items)
        {
            // Labels in Live page
            ElapsedLabel = (Label)item.FindControl("ElapsedLabel");
            EndtimeLabel = (Label)item.FindControl("EndtimeLabel");
            TheatreIDLabel = (Label)item.FindControl("TheatreID");
            CredLabel = (Label)item.FindControl("CredLabel");

            // Elapsed time
            TimeSpan credTime = TimeSpan.Parse(CredLabel.Text); // Credits time from table to evaluate lights-on time
            TimeSpan diff = (Convert.ToDateTime(EndtimeLabel.Text).Subtract(credTime) - DateTime.Now);
            
            // Fix 24Hr clock
            TimeSpan twentyfour = new TimeSpan(23, 60, 0);
            if (diff.Hours < 0)
                diff = diff.Add(twentyfour);

            // 10 Minutes remaining alert with red label
            if (diff.Hours == 0 && diff.Minutes < 10)
            {
                ElapsedLabel.BackColor = Color.Red;
            }
            else ElapsedLabel.BackColor = Color.Green;
            string elapsed = diff.ToString("hh\\:mm\\:ss");
            ElapsedLabel.Text = elapsed;

            // When the show is over
            if (diff.Hours <= 0 && diff.Minutes <= 0 && diff.Seconds <= 0)
            {
                ElapsedLabel.Text = "הקרנה הסתיימה";

                // Delete show from database (will not appear in Live.aspx)
                IsRunning[int.Parse(TheatreIDLabel.Text)] = 1;
                ShowIsOver(TheatreIDLabel.Text);

                // Return theatreID to dropdown list
                GlobalVar.ActiveTheatre[int.Parse(TheatreIDLabel.Text) - 1].Enabled = true;

                // Refreshing the live page after movie is over
                Response.Redirect("Live.aspx");
            }
        }
    }
    #endregion

    private void ShowIsOver(string p)
    {
        if (IsRunning[int.Parse(p)] == 1)
        {
            // Remove finished movie from live view
            RunSQLQuery("UPDATE movies SET [IsRunning] = 0, [TheatreID] = 0, [EndTime] = 0 WHERE [TheatreID] = " + p);
            
            // Prevent from sending query in loop
            IsRunning[int.Parse(TheatreIDLabel.Text)] = 0; 
        }
    }

    private void RunSQLQuery(string sql_query)
    {
        try
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ValidationConnectionString"].ConnectionString);
            conn.Open();
            sqlCmd = new SqlCommand(sql_query, conn);
            sqlCmd.ExecuteNonQuery();
            conn.Close();
        }
        catch (Exception sqlE)
        {
            Logger l = new Logger(Server.MapPath("/log/"));
            l.w("[Live : Sql Query] " + sqlE.Message.ToString(), "Error");
        }
    }
}