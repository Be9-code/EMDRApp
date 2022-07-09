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

		public List<EMDRLedControl> EMDRLeds = new List<EMDRLedControl>();
		public List<EMDRLedControl> LastVisibleEMDRLedControls = new List<EMDRLedControl>();

		public SimpleGenericTimer EMDRLedsTimer;
		public int EMDRLedsOffset;

		protected bool CloseOnEscKey;
		protected bool ExitOnEscKey = true;
		
		bool AutoStart;
		bool IsLeftToRight = true;


		public EMDRDisplayWindow emdrDisplayWindow;

		public List<EMDRLedControl> EMDRLedControls = new List<EMDRLedControl>();

		#endregion

		#region Properties

		public EMDRController emdrController
		{
			get { return EMDRController.Instance; }
		}

		public EMDRSettings emdrSettings { get; set; }

		#endregion

		#region Events

		public event OnIntParamDel OnNumberOfLedsChangedEvent;
		public event OnColorSelectionChangedDel OnColorSelectionChangedEvent;

		#endregion

		public EMDRWindow()
		{
			InitializeComponent();

			emdrSettings = new EMDRSettings();
			DataContext = emdrSettings;
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			EMDRAppModel.Instance.InitEMDRSettings(emdrSettings);

			InitEMDRLedControls(emdrSettings);

			EMDRDot.DataContext = emdrSettings = emdrSettings;

			SpeedSlider.Value = emdrSettings.Speed;

			InitEvents();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (ExitOnEscKey)
			{
				Application.Current.Shutdown();
			}
		}

		#region Key down handlers

		private void OnWindow_KeyDown(object sender, KeyEventArgs e)
		{
			Window target = (Window)sender;

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

			OnNumberOfLedsChangedEvent += EMDRController.Instance.OnNumberOfLedsChanged;
			OnColorSelectionChangedEvent += EMDRController.Instance.OnColorSelectionChanged;
		}

		public void UnInitEvents()
		{
			OnNumberOfLedsChangedEvent -= EMDRController.Instance.OnNumberOfLedsChanged;
			OnColorSelectionChangedEvent -= EMDRController.Instance.OnColorSelectionChanged;
		}

		#endregion

		internal List<EMDRLedControl> InitEMDRLedControls(EMDRSettings emdrSettings)
		{
			EMDRLedControls.Clear();

			for (int i = 0; i < emdrSettings.NumberOfLeds; i++)
			{
				EMDRLedControls.Add(new EMDRLedControl()
				{ DataContext = emdrSettings, emdrSettings = emdrSettings });
			}
			return EMDRLedControls;

		}

		private void btnEMDRPlay_Click(object sender, RoutedEventArgs e)
        {
			if (emdrDisplayWindow is null)
			{
				AutoStart = true;
				OpenEMDRDisplayWindow();
			}
			else
			{
				StartEMDRDisplay();
			}

			e.Handled = true;
		}

		private void StartEMDRDisplay()
		{
			foreach (var control in EMDRLedControls)
				control.Visibility = Visibility.Hidden;

			EMDRLedsTimer = new SimpleGenericTimer(emdrSettings.Speed * 10, OnEMDRLedsTimerTimeout);

			EMDRLedsOffset = 0;
			AutoStart = false;
		}

		private void btnEMDRPause_Click(object sender, RoutedEventArgs e)
        {
			EMDRLedsTimer.PauseGenericTimer(EMDRLedsTimer.IsEnabled);
			e.Handled = true;
		}

		private void btnEMDRStop_Click(object sender, RoutedEventArgs e)
        {
			EMDRLedsTimer.StopGenericTimer();
			e.Handled = true;
		}

		#region TimerTimeout

		private void OnEMDRLedsTimerTimeout()
		{
			foreach (var control in LastVisibleEMDRLedControls)
				control.Visibility = Visibility.Hidden;

			int Range = emdrSettings.NumberPerGroup;

			if (EMDRLedsOffset + Range > EMDRLedControls.Count - 1)
			{
				EMDRLedsOffset = (EMDRLedControls.Count - 1) - Range;
				IsLeftToRight = false;
			}
			else if (EMDRLedsOffset - Range < 0)
            {
				EMDRLedsOffset = 0;
				IsLeftToRight = true;
            }

			LastVisibleEMDRLedControls =
				EMDRLedControls.GetRange(EMDRLedsOffset, Range);

			foreach (var control in LastVisibleEMDRLedControls)
				control.Visibility = Visibility.Visible;

			EMDRLedsOffset += IsLeftToRight ? Range : -Range;
		}

		private void OnEMDRLedsTimerTimeoutX()
		{
			// allow last one to complete fade out
			for (int i = 0; i < EMDRLedsOffset - 2; i++)
			{
				var control = EMDRLedControls[i];
				control.Visibility = Visibility.Hidden;
			}

			//LastVisibleEMDRLedControls.Clear();

			for (int i = 0; i < emdrSettings.NumberPerGroup; i++)
			{
				var control = EMDRLedControls[EMDRLedsOffset++];
				if (EMDRLedsOffset == EMDRLedControls.Count)
				{
					EMDRLedControls.Reverse();
					EMDRLedsOffset = 0;
				}
				if (i < emdrSettings.NumberPerGroup - 1)
					control.Visibility = Visibility.Visible;
				else
					control.RunEMDRDotAnimation();
			}

			//EMDRLedsTimer.PauseGenericTimer(true);
		}

		#endregion

		private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
			if (IsInitialized)
            {
				emdrSettings.Speed = (int)SpeedSlider.Value;

				if (EMDRLedsTimer != null)
					EMDRLedsTimer.InitMSecsTimer(emdrSettings.Speed * emdrSettings.SpeedMultiplier);
			}

			e.Handled = true;
		}

		private void SpeedSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

		private void btnEMDRShow_Click(object sender, RoutedEventArgs e)
        {
			if (emdrDisplayWindow is null)
			{
				OpenEMDRDisplayWindow();
			}
            else
            {
				emdrDisplayWindow.Close();
			}
		}

		private void OpenEMDRDisplayWindow()
		{
			if (emdrDisplayWindow is null)
			{
				InitEMDRLedControls(emdrSettings);

				emdrDisplayWindow = new EMDRDisplayWindow();
				emdrDisplayWindow.Show();

				emdrDisplayWindow.InitEMDRLedControlsUI(EMDRLedControls);

				emdrDisplayWindow.Closed += (sender, e) => { emdrDisplayWindow = null; Focus(); };

				emdrDisplayWindow.OnWindow_FullyLoadedEvent += (window) => { OnEMDRDisplayWindowReady(); };
				emdrDisplayWindow.Closed += (sender, e) => { emdrDisplayWindow = null; Focus(); };
			}
			else
			{
				emdrDisplayWindow.Close();
			}
		}

		private void OnEMDRDisplayWindowReady()
		{
			if (AutoStart)
				StartEMDRDisplay();
		}

		private void btnEMDRSettings_Click(object sender, RoutedEventArgs e)
        {
			EMDRSettingsPopup.IsOpen = !EMDRSettingsPopup.IsOpen;
			e.Handled = true;
        }

		#region EMDRSettingsPopup

		private void EMDRSettingsPopup_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void EMDRSettingsPopup_Closed(object sender, EventArgs e)
        {

        }

        private void EMDRSettingsPopup_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
			EMDRSettingsPopup.IsOpen = false;
			e.Handled = true;
		}

		private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
			if (IsInitialized)
				emdrSettings.Scale = SpeedSlider.Value;
			e.Handled = true;
		}

		private void NumberOfLeds_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			TextBlock TextBlock = sender as TextBlock;
			emdrSettings.NumberOfLeds = ControlUtilityFunctions.HandleMouseWheelIntIncrement(TextBlock, e, 1);

			InitEMDRLedControls(emdrSettings);

			OnNumberOfLedsChangedEvent?.Invoke(emdrSettings.NumberOfLeds);

			if (emdrDisplayWindow != null)
			{
				emdrDisplayWindow.InitEMDRLedControlsUI(EMDRLedControls);
			}

			e.Handled = true;
		}

		private void btnEMDRSaveSettings_Click(object sender, RoutedEventArgs e)
        {
			EMDRAppModel.Instance.SaveEMDRSettings(emdrSettings);
		}

        #endregion

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

				emdrSettings.LEDColor = UtilityFunctions.GetColorName(color);

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
	}
}
