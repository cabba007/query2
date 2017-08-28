<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ruyuanchuyuan.aspx.cs" Inherits="WebApplication2.ruyuanchuyuan" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
    table {
        font-family: verdana,arial,sans-serif;
        font-size:10px;
        color: #333333;
        border-width: 1px;
        border-color: #666666;
        border-collapse: collapse;
    }

     th {
        border-width: 1px;
        padding: 2px;
        border-style: solid;
        border-color: #666666;
        background-color: #ececff;
    }

     td {
        border-width: 1px;
        padding: 2px;
        border-style: solid;
        border-color: #666666;
        background-color: #ffffff;
    }
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
    
    </div>
        <p>
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="导出Word" />
            <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="导出Excel" />
        </p>
    </form>
</body>
</html>
