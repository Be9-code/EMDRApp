using AppCore.Models;
using EMDRApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;

namespace EMDRApp.Converters
{
	//<!-- usage --> 
	//<GenericBooleanConverter x:Key="BooleanToVisibility" TrueValue="Visible" FalseValue="Collapsed" /> 
	//<GenericBooleanConverter x:Key="BoolToOrientation" TrueValue="Vertical" FalseValue="Horizontal" /> 
	//<Converters:BoolToColorBrushConverter x:Key="BoolToColorBrushConverter" TrueValue="LightCyan" FalseValue="Black" />

	public class GenericConverterBase : IValueConverter
	{
		public object TrueValue { get; set; }
		public object FalseValue { get; set; }

		public virtual object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture ) 
		{ return null; }
		public virtual object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{ return null; }
	}

	public class GenericBooleanConverter : GenericConverterBase
	{
		// to the control
		public override object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			try
			{
				var Value = TypeDescriptor.GetConverter( targetType )
						.ConvertFrom( (bool)value ? TrueValue : FalseValue );
				return Value;
			}
			catch ( Exception ex )
			{
				var Message = ex.Message;
				return false;
			}
		}

		// from the control
		public override object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			try
			{
				var convertedTrueValue = TypeDescriptor
					.GetConverter( targetType ).ConvertFrom( TrueValue );

				return convertedTrueValue.Equals( value );
			}
			catch ( Exception ex )
			{
				var Message = ex.Message;
				return null;
			}
		}
	}

	public class BoolToThicknessConverter : GenericConverterBase
	{
		public override object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			Thickness ThicknessRef;

			string ValueString = value as string;
			var boolValue = ValueString.ToUpper() == "TRUE";
			if ( boolValue == true )
				ThicknessRef = new Thickness( System.Convert.ToDouble( TrueValue as string ) );
			else
				ThicknessRef = new Thickness( System.Convert.ToDouble( FalseValue as string ) );
			return ThicknessRef;
		}

	}

	public class BoolValueToColorBrushConverter : GenericConverterBase
	{
		public override object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			SolidColorBrush Brush = null;

			try
			{
				string[] values = ( parameter as string ).Split( ';' );
				if ( (bool)value )
					Brush = new SolidColorBrush( UtilityFunctions.ColorFromString( values[0] ) );
				else
					Brush = new SolidColorBrush( UtilityFunctions.ColorFromString( values[1] ) );
			}
			catch ( Exception ) { }
			return Brush;
		}

	}

	public class BoolStringToColorBrushConverter : GenericConverterBase
	{
		public override object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			SolidColorBrush Brush = null;

			try
			{
				string[] values = ( parameter as string ).Split( ';' );
				string ValueString = value as string;
				var boolValue = ValueString.ToUpper() == "TRUE";
				if ( boolValue )
					Brush = new SolidColorBrush( UtilityFunctions.ColorFromString( values[0] ) );
				else
					Brush = new SolidColorBrush( UtilityFunctions.ColorFromString( values[1] ) );
			}
			catch ( Exception ) { }
			return Brush;
		}

	}

	public class DirectionToColorBrushConverter : GenericConverterBase
	{
		public override object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			SolidColorBrush Brush = null;

			try
			{
				string[] values = (parameter as string).Split( ';' );
				string ValueString = value as string;
				if ( ValueString.ToUpper() == "UP" )
					Brush = new SolidColorBrush( UtilityFunctions.ColorFromString( values[0] ) );
				else if( ValueString.ToUpper() == "DOWN" )
					Brush = new SolidColorBrush( UtilityFunctions.ColorFromString( values[1] ) );
				else if ( ValueString.ToUpper() == "UPDOWN" )
					Brush = new SolidColorBrush( UtilityFunctions.ColorFromString( values[2] ) );
				else if ( ValueString.ToUpper() == "NONE" )
					Brush = new SolidColorBrush( UtilityFunctions.ColorFromString( values[3] ) );
			}
			catch ( Exception ) { }
			return Brush;
		}

	}

	public class TimeStringToTimeOnlyConverter : GenericConverterBase
	{
		public override object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			string TimeString = string.Empty;
			try
			{
				string ValueString = value as string;
				DateTime DateTimeRef = DateTime.Parse( ValueString );
				TimeString = DateTimeRef.ToShortTimeString();
			}
			catch ( Exception ) { }

			return TimeString;
		}

	}

	public class StringToTimelineConverter : GenericConverterBase
	{
		public override object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			string ValueString = value as string;
			var strings = ValueString.Split( ':' );
			int hours = System.Convert.ToInt32( strings[0] );
			int minutes = System.Convert.ToInt32( strings[0] );
			int seconds = System.Convert.ToInt32( strings[0] );
			return new Duration( new TimeSpan( hours, minutes, seconds ) );
		}

	}

	public class ColorStringToColorBrushConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			string ValueString = value as string;
			SolidColorBrush Brush = new SolidColorBrush( UtilityFunctions.ColorFromString( ValueString ) );
			return Brush;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{ return null; }
	}

	// e.g. Checkbox IsChecked to "true" : "false"
	public class TFBooleanConverter : IValueConverter
	{
		// to the control
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return TypeDescriptor.GetConverter( targetType )
					.ConvertFrom( (bool)ConverterHelpers.IsTrue( (string)value ) );
		}

		// from the control
		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			var convertedTrueValue = TypeDescriptor
				.GetConverter( targetType ).ConvertFrom( ConverterHelpers.BoolToTFString( (bool)value ) );

			return convertedTrueValue;
		}
	}

	[ValueConversion( typeof( bool ), typeof( GridLength ) )]
	public class BoolToGridRowHeightConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return ( (bool)value == true ) ? new GridLength( 1, GridUnitType.Star ) : new GridLength( 0 );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{    // Don't need any convert back
			return null;
		}
	}
	
	// String to Name
	//public class StringToNameConverter : IValueConverter
	//{
	//    // to the control
	//    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
	//    {
	//        string Value = (string)value;
	//        string ControlName = MTWindowBase.GetValidControlName( Value );
	//        return ControlName;
	//    }

	//    //// from the control
	//    //public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
	//    //{
	//    //    string Value = (string)value;
	//    //    return System.Convert.ToInt32( Value );
	//    //}
	//}

	// Int to String
	public class IntToStringConverter : IValueConverter
	{
		// to the control
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			int Value = System.Convert.ToInt32( value );
			return Value.ToString();
		}

		// from the control
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string Value = (string)value;
			return System.Convert.ToInt32( Value );
		}
	}

	// Double to FixedPrecision String
	public class DoubleToFixedStringConverter : IValueConverter
	{
		// to the control
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			double Value = (double)value;
			return BaseModel.DoubleToFixedString( Value );
		}

		// from the control
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string Value = (string)value;
			return System.Convert.ToDouble( Value );
		}
	}

	// Checkbox IsChecked to "Armed" : "DisArmed"
	public class CheckedToArmedConverter : IValueConverter
	{
		// to the control
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string Value = (string)value;
			return (Value == "Armed");
		}

		// from the control
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string Value = (bool)value ? "Armed" : "DisArmed";
			return Value;
		}
	}

	// TextBox IsChecked to "increases to" : "decreases to"
	//public class DirectionToUpDown : IValueConverter
	//{
	//	// to the control
	//	public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
	//	{
	//		string Value = (string)value == "Up" ? OrderDefs.IncreasesTo : OrderDefs.DecreasesTo;
	//		return Value;
	//	}

	//	// from the control
	//	public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
	//	{
	//		string Value = (string)value == OrderDefs.IncreasesTo ? "Up" : "Down";
	//		return Value;
	//	}
	//}

	public class BoolToStringConverter : IValueConverter
	{
		public char Separator
		{
			get { return _Separator; }
			set { _Separator = value; }
		}
		char _Separator = ';';

		public object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			var strings = ((string)parameter).Split( Separator );
			var trueString = strings[0];
			var falseString = strings[1];

			string ValueString = value as string;
			var boolValue = ValueString.ToUpper() == "TRUE";
			if ( boolValue == true )
				return trueString;
			else
				return falseString;
		}

		public object ConvertBack( object value, Type targetType, object parameter,
								  CultureInfo culture )
		{
			var strings = ((string)parameter).Split( Separator );
			var trueString = strings[0];
			var falseString = strings[1];

			string ValueString = value as string;
			var boolValue = ValueString.ToUpper() == "TRUE";
			if ( boolValue )
				return true;
			else
				return false;
		}
	}

	public class BoolStringToVisibleConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			string ValueString = value as string;
			if ( ValueString.ToUpper() == "TRUE" )
				return Visibility.Visible;
			else
				return Visibility.Collapsed;
		}

		public object ConvertBack( object value, Type targetType, object parameter,
								  CultureInfo culture )
		{
			string ValueString = value as string;
			var boolValue = ValueString.ToUpper() == "Visible";
			if ( boolValue )
				return true;
			else
				return false;
		}
	}

	public class BoolToYesNoStringConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			string ValueString = value as string;
			if ( ValueString.ToUpper() == "TRUE" )
				return "Yes";
			else
				return "No";
		}

		public object ConvertBack( object value, Type targetType, object parameter,
								  CultureInfo culture )
		{
			string ValueString = value as string;
			var boolValue = ValueString.ToUpper() == "YES";
			if ( boolValue )
				return true;
			else
				return false;
		}
	}

	public class BoolToUpDownStringConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			string ValueString = value as string;
			if ( ValueString.ToUpper() == "TRUE" )
				return "Up";
			else
				return "Down";
		}

		public object ConvertBack( object value, Type targetType, object parameter,
								  CultureInfo culture )
		{
			string ValueString = value as string;
			var boolValue = ValueString.ToUpper() == "TRUE";
			if ( boolValue )
				return true;
			else
				return false;
		}
	}

	public class BoolToOnOffStringConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter,
							  CultureInfo culture )
		{
			string ValueString = value as string;
			if ( ValueString.ToUpper() == "TRUE" )
				return "On";
			else
				return "Off";
		}

		public object ConvertBack( object value, Type targetType, object parameter,
								  CultureInfo culture )
		{
			string ValueString = value as string;
			var boolValue = ValueString.ToUpper() == "ON";
			if ( boolValue )
				return true;
			else
				return false;
		}
	}

	public class AttributesToEnumerableConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return (value as XmlElement).Attributes;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	public static class ConverterHelpers
	{
		public static bool IsTrue( string Value )
		{
			if ( !string.IsNullOrEmpty( Value ) )
				return Value == Boolean.TrueString || Value.ToUpper() == "TRUE";

			return false;
		}

		public static bool IsNull( object value )
		{
			return (value == null);
		}

		public static bool IsNotNull( object value )
		{
			return (value != null);
		}

		public static string BoolToTFString( bool Value, bool LowerCase = true )
		{
			string Result = Value ? Boolean.TrueString : bool.FalseString;
			if ( LowerCase )
				Result = Result.ToLower();

			return Result;
		}
	}

}
