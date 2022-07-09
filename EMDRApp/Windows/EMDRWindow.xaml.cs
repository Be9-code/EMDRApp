using EMDRApp.Controllers;
using EMDRApp.Controls;
using EMDRApp.Helpers;
using EMDRApp.Models;
using GenericUtilityFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EMDRApp.Windows
{
	public delegate void OnColorSelectionChangedDel(Window window, Color ColorRef);

	public partial class EMDRWindow : Window
	{
		#region Variables

		protected bool CloseOnEscKey;
		protected bool ExitOnEscKey;
		
		bool AutoStart;
		bool SingleStep;

		public EMDRLedsBarControl emdrLedsBarControl;

		CallbackTimer NumberOfLedsMouseWheelTimer;

		#endregion

		#region Properties

		public EMDRDisplayWindow emdrDisplayWindow
		{
			get { return _emdrDisplayWindow ??= new EMDRDisplayWindow(emdrValues); }
			set { _emdrDisplayWindow = value; }
		}
		EMDRDisplayWindow _emdrDisplayWindow;

		public EMDRValues emdrValues
		{
			get { return _emdrValues ??= new EMDRValues(); }
			set { _emdrValues = value; }
		}
		EMDRValues _emdrValues;

		public EMDRController emdrController
		{
			get { return EMDRController.Instance; }
		}

		#endregion

		#region Events

		public event OnIntParamDel OnNumberOfLedsChangedEvent;
		public event OnColorSelectionChangedDel OnColorSelectionChangedEvent;

		#endregion

		public EMDRWindow()
		{
			InitializeComponent();

			DataContext = emdrValues;
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			EMDRAppModel.Instance.InitEMDRValues(emdrValues);

			EMDRDot.DataContext = emdrValues;
			InitSpeedSliders();

			emdrLedsBarControl = emdrDisplayWindow.EMDRLedsBar;

#if DEBUG
			ExitOnEscKey = true;
#endif
			InitEvents();
		}

        private void InitSpeedSliders()
        {
			SpeedSlider.Ticks.Clear();

			double Value = SpeedSlider.Minimum;
			while (Value <= SpeedSlider.Maximum)
            {
				SpeedSlider.Ticks.Add(Value);
				Value += emdrValues.SpeedIncrement;
			}

			SpeedSlider.Value = emdrValues.Speed;

			SpeedCoefficientSlider.Value = emdrValues.SpeedCoefficient;
			SpeedMultiplierSlider.Value = emdrValues.SpeedMultiplier;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//if (ExitOnEscKey)
			{
				Application.Current.Shutdown();
			}
		}

		#region Key down handlers

		private void OnWindow_KeyDown(object sender, KeyEventArgs e)
		{
			bool ShiftKey = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			bool AltKey = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
			bool CtrlKey = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

			switch (e.Key)
			{
				case Key.Escape:
					// If this is the escape key
					if (ExitOnEscKey)
					{
						Application.Current.Shutdown();
					}
					else if (CloseOnEscKey)
					{
						Close();
					}
					break;
				case Key.Space:
					btnEMDRPause_Click(null, null);
					break;
				case Key.Enter:
					break;
				case Key.R:
					if (CtrlKey)
					{
						e.Handled = true;
					}
					break;
				case Key.H:
					if (CtrlKey)
					{
						//if ( ShiftKey )
						//{
						//	OnResetHighLowEvent?.Invoke( WndSymbol, true );
						//	OnResetHighLowEvent?.Invoke( WndSymbol, false );
						//}
						//else
						//{
						//	OnResetHighLowEvent?.Invoke( WndSymbol, ShiftKey );
						//}
						e.Handled = true;
					}
					break;
				case Key.Right:
					break;
				case Key.Left:
					break;
			}

			//OnWindow_KeyDown(sender, e);
		}

		#endregion


		#region InitEvents/UnInitEvents

		public void InitEvents()
		{
			UnInitEvents();

			if (emdrDisplayWindow != null)
            {
				emdrDisplayWindow.OnWindow_FullyLoadedEvent += (window) => { OnEMDRDisplayWindowReady(); };
				emdrDisplayWindow.Closed += (sender, e) => OnEMDRDisplayWindowClosed();
			}

			OnNumberOfLedsChangedEvent += EMDRController.Instance.OnNumberOfLedsChanged;
			OnColorSelectionChangedEvent += EMDRController.Instance.OnColorSelectionChanged;
		}

        private void OnEMDRDisplayWindowClosed()
        {
			emdrDisplayWindow = null; 
			Focus();
        }

        public void UnInitEvents()
		{
			OnNumberOfLedsChangedEvent -= EMDRController.Instance.OnNumberOfLedsChanged;
			OnColorSelectionChangedEvent -= EMDRController.Instance.OnColorSelectionChanged;
		}

		#endregion

		private void btnEMDRPlay_Click(object sender, RoutedEventArgs e)
        {
			bool ShiftKey = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			bool AltKey = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
			bool CtrlKey = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

			if ( CtrlKey && !SingleStep)
				emdrLedsBarControl.SetEMDRLedsOpacity(0);

            SingleStep = CtrlKey;

			if (SingleStep)
			{
				emdrLedsBarControl.SetEMDRLedsOpacity(0);
				emdrLedsBarControl.OnEMDRLedsTimerTimeout();
			}
			else
			{
				StartEMDRDisplay();
			}

			e.Handled = true;
		}

		private void StartEMDRDisplay()
		{
			if (emdrDisplayWindow is null)
				emdrDisplayWindow = new EMDRDisplayWindow(emdrValues);

			if (!SingleStep)
			{ 
				var mSecs = SetSpeedTimeouts();
				emdrDisplayWindow.StartEMDRDisplay(mSecs);
			}
			emdrDisplayWindow.Show();
			emdrDisplayWindow.Activate();

			AutoStart = false;
		}

		private void btnEMDRPause_Click(object sender, RoutedEventArgs e)
        {
			emdrDisplayWindow.EMDRLedsTimer.PauseGenericTimer(emdrDisplayWindow.EMDRLedsTimer.IsEnabled);
			e.Handled = true;
		}

		private void btnEMDRStop_Click(object sender, RoutedEventArgs e)
        {
			emdrDisplayWindow.KillEMDRLedsTimer();
			emdrLedsBarControl.SetEMDRLedsOpacity(0);

			e.Handled = true;
		}

		#region TimerTimeout

		#endregion

        #region Show, etc.

        private void btnEMDRShow_Click(object sender, RoutedEventArgs e)
        {
			bool ShiftKey = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			bool AltKey = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
			bool CtrlKey = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

			if (CtrlKey)
			{
				//OpenEMDRDisplayWindow( true );
				foreach (var control in emdrLedsBarControl.EMDRLedControls)
					control.RunEMDRDotAnimation();
			}
			else
            {
				if (!emdrDisplayWindow.IsVisible)
				{
					emdrDisplayWindow.Show();
				}
				else
				{
					emdrDisplayWindow.Hide();
				}
			}
		}

		private void OnEMDRDisplayWindowReady()
		{
			if (AutoStart)
				StartEMDRDisplay();
		}

		private void btnEMDRSettings_Click(object sender, RoutedEventArgs e)
        {
			bool ShiftKey = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			bool AltKey = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
			bool CtrlKey = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

			// SetDefaultSettings
			if (CtrlKey)
            {
				EMDRAppModel.Instance.SetDefaultSettings();
				EMDRAppModel.Instance.InitEMDRValues(emdrValues);
			}

			EMDRValuesPopup.IsOpen = !EMDRValuesPopup.IsOpen;
			e.Handled = true;
        }

		#region EMDRValuesPopup

		private void EMDRValuesPopup_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void EMDRValuesPopup_Closed(object sender, EventArgs e)
        {

        }

        private void EMDRValuesPopup_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
			EMDRValuesPopup.IsOpen = false;
			e.Handled = true;
		}

		#endregion

		private void SpeedCoefficientSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
			if (IsInitialized)
            {
				emdrValues.SpeedCoefficient = (sender as Slider).Value;
				UpdateTimerValues();
			}
			e.Handled = true;
		}

		private void SpeedMultiplierSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
			if (IsInitialized)
            {
				emdrValues.SpeedMultiplier = (sender as Slider).Value / 10;
				UpdateTimerValues();
			}
			e.Handled = true;
		}

		private void NumberOfLeds_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TextBlock TextBlock = sender as TextBlock;
            emdrValues.NumberOfLeds = ControlUtilityFunctions.HandleMouseWheelIntIncrement(TextBlock, e, 1);

			if (NumberOfLedsMouseWheelTimer is null)
				NumberOfLedsMouseWheelTimer = new CallbackTimer(250, OnPCPnLAdornerMouseWheelTimerTimeout);

			e.Handled = true;
		}

		private void OnPCPnLAdornerMouseWheelTimerTimeout()
        {
            if (NumberOfLedsMouseWheelTimer != null)
                NumberOfLedsMouseWheelTimer.StopGenericTimer();
            NumberOfLedsMouseWheelTimer = null;

            ReopenEMDRDisplayWindow();

            //emdrLedsBarControl.InitEMDRLedControls();

            //UpdateTimerValues();

            //emdrDisplayWindow.SetWidth();

            OnNumberOfLedsChangedEvent?.Invoke(emdrValues.NumberOfLeds);
        }

        private void ReopenEMDRDisplayWindow()
        {
			AutoStart = true;

			if (emdrDisplayWindow != null)
				emdrDisplayWindow.Close();

            emdrDisplayWindow = new EMDRDisplayWindow(emdrValues);

			InitEvents();
			emdrDisplayWindow.Show();
			emdrDisplayWindow.Activate();
		}

		private void btnEMDRSaveSettings_Click(object sender, RoutedEventArgs e)
        {
			EMDRAppModel.Instance.SaveEMDRValues(emdrValues);
		}

		private void btnEMDRClose_Click(object sender, RoutedEventArgs e)
        {
			EMDRValuesPopup.IsOpen = false;
			e.Handled = true;
		}

		#endregion

		#region Settings

		private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsInitialized && emdrLedsBarControl != null)
			{
				emdrValues.Speed = (sender as Slider).Value;

				UpdateTimerValues();
			}

			e.Handled = true;
		}

		private void UpdateTimerValues()
		{
			var mSecs = SetSpeedTimeouts();
			emdrDisplayWindow.InitEMDRLedsTimerMSecs(mSecs);
		}

		private double SetSpeedTimeouts()
		{
			//double LeftToRightMSecs = (emdrValues.SlowestMsecs * 10) / 
			//			(emdrValues.Speed * emdrValues.SpeedMultiplier);

			//double mSecs = LeftToRightMSecs / emdrValues.NumberOfLeds;

			double LeftToRightMSecsPercent = (emdrValues.Speed / 100);
			double LeftToRightMSecs = emdrValues.SlowestMsecs * (1 - LeftToRightMSecsPercent);

			//double mSecsPerLED = 10;
			//double mSecsPerLED = LeftToRightMSecs / emdrValues.NumberOfLeds;

			double timerMSecs = (emdrValues.SpeedCoefficient - emdrValues.Speed * emdrValues.SpeedMultiplier);
			emdrValues.FadeinMSecs = timerMSecs * .5;
			emdrValues.FadeoutMSecs = 2;

			if (emdrLedsBarControl != null)
				emdrLedsBarControl.InitAnimations();

			return timerMSecs;
		}

		private void SpeedSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{

		}

		private void ScalingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsInitialized && emdrLedsBarControl != null)
			{
				emdrValues.Scale = (sender as Slider).Value;

				UpdateScale(emdrValues.Scale);
			}

			e.Handled = true;
		}

		private void UpdateScale(double ScaleValue)
		{
			emdrDisplayWindow.ScaleWindow(ScaleValue);

			foreach (var control in emdrLedsBarControl.EMDRLedControls)
				control.ScaleControl(ScaleValue);
		}

		private void DotSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsInitialized && emdrLedsBarControl != null)
			{
				emdrValues.DotSize = (sender as Slider).Value;

				foreach (var control in emdrLedsBarControl.EMDRLedControls)
					control.ScaleDotSize(emdrValues.DotSize);
			}

			e.Handled = true;
		}

		private void OnLEDColorMouseDown(object sender, MouseButtonEventArgs e)
        {
			ColorPicker.Visibility = ColorPicker.IsVisible ? Visibility.Collapsed : Visibility.Visible;

			e.Handled = true;
		}
		private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			if (ColorPicker.SelectedColor.HasValue)
            {
				Color color = ColorPicker.SelectedColor.Value;
				OnColorSelectionChangedEvent?.Invoke(this, ColorPicker.SelectedColor.Value);

				emdrValues.LEDColor = UtilityFunctions.GetColorName(color);

				if (emdrDisplayWindow != null)
				{
					//emdrDisplayWindow.InitEMDRLedControls(EMDRLedControls);
				}

			}
			e.Handled = true;
		}

        private void ColorPicker_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
			ColorPicker.Visibility = Visibility.Collapsed;
			e.Handled = true;
		}

		#endregion

	}
}
