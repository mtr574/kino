using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Xml;
using System.IO;
using TctrlLogger;

public partial class Register : System.Web.UI.Page
{
    private XmlReader reader = null;
    private string runtime, year;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set page title
        Page.Title = GlobalVar.AppTitle;

        UploadPosterButton.Enabled = false;
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        string savePath, poster;

        // Add new movie title or event to [movies] table
        try
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ValidationConnectionString"].ConnectionString);
            conn.Open();
            string addTitle = "IF NOT EXISTS (SELECT * FROM movies WHERE title = @title) INSERT INTO movies (Title, Runtime, Format, Poster, Credits) values (@title, @runtime, @format, @poster, @credits)";
            SqlCommand sqlCmd = new SqlCommand(addTitle, conn);
            sqlCmd.Parameters.AddWithValue("@title", MovieTitle.Text);
            sqlCmd.Parameters.AddWithValue("@format", MovieFormat.Text);

            // Event only (uses event poster)
            if (MovieFormat.Text == "אירוע")
            {
                savePath = Server.MapPath("/assets/") + "event.png";
                sqlCmd.Parameters.AddWithValue("@poster", savePath.Substring(Server.MapPath("~").Length));
                sqlCmd.Parameters.AddWithValue("@runtime", MovieRuntime.Text);
                sqlCmd.Parameters.AddWithValue("@credits", "0");
            }
            // Regular or 3D Movie
            else
            {
                sqlCmd.Parameters.AddWithValue("@credits", CreditsBox.Text);
                // Option to add local poster, if impossible to get from api
                if (UploadPosterButton.HasFile)
                {
                    try
                    {
                        string filename = Path.GetFileName(UploadPosterButton.FileName);
                        savePath = Server.MapPath("/assets/poster/") + filename;
                        UploadPosterButton.SaveAs(savePath);
                        sqlCmd.Parameters.AddWithValue("@poster", savePath.Substring(Server.MapPath("~").Length));
                    }
                    catch (Exception x)
                    {
                        Logger l = new Logger(Server.MapPath("/log/"));
                        l.w("[AddMovie : Poster Upload] " + x.Message.ToString(), "Error");
                    }
                }
                // Get poster file from OMDB API
                else
                {
                    poster = "http://img.omdbapi.com/?i=" + getMovieID() + "&apikey=a6d6bc5";
                    savePath = Server.MapPath("/assets/poster/") + getMovieID() + ".png";

                    // Save poster image locally
                    WebClient client = new WebClient();
                    client.DownloadFile(poster, savePath);
                    sqlCmd.Parameters.AddWithValue("@poster", savePath.Substring(Server.MapPath("~").Length));
                }
            }

            if (MovieRuntime.Text.Equals(""))
            {
                //Cut the 'min' from runtime string (OMDB API).
                runtime = runtime.Substring(0, 3);
                runtime = TimeSpan.FromMinutes(Double.Parse(runtime)).ToString("hh\\:mm\\:ss");
                sqlCmd.Parameters.AddWithValue("@runtime", runtime);
                sqlCmd.Parameters.AddWithValue("@credits", "0");
            }

            sqlCmd.ExecuteNonQuery();
            conn.Close();
            Response.Redirect("Index.aspx");
        }
        catch (System.Net.WebException) // 404 error code
        {
            Response.Write("<script>alert('הסרט לא נמצא, הכנס פוסטר באופן ידני');</script>");
        }
        catch (Exception) { }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        Response.Redirect("Index.aspx");
    }

    protected string getMovieID()
    {
        // Gets the current year;
        if (MovieYear.Text == "")
        {
            year = DateTime.Now.Year.ToString();
        }
        else
        {
            year = MovieYear.Text;
        }

        // Gets movie id, runtime from OMDB API
        string xmlUrl = "http://www.omdbapi.com/?t=" + MovieTitle.Text + "&y=" + year + "&plot=short&r=xml";
        reader = XmlReader.Create(xmlUrl);
        string id = null;
        while (reader.Read())
        {
            if (reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case "movie":
                        id = reader["imdbID"];
                        runtime = reader["runtime"];
                        break;
                }
            }
        }
        return id;
    }
    protected void SetManualUpload(object sender, EventArgs e)
    {
        if (UploadPosterButton.Enabled == false) { UploadPosterButton.Enabled = true; }
        else UploadPosterButton.Enabled = false;
        UploadPosterButton.DataBind();
    }
}