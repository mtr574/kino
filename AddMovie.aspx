<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddMovie.aspx.cs" Inherits="Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" rel="stylesheet" href="css\style.css" />
</head>

<body>
    <form id="form1" runat="server">
        <div class="small_container heading-fancy heb">
            <h2 class="heb center">הוספת מופע חדש</h2>
            <hr />
            כותרת
            <div>
                <asp:TextBox runat="server" ID="MovieTitle" type="text" />
            </div>

            אורך (hh:mm:ss)
            <div>
                <asp:TextBox runat="server" ID="MovieRuntime" type="text" />
            </div>

            זמן כתוביות
            <div>
                <asp:TextBox runat="server" ID="CreditsBox" type="text" />
            </div>

            שנה
            <div>
                <asp:TextBox runat="server" ID="MovieYear" type="text" />
            </div>

            <fieldset class="fieldset">
                <legend>פוסטר</legend>
                <asp:CheckBox runat="server" ID="ManualPoster" OnCheckedChanged="SetManualUpload" Enabled="true" AutoPostBack="true" Text="הוספת פוסטר ידני" />
                <div>
                    <asp:FileUpload runat="server" ID="UploadPosterButton" Enabled="False" Width="270px" />
                </div>
            </fieldset>

            <fieldset class="fieldset">
                <legend>פורמט</legend>
                <asp:RadioButtonList ID="MovieFormat" runat="server">
                    <asp:ListItem Text="רגיל" id="reg" Value="רגיל" />
                    <asp:ListItem Text="תלת-מימד" id="threed" Value="תלת-מימד" />
                    <asp:ListItem Text="אירוע" id="event" Value="אירוע" />
                </asp:RadioButtonList>
            </fieldset>

            <hr />
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="הוסף" CssClass="ok" />
            <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="חזור" Style="float: left;" CssClass="cancel" />

        </div>
    </form>
</body>
</html>
