<%@ Page Language="C#" Inherits="rmbcsharpaspdemo.ViewDrop" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head runat="server">
	<title>ViewDrop</title>
	<link href='/css/main.css' media='screen' rel='stylesheet' type='text/css' />
</head>
<body>	
	<div id="container">
		<a href="..">Back to drop list</a>
		<h1>Assets for drop <asp:Label id="dropNameLabel" runat="server"/></h1>
<%@ Import Namespace="Rmb.Core" %>
		<form id="addAssetForm" runat="server" enctype='multipart/form-data' method='post'>
			<ul>
				<li>
					<label for='file'>Add a new file:</label>
					<input name='file' size='25' type='file' />
				</li>
				<li>
					<input type="hidden" name="api_key" id="api_key" runat="server"/>
					<input name='drop_name' type='hidden' id="drop_name" runat="server" />
					<input name='conversion' type='hidden' value='BASE' />
					<input name="redirect_to" id="redirect_to" type="hidden" runat="server"/>
					<input name='version' type='hidden' value='3.0' />
					<input type='submit' value='submit' />
				</li>
			</ul>
		</form>

		<p>Or use flash uploader</p>
		<input type="file" name="fileUpload" id="file" />

		<asp:PlaceHolder id="messageBoxPlaceHolder" runat="server" />

		<asp:Repeater id="assetListRepeater" runat="server">

			<HeaderTemplate>
				<h2>Files in this drop</h2>
				<table>
			</HeaderTemplate>

			<ItemTemplate>
				<tr>
					<td>
						<img src="<%# DataBinder.Eval(Container.DataItem, "assetUrl") %>" />
					</td>
					<td>
						<%# DataBinder.Eval(Container.DataItem, "assetName") %>
					</td>	
					<td>
						<a href="?deleteasset=<%# DataBinder.Eval(Container.DataItem, "assetId") %>&name=<%= Request.QueryString["name"] %>" title="Delete">Delete</a>
					</td>
				</tr>
			</ItemTemplate>

			<FooterTemplate>
				</table>
			</FooterTemplate>

		</asp:Repeater>
	</div>
</body>
</html>