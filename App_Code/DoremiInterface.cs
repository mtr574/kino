using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using TctrlLogger;

// Version: 1.2; 10-Apr-2015
// Get title, format, runtime, credits time from Doremi Cinema (xml files);

public class DoremiInterface : System.Web.UI.Page
{
    static string title, format, runtime, credits, year;
    static SqlConnection conn;
	public DoremiInterface()
	{
        // Reset movies sync error vars;
        GlobalVar.CounterMovieSyncError = 0;
        GlobalVar.MovieSyncError = "";

        // Clear posters folder
        DeletePosters();

        // DB connection
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ValidationConnectionString"].ConnectionString);

        // Set LastSync Date and time
        GlobalVar.LastSync = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        // Clear table before adding new movie list ; 
        RunSQLQuery("DELETE FROM movies");

        // Gets all xml files within the folder;
        string xmlPath = Server.MapPath("/xml/");   
        string[] xmlFiles = Directory.GetFiles(xmlPath, "*.xml");

        for (int i = 0; i < xmlFiles.Length; i++)
        {
            Logger l = new Logger(Server.MapPath("/log/"));
            l.w("Playlist Synced: " + xmlFiles[i].Substring(23));
            
            try
            {
                XmlTextReader reader = new XmlTextReader(xmlFiles[i]);
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "ShowPlaylist": // Movie Title (includes format type)
                                {
                                    // Playlist ShowTitleText format: "movie_name format" ex: "Sponge_Bob 2D 2014"
                                    reader.ReadToFollowing("ShowTitleText");
                                    string[] t = reader.ReadElementContentAsString().Split(' ');
                                    title = t[0];
                                    title = title.Replace('_', ' ');
                                    format = t[1];

                                    // Option to get year from playlist title, TURNED OFF
                                    // year = t[2];
                                    break;
                                }

                            case "Composition": // Movie runtime
                                {
                                    reader.ReadToFollowing("AnnotationText");
                                    if (reader.ReadElementContentAsString().Contains(title))
                                    {
                                        reader.ReadToFollowing("IntrinsicDuration");
                                        double time = reader.ReadElementContentAsDouble();
                                        
                                        // Convert offset to seconds;
                                        time /= 24;
                                        // Covert to minutes;
                                        time /= 60;
                                        runtime = (Math.Truncate(time * 100) / 100).ToString();
                                        runtime = TimeSpan.FromMinutes(Double.Parse(runtime)).ToString("hh\\:mm\\:ss");
                                        break;
                                    }
                                    break;
                                }

                            case "AutomationCue": // Movie Credits time
                                {
                                    reader.ReadToFollowing("Action");
                                    if (reader.ReadElementContentAsString().Equals("Back Lights On"))
                                    {
                                        reader.ReadToFollowing("Offset");
                                        double time = reader.ReadElementContentAsDouble();
                                        time /= 24;
                                        // Covert to minutes;
                                        time /= 60;
                                        credits = (Math.Truncate(time * 100) / 100).ToString();
                                        credits = TimeSpan.FromMinutes(Double.Parse(credits)).ToString("hh\\:mm\\:ss");
                                        break;
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception) { }

            // Add to movies table;
            AddToMovies(title, format, runtime, credits);
        }
    }

    private void AddToMovies(string t, string f, string r, string c)
	{
        try
        {
            conn.Open();
            string addTitle = "INSERT INTO movies (Title, Runtime, Format, Poster, Credits) values (@title, @runtime, @format, @poster, @credits);";
            SqlCommand sqlCmd = new SqlCommand(addTitle, conn);

            sqlCmd.Parameters.AddWithValue("@runtime", r);
            if (f.Equals("3D"))
            {
                sqlCmd.Parameters.AddWithValue("@format", "תלת-מימד");
                sqlCmd.Parameters.AddWithValue("@title", t + " 3D");
            }
            else
            {
                sqlCmd.Parameters.AddWithValue("@format", "רגיל");
                sqlCmd.Parameters.AddWithValue("@title", t);
            }
            sqlCmd.Parameters.AddWithValue("@poster", GetPoster());
            sqlCmd.Parameters.AddWithValue("@credits", credits);
            
            sqlCmd.ExecuteNonQuery();
        }
        catch (Exception) {
            // Notify the title which didnt added to the table;
            GlobalVar.MovieSyncError += title + ", ";
            GlobalVar.CounterMovieSyncError++;
        }
        finally { conn.Close(); }
	}
    private string GetPoster()
    {
        string movieId = getMovieID();
        string poster = "http://img.omdbapi.com/?i=" + movieId + "&apikey=a6d6bc5";
        string posterPath = Server.MapPath("/assets/poster/") + movieId + ".png";
        
        // Save poster image locally
        WebClient client = new WebClient();
        client.DownloadFile(poster, posterPath);

        return posterPath.Substring(Server.MapPath("~").Length);
    }
    private string getMovieID()
    {
        // Gets the current year;
        year = DateTime.Now.Year.ToString();

        // Gets movie id from OMDB API
        string xmlUrl = "http://www.omdbapi.com/?t=" + title + "&y=" + year + "&plot=short&r=xml";
        XmlReader reader = XmlReader.Create(xmlUrl);
        string id = null;
        while (reader.Read())
        {
            if (reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case "movie":
                        id = reader["imdbID"];
                        break;
                }
            }
        }
        return id;
    }

    // Access to database function
    private void RunSQLQuery(string sql_query)
    {
        try
        {
            conn.Open();
            SqlCommand sqlCmd = new SqlCommand(sql_query, conn);
            sqlCmd.ExecuteNonQuery();
            conn.Close();
        }
        catch (Exception) { }
    }

    // Delete poster files
    private void DeletePosters()
    {
        string path = Server.MapPath("/assets/poster/");
        string[] posters = Directory.GetFiles(path, "*.png");
        for (int i = 0; i < posters.Length; i++)
        {
            File.Delete(posters[i]);
        }
    }
}