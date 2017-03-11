/* 
 * Created with <3 by Netanel ben-yoram , netanelbeny@gmail.com 2015
 * 
 * uses OMDB Api with poster license
 * http://www.omdbapi.com/
 * 
 * Hover.css v2
 * http://ianlunn.github.io/Hover/
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

public static class GlobalVar
{
    public static string TheatreName = "רב-חן שבעת הכוכבים";
    public static string AppTitle = "Theatre Controller";
    public static string LastSync = "לא ידוע";
    public static int SyncTimerInterval = 43200000; // ms = 12 hours
    public static string MovieSyncError = "";
    public static int CounterMovieSyncError;

    // TheatreID Dropdown list manager
    public static ListItemCollection ActiveTheatre = new ListItemCollection()
    {
        new ListItem("אולם 1", "1", true), 
        new ListItem("אולם 2", "2", true),
        new ListItem("אולם 3", "3", true),
        new ListItem("אולם 4", "4", true), 
        new ListItem("אולם 5", "5", true), 
        new ListItem("אולם 6", "6", true)
    };
}