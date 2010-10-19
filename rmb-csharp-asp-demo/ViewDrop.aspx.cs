using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using Rmb.Core;
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace rmbcsharpaspdemo
{

	public partial class ViewDrop : System.Web.UI.Page
	{
		// called when the page is loaded
		public virtual void Page_Load (object sender, EventArgs args)
		{
			// add the script for the uploadify uploader
			addUploadifyScript();
			
			addAssetForm.Action = ServiceProxy.Instance.ServiceAdapter.UploadUrl;
			// if this is not a postback
			if (!IsPostBack)
			{
				// if the page load request contains a "deleteasset" parameter, delete the specified asset
				if (Request.QueryString["deleteasset"] != "" && Request.QueryString["deleteasset"] != null) {
					deleteAsset (Request.QueryString["deleteasset"]);
				}
				
				// put the drop name in the label at the top of the page
				dropNameLabel.Text = Request.QueryString["name"];
			
				// set the values of certain input parameters for the upload form:
				api_key.Value = ConfigurationManager.AppSettings["ApiKey"];
				drop_name.Value = Request.QueryString["name"];
				
				// set the redirect
				redirect_to.Value = createRedirectUrl();
			
				// show the assets in the drop
				updateAssetList();
			}

		}
		
		protected void deleteAsset( string assetId )
		{
			bool success;
			try
			{
				Drop drop = Drop.Find( Request.QueryString["name"] );
				Asset asset = Asset.Find( drop, Request.QueryString["deleteasset"] );
				success = asset.Destroy();
				
				// or, as a one-liner:
				//success = Asset.Find( Drop.Find( Request.QueryString["name"]), Request.QueryString["deleteasset"]).Destroy();
			}
			catch ( ServiceException exc )
			{
				// something went wrong, display the message returned by the exception
				createMessageBox( "failuremessage", exc.serviceMessage );
				return;
			}

			// display the success or failure of the deletion
			if( success == true )
				createMessageBox( "successmessage", "Asset was sucessfully deleted.");
			else if ( success == false )
				createMessageBox( "failuremessage", "There was a problem deleting the asset.");
		}

		// update the list of assets on the page (via the assetListRepeater repeater control)
		protected void updateAssetList ()
		{

			bool moreAssets = true;
			int page = 1;

			// create a new DataSet. This allows us to extract just the information we need out of the Asset object for
			// binding with the assetListRepeater repeater control.
			DataSet ds = new DataSet ();

			// create the table for the data set. Create a column for each data item we want to put into the dataset
			DataTable dt = new DataTable ();
			dt.Columns.Add (new DataColumn ("assetName", typeof(string)));
			dt.Columns.Add (new DataColumn ("assetUrl", typeof(string)));
			dt.Columns.Add (new DataColumn ("assetId", typeof(string)));
			// add this DataTable to the DataSet
			ds.Tables.Add (dt);

			//get assets
			while (moreAssets == true)
			{
				moreAssets = false;
				
				List<Asset> assets = new List<Asset> ();
				
				try
				{
					// get the list of assets
					Drop drop = Drop.Find (Request.QueryString["name"]);
					assets = drop.Assets (page);
//					ServiceProxy.Instance.ServiceAdapter.UploadUrl
				}
				catch (ServiceException exc)
				{
					// something went wrong, display the message returned by the exception
					createMessageBox( "failuremessage", exc.serviceMessage );
					return;
				}

				// iterate through each asset in the list of assets to extract the information we need to construct the
				// list of assets on the page
				foreach (Asset asset in assets)
				{
					// create a new data row to be added to our dataset
					DataRow dr = dt.NewRow();

					// asset name
					dr[0] = asset.Name;

					// asset large thumbnail url
					// interate through role contained in the asset object
					foreach( AssetRoleAndLocations roles in asset.Roles )
					{
						// iterate through each key in the role hashtable
						foreach( string roleItem in roles.Role.Keys )
						{
							// we want the "large_thumbnail" role for displaying an image preview. We don't care about
							// previews for other types, since this is just a simple demo. When we find it, go in and get
							// the url for the large_thumbnail file
							if  (  roles.Role[roleItem].ToString() == "large_thumbnail" )
							{
								// look for the url in the "DropioS3" location. Again, since this is a simple demo, we are
								// assuming that the asset is stored in the default (DropioS3) location
								foreach( Hashtable location in roles.Locations )
								{
									if( (string)location["name"] == "DropioS3")
										// save the url to the data row
										dr[1] = location["file_url"];
								}
							}
						}
					}
				
					// the id of the asset. This is needed to delete the asset
					dr[2] = asset.Id;
				
					// add this row to the data table
					dt.Rows.Add( dr );
				}
				
				// if we have 30 assets in the List object that means there are (possibly) more assets to get (30 assets
				// per page returned max), so increase the page and run this loop again
				if( assets.Count == 30 )
				{
					page++;
					moreAssets = true;
				}
			}

			// set the newly created DataStore as the data source for the repeater and bind the data
			assetListRepeater.DataSource = ds;
			assetListRepeater.DataBind();
		}

		// display a message box on the page
		protected void createMessageBox( string type, string message )
		{
			// use HtmlGenericControl because ASP doesn't have a div control
			HtmlGenericControl div = new HtmlGenericControl("div");
			
			// set the id and message to be displayed
			div.Attributes.Add( "id", type );
			div.InnerText = message;
			
			// put the div inside the placeholder
			messageBoxPlaceHolder.Controls.Add( div );
		}
		
		protected void addUploadifyScript ()
		{

			// get drop object
			Drop drop = Drop.Find( Request.QueryString["name"] );

			// options to add to the uploadify form
			Hashtable uploadifyOpts = new Hashtable();
			// do this when all uploads are complete
			uploadifyOpts.Add( "onAllComplete", "function() { setTimeout(window.location = '" + createRedirectUrl() + "', 1000); }");
			// do this when there's an error
			uploadifyOpts.Add( "onError", "function(e, q, f, o) { alert(\"ERROR: \" + o.info + o.type); }" );

			// add a parameter to the "scriptData" parameter
			Hashtable scriptData = new Hashtable();
			// do the base drop.io conversions
			scriptData.Add( "conversion", "BASE"); 
			uploadifyOpts.Add( "scriptData", scriptData );

			// add the script that uploadify needs to the page
			Type cstype = this.GetType (); 
			string csname = "uploadifyScript";

			ClientScriptManager cs = Page.ClientScript;

			if (!cs.IsStartupScriptRegistered (cstype, csname)) {
				// last arg is "false" because GetUplodifyForm already includes the script tags
				cs.RegisterStartupScript (cstype, csname, drop.GetUploadifyForm ( uploadifyOpts ), false);
			}
		}

		protected string createRedirectUrl ()
		{
			StringBuilder redirectUrl = new StringBuilder ("http://" + Request.ServerVariables["SERVER_NAME"]);

			// if the app is using a port other than 80, include it in the url string
			if (Request.ServerVariables["SERVER_PORT"] != "80")
				redirectUrl.Append (":" + Request.ServerVariables["SERVER_PORT"]);

			// append the query string (which contains the name of the drop we want to view)
			redirectUrl.Append (Request.ServerVariables["URL"] + "?name=" + Request.QueryString["name"]);

			return redirectUrl.ToString();

		}
	}
}