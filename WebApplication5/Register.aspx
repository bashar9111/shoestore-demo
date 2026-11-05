<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="WebApplication5.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="panel auth">

    <h1>Create Account</h1><p>Use fictional details. Passwords must contain 12&#8211;128 characters.</p>

    <div style="margin-bottom:15px;">
        <label for="<%= txtUsername.ClientID %>">Username</label><br />
        <asp:TextBox ID="txtUsername" runat="server" Width="100%" Height="35"></asp:TextBox>

        <asp:RequiredFieldValidator
            ID="rfvUsername"
            runat="server"
            ControlToValidate="txtUsername"
            ErrorMessage="Username is required"
            ForeColor="Red">
        </asp:RequiredFieldValidator>
    </div>

    <div style="margin-bottom:15px;">
        <label for="<%= txtEmail.ClientID %>">Email</label><br />
        <asp:TextBox ID="txtEmail" runat="server" Width="100%" Height="35"></asp:TextBox>
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
        <label for="<%= txtConfirm.ClientID %>">Confirm Password</label><br />

        <asp:TextBox ID="txtConfirm"
            runat="server"
            TextMode="Password"
            Width="100%"
            Height="35"></asp:TextBox>

        <asp:CompareValidator
            ID="cvPassword"
            runat="server"
            ControlToCompare="txtPassword"
            ControlToValidate="txtConfirm"
            ErrorMessage="Passwords do not match"
            ForeColor="Red">
        </asp:CompareValidator>
    </div>

    <div style="text-align:center;">
        <asp:Button ID="btnRegister"
            runat="server"
            Text="Create Account"
            BackColor="#3a241c"
            ForeColor="White"
            Width="170"
            Height="40"
            OnClick="btnRegister_Click" />
    </div>

    <br />

    <div style="text-align:center;">
        <asp:Label role="status" ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>
    </div>

</div>

</asp:Content>
