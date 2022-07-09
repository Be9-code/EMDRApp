using AppCore.Helpers;
using AppCore.Windows;
using EMDRApp.Controllers;
using EMDRApp.Controls;
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
using System.Windows.Threading;

namespace EMDRApp.Windows
{
	public partial class EMDRDisplayWindow : Window
	{
		#region Variables

		protected bool CloseOnEscKey = true;
		EMDRValues emdrValues;

		public SimpleMSecsTimer EMDRLedsTimer;
		public double EMDRLedsTimerMSecs;

		public double BaseHeight;
		public double BaseWidth;
		public double BaseImagesGridHeight;
		public double BaseImagesGridWidth;
		public double BaseBarHeight;
		public double BaseBarWidth;
		public double BaseEMDRLedControlsGridHeight;
		public double BaseEMDRLedControlsGridWidth;

		#endregion

		#region Properties

		public EMDRController appController
		{
			get { return EMDRController.Instance; }
		}

		#endregion

		#region Events

		public event OnObjectParamDel OnEMDRDisplayWindowClosingEvent;
		public event OnWindowFullyLoadedDel OnWindow_FullyLoadedEvent;

		#endregion

		public EMDRDisplayWindow(EMDRValues emdrValues)
		{
			InitializeComponent();
			this.emdrValues = emdrValues;
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			EMDRController.Instance.emdrDisplayWindow = this;

			Dispatcher.BeginInvoke(new Action(() => OnWindow_FullyLoaded()),
									DispatcherPriority.ContextIdle, null);
		}

		private void OnWindow_FullyLoaded()
        {
            InitEvents();

            InitEMDRLedsBarControl();
            OnWindow_FullyLoadedEvent?.Invoke(null);

            AppCoreUtil.ExecAfterDelay(500, CaptureHeightWidthValues);
        }

        private void CaptureHeightWidthValues()
        {
            BaseHeight = this.Height;
            BaseWidth = this.Width;
            BaseImagesGridHeight = EMDRImagesGrid.ActualHeight;
            BaseImagesGridWidth = EMDRImagesGrid.ActualWidth;
            BaseBarHeight = EMDRLedsBar.ActualHeight;
            BaseBarWidth = EMDRLedsBar.ActualWidth;
            BaseEMDRLedControlsGridHeight = EMDRLedsBar.EMDRLedControlsGrid.ActualHeight;
            BaseEMDRLedControlsGridWidth = EMDRLedsBar.EMDRLedControlsGrid.ActualWidth;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			KillEMDRLedsTimer();
			OnEMDRDisplayWindowClosingEvent?.Invoke(this);
		}

		#region InitEvents/UnInitEvents

		public void InitEvents()
		{
			UnInitEvents();
			EMDRLedsBar.OnClearEMDRLedControlsEvent += OnClearEMDRLedControls;
			EMDRLedsBar.OnInitEMDRLedControlsEvent += OnInitEMDRLedControls;

			OnEMDRDisplayWindowClosingEvent += EMDRController.Instance.OnEMDRDisplayWindowClosing;
		}

        public void UnInitEvents()
		{
			EMDRLedsBar.OnClearEMDRLedControlsEvent -= OnClearEMDRLedControls;
			EMDRLedsBar.OnInitEMDRLedControlsEvent -= OnInitEMDRLedControls;
			OnEMDRDisplayWindowClosingEvent -= EMDRController.Instance.OnEMDRDisplayWindowClosing;
		}

		#endregion

		public void StartEMDRDisplay( double mSecs )
		{
			EMDRLedsBar.EMDRLedsOffset = 0;

			EMDRLedsTimerMSecs = mSecs;
			EMDRLedsTimer = new SimpleMSecsTimer(EMDRLedsTimerMSecs, EMDRLedsBar.OnEMDRLedsTimerTimeout);
		}

		public void InitEMDRLedsTimerMSecs(double mSecs)
		{
			if (EMDRLedsTimer != null && mSecs > 0)
				EMDRLedsTimer.Interval = TimeSpan.FromMilliseconds(mSecs);
		}

		public void KillEMDRLedsTimer()
		{
			if (EMDRLedsTimer != null)
			{
				EMDRLedsTimer.StopGenericTimer();
				EMDRLedsTimer = null;
			}
		}

		private void OnClearEMDRLedControls(object obj)
		{
			if ((obj as EMDRValues) == emdrValues )
            {
    //            if (EMDRImagesGrid.Children.Contains(emdrLedsBarControl))
    //                EMDRImagesGrid.Children.Remove(emdrLedsBarControl);

    //            EMDRImagesGrid.Children.Clear();
				//emdrLedsBarControl = null;
			}
		}

		private void OnInitEMDRLedControls(object obj)
		{
			if ((obj as EMDRValues) == emdrValues )
            {
                InitEMDRLedsBarControl();

                //InitEMDRLedControlsUI(emdrValues.EMDRLedControls);
            }
        }

        private void InitEMDRLedsBarControl()
        {
            if (emdrValues != null)
            {
                //if (EMDRImagesGrid.Children.Contains(emdrLedsBarControl))
                //    EMDRImagesGrid.Children.Remove(emdrLedsBarControl);

                //emdrLedsBarControl = new EMDRLedsBarControl(emdrValues);
                EMDRLedsBar.InitControl(emdrValues);

                //EMDRImagesGrid.Children.Add(emdrLedsBarControl);
            }

            SetWidth();
        }

		public void SetWidth()
        {
            this.Width = (EMDRLedsBar.EMDRLedControls.Count * 40) + 20;
        }

        //public void InitEMDRLedControlsUI(List<EMDRLedControl> EMDRLedControls)
        //      {
        //	EMDRImagesGrid.Children.Clear();
        //          EMDRImagesGrid.ColumnDefinitions.Clear();

        //	int row = 0;
        //          for (int i = 0; i < EMDRLedControls.Count; i++)
        //          {
        //              var control = EMDRLedControls[i];
        //              EMDRImagesGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 40 });
        //		Grid.SetRow(control, row);
        //		Grid.SetColumn(control, i);

        //              if (EMDRImagesGrid.Children.Contains(control))
        //                  EMDRImagesGrid.Children.Remove(control);

        //              try
        //              {
        //			EMDRImagesGrid.Children.Add(control);
        //		}
        //		catch (Exception) { }

        //		//control.SetValue(Grid.RowProperty, row);
        //		//control.SetValue(Grid.ColumnProperty, i);
        //	}

        //	this.Width = (EMDRLedControls.Count * 40) + 20;
        //      }


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
					if (CloseOnEscKey)
					{
						Close();
					}
					break;
				case Key.Enter:
					break;
				case Key.Space:
					EMDRLedsTimer.PauseGenericTimer(EMDRLedsTimer.IsEnabled);
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

		internal void ScaleWindow(double ScaleValue)
		{
			double gridScalar = 1;
			this.Height = BaseHeight * ScaleValue;
			this.Width = BaseWidth * ScaleValue;
			EMDRImagesGrid.Height = BaseImagesGridHeight * (ScaleValue * gridScalar);
			EMDRImagesGrid.Width = BaseImagesGridWidth * (ScaleValue * gridScalar);
			EMDRLedsBar.Height = BaseBarHeight * (ScaleValue * gridScalar);
			EMDRLedsBar.Width = BaseBarWidth * (ScaleValue * gridScalar);
			EMDRLedsBar.EMDRLedControlsGrid.Height = BaseEMDRLedControlsGridHeight * (ScaleValue * gridScalar);
			EMDRLedsBar.EMDRLedControlsGrid.Width = BaseEMDRLedControlsGridWidth * (ScaleValue * gridScalar);
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
			e.Handled = true;
        }
    }
}
