using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EMDRApp.Models;
using AppCore.Helpers;
using EMDRApp.Controls;
using GenericUtilityFunctions;
using AppCore.Controllers;
using EMDRApp.Windows;
using System.Windows;
using System.Windows.Media;

namespace EMDRApp.Controllers
{
	public delegate void OnNoParamsDel();
	public delegate void OnObjectParamDel(object obj);
	public delegate void OnIntParamDel(int Value);

	public class EMDRController : BaseController
	{
		#region Variables

		bool AppOnStartupInitialized;

		public EMDRDisplayWindow emdrDisplayWindow;

		#endregion

		#region Properties

		public static EMDRController Instance
		{
			get { return _Instance ??= new EMDRController(); }
			set { _Instance = value; }
		}
		static EMDRController _Instance;

        public EMDRAppModel emdrAppModel
        {
            get { return _AppModel ??= EMDRAppModel.Instance; }
            set { _AppModel = value; }
        }
        EMDRAppModel _AppModel;

        #endregion

        #region Events

        //public event OnNoParamsDel OnAppControllerEvent;

		#endregion

		public EMDRController()
		{
			Instance = this;
			InitEMDRController();
			//AppCoreUtil.ExecAfterSecondsDelay( 1, InitEMDRController );
		}

		#region Initialization

		public void InitEMDRController()
		{
			InitEvents();

			//EMDRLedsTimer = new GenericTimer();
		}

		public void InitAppOnStartup()
		{
			if (!AppOnStartupInitialized)
			{
				AppOnStartupInitialized = true;

			}
		}

		#endregion

		#region InitEvents/UnInitEvents

		public override void InitEvents()
		{
			UnInitEvents();
			base.InitEvents();

		}

        public override void UnInitEvents()
		{
			base.UnInitEvents();

		}

        internal void OnNumberOfLedsChanged(int Value)
        {
            if (emdrDisplayWindow != null)
            {

            }
        }

		internal void OnColorSelectionChanged(Window window, Color ColorRef)
		{
		}

		internal void OnEMDRDisplayWindowClosing(object obj)
		{
			//emdrDisplayWindow = null;
		}

        #endregion

    }

}
