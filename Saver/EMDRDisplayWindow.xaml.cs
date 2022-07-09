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
	public partial class EMDRDisplayWindow : WindowBase
	{
		#region Variables

		protected bool CloseOnEscKey = true;
		public bool AutoStart;

		#endregion

		#region Properties

		public EMDRController appController
		{
			get { return EMDRController.Instance; }
		}

		#endregion

		#region Events

		public event OnObjectParamDel OnEMDRDisplayWindowClosingEvent;

		#endregion

		public EMDRDisplayWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			EMDRController.Instance.emdrDisplayWindow = this;

			Dispatcher.BeginInvoke(new Action(() => OnWindow_FullyLoaded()),
									DispatcherPriority.ContextIdle, null);
		}

		public override void OnWindow_FullyLoaded()
        {
			InitEvents();
			base.OnWindow_FullyLoaded();
		}

		#region InitEvents/UnInitEvents

		public void InitEvents()
		{
			UnInitEvents();

			OnEMDRDisplayWindowClosingEvent += EMDRController.Instance.OnEMDRDisplayWindowClosing;
		}

		public void UnInitEvents()
		{
			OnEMDRDisplayWindowClosingEvent -= EMDRController.Instance.OnEMDRDisplayWindowClosing;
		}

		#endregion

		public void AddEMDRLedControls(List<EMDRLedControl> EMDRLedControls)
        {
			EMDRImagesGrid.Children.Clear();
            EMDRImagesGrid.ColumnDefinitions.Clear();

            int row = 0;
            for (int i = 0; i < EMDRLedControls.Count; i++)
            {
                var control = EMDRLedControls[i];
                EMDRImagesGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 40 });
				Grid.SetRow(control, row);
				Grid.SetColumn(control, i);

                if (EMDRImagesGrid.Children.Contains(control))
                    EMDRImagesGrid.Children.Remove(control);

                EMDRImagesGrid.Children.Add(control);

				//control.SetValue(Grid.RowProperty, row);
				//control.SetValue(Grid.ColumnProperty, i);
			}

            this.Width = (EMDRLedControls.Count * 40) + 20;
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
					if (CloseOnEscKey)
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
			e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			OnEMDRDisplayWindowClosingEvent?.Invoke(this);
		}
    }
}
