using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.XPath;
using AppCore.Helpers;
using XMLXSL;
using System.ComponentModel;
using System.Windows;
using System.IO;

namespace EMDRApp.Helpers
{
	public class EMDRAppConfig : AppConfig
	{
		#region Variables

		//public static string AppBasePath;

		public static bool IsAppBasePathInitialized;
		public static string EMDRAppConfigXMLFile = "XML/AppConfig.xml";

		#endregion

		#region Properties

		public static EMDRAppConfig EMDRAppConfigInstance
		{
			get { return _EMDRAppInstance ??= new EMDRAppConfig(); }
			set { _EMDRAppInstance = value; }
		}
		static EMDRAppConfig _EMDRAppInstance = null;

		public static XMLXSLBase EMDRAppConfigXmlBase
		{
			get
			{
				if ( _EMDRAppConfigXmlBase == null )
				{
					_EMDRAppConfigXmlBase = new XMLXSLBase();
					_EMDRAppConfigXmlBase.InitXMLFromFile( EMDRAppConfig.EMDRAppConfigXMLFile );
				}
				return _EMDRAppConfigXmlBase;
			}
			set
			{
				_EMDRAppConfigXmlBase = value;
			}
		}
        public static XMLXSLBase _EMDRAppConfigXmlBase = null;

		#endregion

		public EMDRAppConfig()
		{
			AppInstance = this;
			AppConfigName = "EMDRAppConfig";
		}

		internal void Init()
		{
		}

	}

	#region XPath strings

	public static class EMDRAppConfigXPath
	{
	}

	#endregion

	#region AppModes

	public static class EMDRAppModes
	{
		public const string LiveTrading = "LiveTrading";
		public const string Test = "Test";
		public const string BasicTest = "BasicTest";
		public const string EMDRAppAnalyzer = "EMDRAppAnalyzer";
		public const string EMDRAppMonitor = "EMDRAppMonitor";
		public const string EMDRAppHelper = "EMDRAppHelper";
		public const string EMDRAppWebProxy = "EMDRAppWebProxy";
		public const string PGCOMServer = "PGCOMServer";

		public const string SimpleTestApp = "SimpleTestApp";
		public const string EMDRAppProto = "EMDRAppProto";
		public const string IBTestMode = "IBTestMode";
		public const string Weekend = "Weekend";
	}

	#endregion

}
