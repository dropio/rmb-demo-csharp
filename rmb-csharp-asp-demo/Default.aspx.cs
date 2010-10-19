using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Rmb.Core;

namespace rmbcsharpaspdemo
{

	public partial class Default : System.Web.UI.Page  
	{
	
		// method called anytime the page is loaded
		public virtual void Page_Load (object sender, EventArgs args)
		{

			// if this is not a postback
			if (!IsPostBack) { 
			
				// if the page load request contains a "deletedrop" parameter, delete the specified drop
				if (Request.QueryString["deletedrop"] != "" && Request.QueryString["deletedrop"] != null) {
					deleteDrop( Request.QueryString["deletedrop"]); 
				} 
				
				// display the api key on the page. the api key is set in the web.config file
				apiKeyLabel.Text = ConfigurationManager.AppSettings["ApiKey"];
				ServiceProxy.Instance.ServiceAdapter.ApiKey = ConfigurationManager.AppSettings["ApiKey"];
				
				// display the list of drops
				updateDropList();
			}
		}

		// delete a drop
		protected void deleteDrop (string dropName)
		{
			bool success;
			
			try
			{
				// find the drop and delete
				Drop drop = Drop.Find (dropName);
				success = drop.Destroy ();
			}
			catch (ServiceException exc)
			{
				// something went wrong, display the message returned by the exception
				createMessageBox ("failuremessage", exc.serviceMessage);
				return;
			}
			
			// display the success or failure of the deletion
			if (success == true)
				createMessageBox ("successmessage", "Drop was successfully deleted."); 
			else if (success == false)
				createMessageBox ("failuremessage", "There was a problem deleting the drop.");
			
			// note that we don't update the drop list here, this is because the update will be taken care of by the 
			// rest of the "if (!IsPostBack)" block in the Page_Load() method

		}

		// display a message box on the page (in the messageBoxPlaceHolder PlaceHolder control)
		protected void createMessageBox( string type, string message )
		{
			// create a new div (there is not 'div' control, we have must use HtmlGenericControl)
			HtmlGenericControl div = new HtmlGenericControl( "div" );
			
			// set the id attribute so that the message box will be rendered as specified by that style in the stylesheet
			div.Attributes.Add( "id", type );
			
			// add the message to display and add the div to the page
			div.InnerText = message;
			messageBoxPlaceHolder.Controls.Add( div );
		}

		// update the list of drops on the main page (via the dropListRepeater repeater control)
		protected void updateDropList ()
		{
			// all drops returned will be put into 'drops'. This is because the maximum number of drops returned is
			// 30, so if there is more than 30 we have to call Drop.FindAll() more than once
			List<Drop> allDrops = new List<Drop> ();

			// should we try to get more drops?
			bool moreDrops = true;

			// which page of results we want returned
			int page = 1;

			while (moreDrops == true)
			{
				moreDrops = false;
				List<Drop> drops;

				// get the drops for the current page
				try
				{
					drops = Drop.FindAll (page);
				}
				catch (ServiceException exc)
				{
					// something went wrong, display the message returned by the exception
					createMessageBox ("failuremessage", exc.serviceMessage);
					return;
				}

				// add the contents of this List to the allDrops List.
				allDrops.AddRange (drops);

				// if we have 30 drops that means there are (possibly) more drops, so increase the page and go run this
				// block again
				if( drops.Count == 30 )
				{
					page++;
					moreDrops = true;
				}
			}

			// we now have all the drops, set the List of all drops as the data source for the repeater and bind
			dropListRepeater.DataSource = allDrops;
			dropListRepeater.DataBind ();
		}

		// create a new named drop button clicked
		protected void createDropButton_Click (object sender, EventArgs e)
		{
			createDrop( newDropNameTextBox.Text );
		}
		
		protected void createRandomDropButton_Click (object sender, EventArgs e)
		{
			createDrop( string.Empty );
		}

		protected void createDrop( string dropName )
		{
		
			try
			{
				// if a drop name is specified, use it
				if (dropName != string.Empty) {
					Hashtable newDropHash = new Hashtable ();
					newDropHash.Add ("name", dropName );
					Drop.Create (newDropHash);
				}
				else
				{
					// otherwise create it with a random name
					Drop.Create ();
				}
			}
			catch (ServiceException exc)
			{
				// something went wrong, display the message returned by the exception
				createMessageBox( "failuremessage", exc.serviceMessage );
				return;
			}
			
			// make the new drop show up on the page
			updateDropList();
	
			// let the user know everything went ok
			createMessageBox( "successmessage", "Drop was successfully created.");

		}
	}
}