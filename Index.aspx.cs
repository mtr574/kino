using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using TctrlLogger;

public partial class Index : System.Web.UI.Page
{
    protected static int title_ID;
    protected static string title, endtime;
    protected static TimeSpan runtime, creditstime, actualtime, doremi_elapsed;
    protected static int theatreID;
    protected SqlCommand sqlCmd;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Sync timer interval init
        SyncTimer.Interval = GlobalVar.SyncTimerInterval;
        if (SyncTimer.Enabled.Equals(true)) NextSyncLabel.Text = "פעיל, כל " + TimeSpan.FromMilliseconds(GlobalVar.SyncTimerInterval).ToString("hh") + " שעות";
        else NextSyncLabel.Text = "כבוי";

        if (IsPostBack) theatreID = int.Parse(theatreSelector.SelectedIndex.ToString()) + 1;

        // Init theatreID selector dropdown list
        if (IsPostBack)
        {
            theatreSelector.Items.Clear();
            for (int i = 0; i < GlobalVar.ActiveTheatre.Count; i++)
            {
                theatreSelector.Items.Insert(i, GlobalVar.ActiveTheatre[i]);
            }
        }

        // Set page title
        Page.Title = GlobalVar.AppTitle;
    }

    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Change selected
        GridView1.SelectedRow.BackColor = Color.Azure;

        // Set movie details from movies table
        title_ID = int.Parse(GridView1.SelectedRow.Cells[1].Text);
        title = GridView1.SelectedRow.Cells[3].Text;

        SetMovieDetails();
        mp1.Show();
    }

    // Init add to live details
    protected void SetMovieDetails()
    {
        // Movie title in AddToLive panel
        addToLiveMovieTitle.Text = title;

        // Get poster url from table;
        string poster = GetSQLQuery(title_ID, "Poster");
        ChoosenTitlePoster.ImageUrl = poster;

        runtime = TimeSpan.Parse(GetSQLQuery(title_ID, "Runtime"));
        creditstime = TimeSpan.Parse(GetSQLQuery(title_ID, "Credits"));

        // ActualTime = Runtime - CreditsTime ( The time untill lights are ON )
        actualtime = runtime.Subtract(creditstime);

        // Set endtime to be NowTime + ActualTime -- Turned OFF
        //endtime = TimeSpan.Parse(DateTime.Now.Add(actualtime).ToString("HH\\:mm\\:ss"));

    }

    // Doremi elapsed time text box ( endtime = elasped time + current time)
    protected void DoremiElapsedTextBox_Changed(object sender, EventArgs e)
    {
        TimeSpan time = DateTime.Now.TimeOfDay; // Current time

        if (!(String.IsNullOrEmpty(DoremiElapsedTime.Text)))
        {
            DateTime endtime_x = DateTime.Parse(DoremiElapsedTime.Text); // Doremi elapsed time
            DoremiElapsedTime.Text = ""; // Reset elasped time text box

            endtime_x = endtime_x.Add(time);

            endtime = endtime_x.ToString("t");
        }
    }

    // Adding movie to live database
    protected void InsertMovieToLiveTable(object sender, EventArgs e)
    {
        try
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ValidationConnectionString"].ConnectionString);
            conn.Open();
            sqlCmd = new SqlCommand("UPDATE movies SET [IsRunning] = '1', [TheatreID] = '" + theatreID + "', [EndTime] = '" + endtime + "'  WHERE [ID] = '" + title_ID + "'", conn);
            sqlCmd.ExecuteNonQuery();

            // Removes theatre id after assigning movie
            GlobalVar.ActiveTheatre[theatreID - 1].Enabled = false;

            conn.Close();
        }
        catch (SqlException sqle)
        {
            Logger l = new Logger(Server.MapPath("/log/"));
            l.w("[Index : Sql Add to live] " + sqle.Message.ToString(), "Error");
        }
        finally
        {
            GridView2.DataBind();
        }
    }

    #region controll buttons
    /*
     * Index.aspx controll buttons
     *
     */
    protected void RefreshTitleList(object sender, EventArgs e)
    {
        GridView1.DataBind();
    }
    protected void RefreshLiveList(object sender, EventArgs e)
    {
        GridView2.DataBind();
    }
    protected void AddNewMovie(object sender, EventArgs e)
    {
        Response.Redirect("AddMovie.aspx");
    }
    protected void CleanTable(object sender, EventArgs e)
    {
        RunSQLQuery("DELETE FROM movies");
        GridView1.DataBind();
    }
    protected void GoToLive(object sender, EventArgs e)
    {
        Response.Redirect("Live.aspx");
    }
    protected void ClearLive(object sender, EventArgs e)
    {
        RunSQLQuery("UPDATE movies SET [IsRunning] = 0, [TheatreID] = 0, [EndTime] = 0");
        GridView2.DataBind();
        RenewTheatreList();
    }
    protected void RenewList(object sender, EventArgs e)
    {
        RenewTheatreList();
    }
    protected void SyncMovies(object sender, EventArgs e)
    {
        RunSQLQuery("DBCC CHECKIDENT('movies', RESEED, 0)");

        // Init Doremi Interface (automatic movies scanner)
        DoremiInterface doremiInterface = new DoremiInterface();
        GridView1.DataBind();

        // Last sync (movie list)
        LastSyncLabel.Text = GlobalVar.LastSync;
        LastSyncLabel.DataBind();

        SyncErrorLabel.Text = GlobalVar.MovieSyncError;
        SyncErrorLabel.DataBind();

        FailedSyncCounter.Text = GlobalVar.CounterMovieSyncError.ToString();
        FailedSyncCounter.DataBind();
    }

    /*
     * End controllers
     *
     */
    #endregion

    protected void RenewTheatreList()
    {
        theatreSelector.Items.Clear();
        for (int i = 0; i < GlobalVar.ActiveTheatre.Count; i++)
        {
            GlobalVar.ActiveTheatre[i].Enabled = true;
            theatreSelector.Items.Insert(i, GlobalVar.ActiveTheatre[i]);
        }
    }
    protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        try { GridView1.SelectedRow.BackColor = Color.Transparent; }
        catch (Exception) { }
    }
    protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
    {
        string live_theatre_id = GridView2.SelectedRow.Cells[1].Text;
        RunSQLQuery("UPDATE movies SET [IsRunning] = 0, [TheatreID] = 0, [EndTime] = 0 WHERE [TheatreID] = '" + live_theatre_id + "'");
        GridView2.DataBind();
        GlobalVar.ActiveTheatre[int.Parse(live_theatre_id) - 1].Enabled = true;
    }
    // Access to database function
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
        catch (Exception) { }
    }

    // Retrieve data from sql db
    private string GetSQLQuery(int id, string value)
    {
        string val = "N/A";
        try
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ValidationConnectionString"].ConnectionString);
            conn.Open();

            string query = "SELECT [" + value + "] FROM movies WHERE [ID] = '" + id + "'";
            sqlCmd = new SqlCommand(query, conn);

            SqlDataReader reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {
                val = reader[value].ToString();
            }
            conn.Close();
        }
        catch (Exception sqlE)
        {
            Logger l = new Logger(Server.MapPath("/log/"));
            l.w("[Index : Sql Query] " + sqlE.Message.ToString(), "Error");
        }

        return val;
    }
}