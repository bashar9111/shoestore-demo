<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication5.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="panel auth">

        <h1 style="text-align:center;">Login</h1>

        <div style="margin-bottom:15px;">
            <label for="<%= txtUsername.ClientID %>">Username</label><br />
            <asp:TextBox ID="txtUsername" runat="server" Width="100%" Height="35"></asp:TextBox>
        </div>

        <div style="margin-bottom:15px;">
            <label for="<%= txtPassword.ClientID %>">Password</label><br />
            <asp:TextBox ID="txtPassword"
                runat="server"
                TextMode="Password"
                Width="100%"
                Height="35"></asp:TextBox>
        </div>

        <div style="margin-bottom:15px;">
            <asp:CheckBox ID="chkRemember" runat="server" Text="Remember Me" />
        </div>

        <div style="text-align:center;">
            <asp:Button ID="btnLogin"
                runat="server"
                Text="Login"
                BackColor="#3a241c"
                ForeColor="White"
                Width="150"
                Height="40"
                OnClick="btnLogin_Click" />
        </div>

        <br />

        <div style="text-align:center;">
            <asp:Label role="status" ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
        </div>

    </div>

</asp:Content>
