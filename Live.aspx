<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Live.aspx.cs" Inherits="Live" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" rel="stylesheet" href="css\style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="live heading-fancy heb">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="heb center">
                            <h2><%= GlobalVar.TheatreName %> | כרגע מקרינים
                            <div class="datetime">
                                <asp:Label ID="TheTime" runat="server" CssClass="big"></asp:Label>
                                <asp:Label ID="TheDate" runat="server" CssClass="big"></asp:Label>
                            </div>
                        </div>
                        </h2>
                        <hr />
                        <asp:ScriptManager ID="ScriptManager1" runat="server">
                        </asp:ScriptManager>

                        <asp:ListView ID="ListView1" runat="server" DataSourceID="SqlDataSource1">
                            <ItemTemplate>
                                <div class="inline heb">
                                    <div class="box_shadow">
                                        <span class="format">
                                            <asp:Label ID="FormatLabel" runat="server" Text='<%# Eval("Format") %>' />
                                        </span>
                                        אולם
                                <asp:Label ID="TheatreID" runat="server" Text='<%# Eval("TheatreID") %>' /></p>
                    <p class="movie_title">
                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("Title") %>' /><br>
                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("Runtime") %>' />
                    </p>
                                        <img class="poster heading-fancy" src="<%#Eval("Poster") %>" />
                                        <p>
                                            זמן נותר: 
                                            <asp:Label ID="ElapsedLabel" runat="server"></asp:Label>
                                        </p>

                                        <p>
                                            שעת סיום: 
                                <asp:Label ID="EndtimeLabel" runat="server" Text='<%# DateTime.Parse(Eval("EndTime").ToString()).ToString("HH:mm") %>' />
                                        </p>
                                        <asp:Label ID="CredLabel" runat="server" Text='<%# Eval("Credits") %>' CssClass="hidden" />
                                    </div>
                                </div>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                אין מופעים
                            </EmptyDataTemplate>
                            <LayoutTemplate>
                                <ul id="itemPlaceholderContainer" runat="server">
                                    <li runat="server" id="itemPlaceholder" />
                                </ul>
                            </LayoutTemplate>
                        </asp:ListView>
                        </div>
                                <asp:Timer ID="Timer1" runat="server" Interval="999" OnTick="Timer1_Tick">
                                </asp:Timer>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ValidationConnectionString %>" SelectCommand="SELECT * FROM [movies] WHERE IsRunning = 1 ORDER BY TheatreID;"></asp:SqlDataSource>
            </div>
        </div>
    </form>
</body>
</html>
