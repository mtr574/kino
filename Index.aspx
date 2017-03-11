<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link type="text/css" rel="stylesheet" href="css\style.css" />
    <link type="text/css" rel="stylesheet" href="css\icons.css" />
    <link type="text/css" rel="stylesheet" href="css\hover.min.css" media="all" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="menu">
            <asp:Button ID="Button3" runat="server" OnClick="GoToLive" Text="כרגע מקרינים" CssClass="btn3 live_icon hvr-float" ToolTip="דף סרטים מוקרנים בזמת אמת" />
            <br />
            <br />
            <asp:Button ID="Button1" runat="server" OnClick="AddNewMovie" Text="הוספת מופע" CssClass="btn2 add_icon hvr-float" ToolTip="הוספת מופע חדש" />
            <asp:Button ID="Button7" runat="server" OnClick="SyncMovies" Text="סנכרון סרטים" CssClass="btn2 hvr-pulse-grow sync_icon" ToolTip="סנכרון רשימת סרטים" />
            <br />
            <br />
            <asp:Button ID="Button5" runat="server" OnClick="RenewList" Text="שחזר אולמות" CssClass="btn renew_icon hvr-float" ToolTip="שחזור רשימת אולמות בדף שיוך סרט" />
        </div>

        <div runat="server" class="header">
            <span style="text-align: center; color: white; font-size: 30px;"><%= GlobalVar.TheatreName %> | Theatre Controller </span>
        </div>

        <!-- טבלת מופעים מוקרנים -->
        <div id="content">
            <div class="right_container heading-fancy heb">
                <h2 class="heb center">מופעים מוקרנים</h2>
                <hr />
                <div>
                    <asp:GridView EmptyDataText="אין מופעים מוקרנים" ID="GridView2" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource2" OnSelectedIndexChanged="GridView2_SelectedIndexChanged" AllowPaging="False" AllowSorting="True" OnSelectedIndexChanging="GridView1_SelectedIndexChanging">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="מחק" ItemStyle-CssClass="edit_icon">
                                <ItemStyle Width="90px" Height="40px" HorizontalAlign="Left" />
                            </asp:CommandField>
                            <asp:BoundField DataField="TheatreID" HeaderText="אולם" HeaderStyle-HorizontalAlign="Center" SortExpression="TheaterID">
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Title" HeaderText="שם מופע" HeaderStyle-HorizontalAlign="Center" SortExpression="Title">
                                <ItemStyle Width="200px" HorizontalAlign="Center" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
                <hr />
                <asp:Button ID="Button9" runat="server" OnClick="RefreshLiveList" Text="ריענון" CssClass="btn2_small hvr-float renew_icon_small txt_left" ToolTip="נקה דף רשימת סרטים" />
                <asp:Button ID="Button4" runat="server" OnClick="ClearLive" Text="נקה מופעים" CssClass="btn2 hvr-float clear_icon txt_left" ToolTip="נקה מופעים" />
            </div>
            <!-- טבלת מופעים מוקרנים -סוף-  -->

            <div class="container heading-fancy heb">
                <h2 class="heb center">רשימת מופעים</h2>
                <hr />
                <div>
                    <asp:GridView EmptyDataText="רשימה ריקה, אנא בצע סנכרון סרטים" ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" AllowSorting="True" OnSelectedIndexChanging="GridView1_SelectedIndexChanging">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="הוסף לאולם" ItemStyle-CssClass="">
                                <ItemStyle Width="140px" Height="40px" HorizontalAlign="Center" />
                            </asp:CommandField>
                            <asp:BoundField DataField="ID" SortExpression="ID" HeaderText="ID" ItemStyle-Width="25px" />
                            <asp:TemplateField HeaderText="פוסטר">
                                <ItemTemplate>
                                    <asp:Image ID="poster" runat="server" ImageUrl='<%#Bind("Poster") %>' Width="50px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Title" HeaderText="שם מופע" SortExpression="Title" HeaderStyle-HorizontalAlign="Center">
                                <ItemStyle Width="300px" HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Runtime" HeaderText="אורך" SortExpression="Runtime" HeaderStyle-HorizontalAlign="Center">
                                <ItemStyle Width="150px" HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Format" HeaderText="פורמט" SortExpression="Format" HeaderStyle-HorizontalAlign="Center">
                                <ItemStyle Width="100px" HorizontalAlign="Center" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
                <hr />
                <asp:Button ID="Button8" runat="server" OnClick="RefreshTitleList" Text="ריענון" CssClass="btn2_small hvr-float renew_icon_small txt_left" ToolTip="נקה דף רשימת סרטים" />
                <asp:Button ID="Button2" runat="server" OnClick="CleanTable" Text="נקה רשימה" CssClass="btn2 hvr-float clear_icon txt_left" ToolTip="נקה דף רשימת סרטים" />
            </div>

            <!-- DO-NOT DELETE (REFERENCE TO FUNCTIONAL METHOD ) -->
            <asp:Button ID="FictionalBtn" runat="server" CssClass="hidden" />
            <!-- DO-NOT DELETE (REFERENCE TO FUNCTIONAL METHOD ) -->

            <asp:ScriptManager ID="ScriptMngr1" runat="server"></asp:ScriptManager>

            <cc1:ModalPopupExtender ID="mp1" runat="server" PopupControlID="Panl1"
                CancelControlID="panelCloserBTN" BackgroundCssClass="Background" TargetControlID="FictionalBtn">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="Panl1" runat="server" CssClass="Popup" align="center" Style="display: none">

                <div class="container heading-fancy heb small" style="height: auto">
                    <h2 class="heb center">שיוך מופע לאולם קולנוע</h2>
                    <hr />
                    <div>
                        <b>המופע הנבחר: </b>
                        <u>
                            <asp:Label runat="server" ID="addToLiveMovieTitle">לא נבחר מופע</asp:Label></u>
                        <div class="heading-fancy" style="float: left;">
                            <asp:Image ID="ChoosenTitlePoster" runat="server" CssClass="poster_small" />
                        </div>
                        <br />
                        <br />
                        <br />
                        <label for="theatreSelector">בחר מספר אולם: </label>
                        <br />
                        <asp:DropDownList runat="server" ID="theatreSelector" Enabled="true" EnableViewState="true" CssClass="movie_selector"></asp:DropDownList>
                        <br />
                        <br />
                        <label for="DoremiElapsedTime">זמן נותר: </label>
                        <br />
                        <asp:TextBox ID="DoremiElapsedTime" runat="server" Width="100px" ToolTip="[hh:mm הכנס] לפי מחשב מקרן" OnTextChanged="DoremiElapsedTextBox_Changed" TextMode="Time"></asp:TextBox>

                        <div id="controls" style="margin-top: 50px; padding-bottom: 10px;">
                            <asp:Button ID="Button6" runat="server" OnClick="InsertMovieToLiveTable" Text="אישור" Style="float: right;" CssClass="ok" />
                            <asp:Button ID="panelCloserBTN" runat="server" Text="סגור" Style="float: left;" CssClass="cancel" />
                        </div>
                        <br />
                    </div>
                </div>
            </asp:Panel>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ValidationConnectionString %>" SelectCommand="SELECT * FROM [movies]"></asp:SqlDataSource>
            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ValidationConnectionString %>" SelectCommand="SELECT * FROM [movies] WHERE [IsRunning] = 1 ORDER BY [TheatreID]"></asp:SqlDataSource>
        </div>

        <asp:Timer ID="SyncTimer" runat="server" OnTick="SyncMovies" Enabled="True">
        </asp:Timer>
    </form>
    <div class="footer">
        <div style="float: right; text-indent: 25px;">
            סנכרון מתוזמן:
            <asp:Label ID="NextSyncLabel" runat="server" CssClass="sync_label">לא ידוע</asp:Label>
        </div>
        <br />
        <br />
        <div style="float: right; text-indent: 25px;">
            סנכרון אחרון:
            <asp:Label ID="LastSyncLabel" runat="server" CssClass="sync_label"></asp:Label>
        </div>
        <br />
        <br />
        <div style="text-indent: 25px;">
            סרטים חסרים:
            <asp:Label ID="FailedSyncCounter" runat="server" CssClass="sync_label"></asp:Label>
            <br />
            <asp:Label ID="SyncErrorLabel" runat="server" CssClass="sync_label" Style="float: left; direction: ltr;"></asp:Label>
        </div>
    </div>
</body>
</html>
