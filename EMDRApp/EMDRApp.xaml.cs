using EMDRApp.Controllers;
using EMDRApp.Helpers;
using EMDRApp.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EMDR
{
	public partial class EMDRApp : Application
	{
		#region Variables
#if DEBUG
		public static string BasePath = System.AppDomain.CurrentDomain.BaseDirectory;
#else
		public static string BasePath = "";
#endif
		#endregion

		#region Properties

		public EMDRController appController
		{
			get { return EMDRController.Instance; }
		}

		#endregion

		#region Startup

		private void Application_Startup( object sender, StartupEventArgs e )
		{
			appController.InitEMDRController();
			EMDRAppConfig.EMDRAppConfigInstance.Init();

			// ==>> EMDRWindow
			var emdrWindow = new EMDRWindow();
			emdrWindow.Show();
		}

		#endregion
	}
}
