<%@ Page Language="C#" Inherits="rmbcsharpaspdemo.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head runat="server">
	<title>Drop.io ASP.NET API Simple Demo</title>
	<link href='css/main.css' media='screen' rel='stylesheet' type='text/css' />
</head>
<body>
	<div id="container">	

		<h1>Drop.io API Simple Demo for ASP.NET</h1>
		<p>This demo provides a simple examples which will get you started on making your own apps which use the Drop.io Rich Media Backbone.</p>
		<p>This is a fully functional demo app that you can use to manage your drops. To get started all you need to do is edit the <i>web.config</i>
			file with your <i>api key</i> and <i>api secret</i> if you are using secured keys.</p>
		<p>Note: api secret is optional if you are using unsecured api keys.</p>
	
		<hr />	
	
		<p><b>Your API key: <asp:Label runat="server" id="apiKeyLabel" /></b></p>
	
		<hr />	 
		
		<asp:PlaceHolder id="messageBoxPlaceHolder" runat="server" />
	
		<form method="get" id="createDropForm" runat="server">
			<label>Create New Drop:</label>
			<asp:TextBox id="newDropNameTextBox" runat="server" />
			<asp:Button id="createDropButton" runat="server" text="Submit Query" OnClick="createDropButton_Click"/>
			<asp:Button id="createRandomDropButton" runat="server" Text="Generate Random Drop" OnClick="createRandomDropButton_Click"/>

	

		
		<div>

			<asp:Repeater id="dropListRepeater" runat="server">
				<HeaderTemplate>
					<h2>Your Drops</h2>
				</HeaderTemplate>
				
				<ItemTemplate>
				
					<div class="dropname">
						<a href="ViewDrop.aspx?name=<%# DataBinder.Eval(Container.DataItem, "Name") %>" title="View <%# DataBinder.Eval(Container.DataItem, "Name") %>"><%# DataBinder.Eval(Container.DataItem, "Name") %></a>
					</div>
					
					<div class="actionlink delete">
						<a href="?deletedrop=<%# DataBinder.Eval(Container.DataItem, "Name") %>" title="Delete <%# DataBinder.Eval(Container.DataItem, "Name") %>">Delete</a>
					</div>
					
					<div style="clear:both"></div>
					
				</ItemTemplate>
			</asp:Repeater>
		</div>
		</form>
	</div>
</body>
</html>