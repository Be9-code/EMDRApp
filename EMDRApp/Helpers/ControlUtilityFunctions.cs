using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AppCore.Models;

namespace EMDRApp.Controls
{
	public static class ControlUtilityFunctions
	{
		#region Variables

		public const double DefaultPriceIncrement = .10;
		public const int DefaultQuantityIncrement = 10;

		#endregion

		#region Properties

		#endregion

		#region Events

		#endregion

		public static double HandleMouseWheelPriceIncrement( TextBlock TextBlock, MouseWheelEventArgs e,
														double Increment = .01, bool NegativeOk = false )
		{
			string Text = TextBlock.Text;
			double Value = CalcPriceIncValue( e, Increment, Text );
			if ( Value < 0 && !NegativeOk )
				return -1;

			TextBlock.Text = BaseModel.DoubleToFixedString( Value );

			e.Handled = true;
			return Value;
		}

		public static double HandleMouseWheelPriceIncrement( TextBox TextBox, MouseWheelEventArgs e,
														double Increment = .01, bool NegativeOk = false )
		{
			string Text = TextBox.Text;
			double Value = CalcPriceIncValue( e, Increment, Text );
			if ( Value < 0 && !NegativeOk )
				return -1;

			TextBox.Text = BaseModel.DoubleToFixedString( Value );

			e.Handled = true;
			return Value;
		}

		public static double HandleMouseWheelPriceIncrement( ComboBox cBox, MouseWheelEventArgs e,
														double Increment = .01, bool NegativeOk = false )
		{
			string Text = cBox.Text;
			double Value = CalcPriceIncValue( e, Increment, Text );
			if (Value < 0 && !NegativeOk)
				return -1;

			cBox.Text = BaseModel.DoubleToFixedString( Value );

			e.Handled = true;
			return Value;
		}

		private static double CalcPriceIncValue( MouseWheelEventArgs e, double Increment, string Text )
		{
			double Value = -1;
			if ( !string.IsNullOrEmpty( Text ) )
			{
				double dValue = double.Parse( Text );
				double Diff = ((e.Delta / 120) * Increment);

				Value = dValue + Diff;
			}
			return Value;
		}

		public static int HandleMouseWheelIntIncrement( TextBlock TextBlock, MouseWheelEventArgs e,
														int Increment = DefaultQuantityIncrement, 
														bool NegativeOk = false )
		{
			int Value = -1;
			string Text = TextBlock.Text;
			if ( !string.IsNullOrEmpty( Text ) )
			{
				int dValue = int.Parse( Text );
				int Diff = ((e.Delta / 120) * Increment);

				Value = dValue + Diff;
				if ( Value < 0 && !NegativeOk )
					return 0;

				TextBlock.Text = Value.ToString();
			}

			e.Handled = true;
			return Value;
		}

		public static bool IsNumLockPressed()
		{
			return Keyboard.IsKeyToggled(System.Windows.Input.Key.NumLock);
		}
	}
}
