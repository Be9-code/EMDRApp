using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Threading;
using AppCore.Models;
using System.Windows;
using System.Diagnostics;
using AppCore.Helpers;
using System.Reflection;

namespace EMDRApp.Helpers
{
	public class UtilityFunctions
	{
		#region Variables

		public static string Success = "Success";
		public static string MachineName = AppConfig.GetMachineName();
		//public static bool IsNotificationsEnabled = true;

		//public static List<AppControlsNotificationWindow> NotifyWindows = new List<AppControlsNotificationWindow>();
		//public static List<string> WindowMessages = new List<string>();

		#endregion

		#region Properties

		//public static AppControlsNotificationWindow InfoWindow;


		#endregion

		//public static void DisplayForcedNotification( string Message, double Delay = 3,
		//									 string Name = "StandardDefaults",
		//									 bool Force = false )
		//{
		//	DisplayNotification( Message, Delay, Name, true );
		//}

		//public static void DisplayNotification( string Message, double Delay = 3,
		//									 string Name = "StandardDefaults",
		//									 bool Force = false )
		//{
		//	// ==>>> Simple fix/hack: don't display redundant messages
		//	if (!Force && IsNotificationMessage(Message))
		//		return;

		//	WindowMessages.Add(Message);

		//	if ( MTDefs.AppDispatcher != null && (IsNotificationsEnabled || Force) )
		//	{
		//		MTDefs.AppDispatcher.Invoke( DispatcherPriority.ApplicationIdle, new Action( () =>
		//		{
		//			InitNotificationWindow( Message, Name, Delay );
		//		} ) );
		//	}
		//	else
		//		InitNotificationWindow( Message, Name, Delay );

		//	// Note: ==>>> on DisplayNotification(), also display in TradingUpdatesController
		//	TradingUpdatesController.AppendMessage( Message );
		//}

		//public static void InitNotificationWindow( string Message, string Name, double Delay )
		//{
		//	// Note: ===>>> XML UI customizations happen here

		//	// get the UIDef node, with any customizations or overrides
		//	XmlElement xmlConfig = UIDefsModel.GetPopUpNotificationXMLNode( Name );

		//	int ConfigDelay = BaseModel.GetXmlNodeIntAttribute( xmlConfig, "Delay" );
		//	if ( IsNotMinusOne( ConfigDelay ) )
		//		Delay = ConfigDelay;

		//	var NotifyWindow = new AppControlsNotificationWindow( Message, Delay, xmlConfig );
		//	//NotifyWindows.Add(NotifyWindow);

		//	NotifyWindow.OnWindowClosedEvent += OnNotifyWindowClosed;
		//}

  //      private static void OnNotifyWindowClosed(Window window)
  //      {
		//	var msgWindow = window as AppControlsNotificationWindow;
		//	//RemoveNotificationWindow(msgWindow);

		//	if (msgWindow != null && WindowMessages.Contains(msgWindow.Message))
		//		WindowMessages.Remove(msgWindow.Message);
		//}

  //      public static bool IsNotificationMessage(string Message)
		//{
		//	Debug.WriteLine($"Message: {Message}");

		//	//int Count = 1;
		//	//foreach (var message in WindowMessages)
  // //         {
		//	//	Debug.WriteLine($"{Count++}: {message}");
  // //         }
		//	return (WindowMessages.Contains(Message));
		//}

  //      public static bool IsNotificationWindow(string Message)
		//{
		//	Debug.WriteLine($"Message: {Message}");

		//	//int Count = 1;
		//	//foreach (var window in NotifyWindows)
  // //         {
		//	//	Debug.WriteLine($"{Count++}: {window.Message}");
  // //         }
		//	return (NotifyWindows.Find(x => x.Message == Message) != null);
		//}

		//public static void RemoveNotificationWindow(AppControlsNotificationWindow window)
		//{
		//	if (window != null && NotifyWindows.Contains(window))
		//		NotifyWindows.Remove(window);
		//}


		//public static void DisplayInfoWindow( string Message, string Name = "InfoWindowDefaults", 
		//									double Delay = -1 )
		//{
		//	InfoWindow = new AppControlsNotificationWindow();

		//	if ( IsNotNull( MTDefs.AppDispatcher ) )
		//	{
		//		MTDefs.AppDispatcher.Invoke( DispatcherPriority.ApplicationIdle, new Action( () =>
		//		{
		//			InitInfoWindow( Message, Name, Delay );
		//		} ) );
		//	}
		//	else
		//		InitInfoWindow( Message, Name, Delay );

		//	//TradingUpdatesController.AppendMessage( Message );
		//}

		//private static void InitInfoWindow( string Message, string Name, double Delay = -1 )
		//{
		//	// get the UIDef node, with any customizations or overrides
		//	XmlElement xmlConfig = UIDefsModel.GetPopUpNotificationXMLNode( Name, "InfoWindowDefaults" );

		//	if ( IsMinusOne( Delay ) )
		//	{
		//		int ConfigDelay = BaseModel.GetXmlNodeIntAttribute( xmlConfig, "Delay" );
		//		if ( IsNotMinusOne( ConfigDelay ) )
		//			Delay = ConfigDelay;
		//	}

		//	InfoWindow.OnWindowOpenedEvent += OnInfoWindowOpened;
			
		//	InfoWindow.InitPopUpNotificationWindow( Message, Delay, xmlConfig );

		//	//InfoWindow.Show();
		//	//InfoWindow.Activate();

		//	InfoWindow.OnWindowClosedEvent += OnInfoWindowClosed;
		//}

		//private static void OnInfoWindowOpened( Window window )
		//{
		//}

		//private static void OnInfoWindowClosed( Window window )
		//{
		//	InfoWindow.OnWindowOpenedEvent -= OnInfoWindowOpened;
		//	InfoWindow.OnWindowClosedEvent -= OnInfoWindowClosed;
		//}

		public static string RemoveCommas( string Text )
		{
			return Text.Replace( ",", "" );
		}

		public static string RemoveSpaces( string Text )
		{
			return Text.Replace( " ", "" );
		}

		public static void DisplayError( string Message )
		{
		}

		public static string InitForDollars( string Value )
		{
			return string.Format( "${0}", Value );
		}

		public static bool IsErrorString( string Text )
		{
			if ( !string.IsNullOrEmpty(Text) )
				return Text.IndexOf( "Error" ) != -1;
			return false;
		}

		public static bool IsSuccess( string Text )
		{
			if ( !string.IsNullOrEmpty(Text) )
				return Text == UtilityFunctions.Success;
			return false;
		}

		public static string CleanName( string Name )
		{
			Regex regex = new Regex( @"[\W]+" );

			Name = Name.Replace( "%", "" );
			//Name = Name.Replace( ".", "" );
			string cleanName = regex.Replace( Name, "" ).ToUpper();
			return cleanName;
		}

		public static string RemoveCRLFsAndTabs( string Text )
		{
			Text = Regex.Replace( Text, @"[\t\n\r]+", "" );
			return Text;
		}

		public static string RemoveErrorPrompt( string Error )
		{
			return Error.Replace( "Error: ", "" );
		}

		public static string RemoveWhitespace( string Text )
		{
			Regex r = new Regex( @"\s+" );

			return r.Replace( Text, @" " );
		}

		public static string BoolToTrueFalseString( bool bIsTrue )
		{
			return bIsTrue ? bool.TrueString : bool.FalseString;
		}

		public static bool StringToBool( string bIsBool )
		{
			return bIsBool == "true" ? true : false;
		}

		public static void InitBackgroundForegroundColors( XmlElement xmlColorsNode, out SolidColorBrush ControlBackgroundBrush, out SolidColorBrush ControlForegroundBrush )
		{
			ControlBackgroundBrush = ControlForegroundBrush = null;
			string Color = xmlColorsNode.GetAttribute( "Background" );
			if ( IsNotNull( Color ) )
				ControlBackgroundBrush = new SolidColorBrush( UtilityFunctions.ColorFromString( Color ) );
			Color = xmlColorsNode.GetAttribute( "Foreground" );
			if ( IsNotNull( Color ) )
				ControlForegroundBrush = new SolidColorBrush( UtilityFunctions.ColorFromString( Color ) );
		}

		public static Color ColorFromString( string ColorString )
		{
			Color color = Colors.White;
			if ( !string.IsNullOrEmpty( ColorString ) )
			{
				color = (Color)TypeDescriptor.GetConverter( typeof( Color ) ).ConvertFromString( ColorString );
				//new BrushConverter().ConvertFromString( "White" ) as SolidColorBrush
			}
			return color;
		}

		public static string GetColorName(Color col, string Default = "Blue")
		{
			PropertyInfo colorProperty = typeof(Colors).GetProperties()
				.FirstOrDefault(p => Color.AreClose((Color)p.GetValue(null), col));
			return colorProperty != null ? colorProperty.Name : Default;
		}

		public static Color ColorFromArgbString( string ColorString )
		{
			Color c = Colors.White;
			try
			{
				string[] ColorValues = ColorString.Split( ',' );
				if ( ColorValues.Length == 3 )
					c = Color.FromArgb( 0xFF, Convert.ToByte( ColorValues[0] ), 
										Convert.ToByte( ColorValues[1] ), Convert.ToByte( ColorValues[2] ) );
			}
			catch ( System.Exception ex )
			{
				//string StackTrace = ex.StackTrace;
				string Message = ex.Message;
			}
			return c;
		}

		public static void InitTextBlock( TextBlock textBlock, string Text, string ColorString )
		{
			textBlock.Text = Text;
			textBlock.Foreground = new SolidColorBrush( UtilityFunctions.ColorFromString( ColorString ) );
		}

		public static void InitTextBox( TextBox textBlock, string Text, string ColorString )
		{
			textBlock.Text = Text;
			textBlock.Foreground = new SolidColorBrush( UtilityFunctions.ColorFromString( ColorString ) );
		}

		//public static DateTime AdjustForTimeZone( DateTime dateTime, string TimeZone = "Eastern Standard Time" )
		//{
		//	TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById( TimeZone );
		//	DateTime dt = TimeZoneInfo.ConvertTimeFromUtc( dateTime, tzi );
		//	return dt;
		//}

		//public static DateTimeOffset ConvertUtcTimeToTimeZone( this DateTime dateTime, string toTimeZoneDesc )
		//{
		//	if ( dateTime.Kind != DateTimeKind.Utc ) throw new Exception( "dateTime needs to have Kind property set to Utc" );
		//	var toUtcOffset = TimeZoneInfo.FindSystemTimeZoneById( toTimeZoneDesc ).GetUtcOffset( dateTime );
		//	var convertedTime = DateTime.SpecifyKind( dateTime.Add( toUtcOffset ), DateTimeKind.Unspecified );
		//	return new DateTimeOffset( convertedTime, toUtcOffset );
		//}

		// ----------------------------------------------------

		#region Helper functionality

		public static bool IsTrue( string Value )
		{
			if ( !string.IsNullOrEmpty( Value ) )
				return Value == Boolean.TrueString || Value.ToUpper() == "TRUE";

			return false;
		}

		public static bool IsNull( object value )
		{
			return ( value == null );
		}

		public static bool IsNotNull( object value )
		{
			return ( value != null );
		}

		public static bool IsMinusOne( int value )
		{
			return ( value == -1 );
		}

		public static bool IsMinusOne( double value )
		{
			return ( value == -1 );
		}

		public static bool IsNotMinusOne( int value )
		{
			return ( value != -1 );
		}

		public static bool IsNotMinusOne( double value )
		{
			return ( value != -1 );
		}

		public static int ToInt( string ValueString, int Digits = 2 )
		{
			int Value = int.Parse( ValueString );
			return Value;
		}

		public static double ToDouble( string ValueString, int Digits = 2 )
		{
			double Value = double.Parse( ValueString );
			return Value;
		}

		public static bool IsZero( double Value )
		{
			return Value == 0;
		}

		public static bool IsNotZero( double Value )
		{
			return Value != 0;
		}

		#endregion
	}

	#region Suppport classes

	#endregion
}