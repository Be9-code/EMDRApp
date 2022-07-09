using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AppCore.Models;
using XMLXSL;
using System.ComponentModel;
using System.Windows.Media.Animation;
using EMDRApp.Models;
using EMDRApp.Controllers;

namespace EMDRApp.Controls
{
	public partial class EMDRLedsBarControl : UserControl, INotifyPropertyChanged
	{
		#region Variables

		public EMDRValues emdrValues;

		public int EMDRLedsOffset;

		public List<EMDRLedControl> EMDRLedControls = new List<EMDRLedControl>();
		public List<EMDRLedControl> AllEMDRLedControls = new List<EMDRLedControl>();

		#endregion

		#region Properties

		#endregion

		#region Events

		public event OnObjectParamDel OnClearEMDRLedControlsEvent;
		public event OnObjectParamDel OnInitEMDRLedControlsEvent;
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public EMDRLedsBarControl(EMDRValues emdrValues)
			: base()
		{
			InitializeComponent();
			InitControl(emdrValues);
		}

		public EMDRLedsBarControl()
			: base()
		{
			InitializeComponent();
		}

		#region Initialization

		public void InitControl(EMDRValues emdrValues)
		{
			this.emdrValues = emdrValues;
			DataContext = emdrValues;

			InitAllEMDRLedControls(emdrValues);
			InitEMDRLedControls(EMDRLedControls);
		}

		#endregion

		#region InitEvents/UnInitEvents

		public void InitEvents()
		{
			UnInitEvents();
		}

		public void UnInitEvents()
		{
		}

		#endregion

		public void InitEMDRLedControls(List<EMDRLedControl> EMDRLedControls)
		{
			EMDRLedControlsGrid.Children.Clear();
			EMDRLedControlsGrid.ColumnDefinitions.Clear();

			int row = 0;
			for (int i = 0; i < EMDRLedControls.Count; i++)
			{
				var control = EMDRLedControls[i];
				EMDRLedControlsGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 40 });
                control.SetValue(Grid.RowProperty, row);
                control.SetValue(Grid.ColumnProperty, i);

                if (EMDRLedControlsGrid.Children.Contains(control))
					EMDRLedControlsGrid.Children.Remove(control);

				try
				{
					EMDRLedControlsGrid.Children.Add(control);
				}
				catch (Exception ex) 
				{ 
					string Message = ex.Message;
				}
			}
		}

		public void InitAllEMDRLedControls(EMDRValues emdrValues)
		{
			this.emdrValues = emdrValues;
			AllEMDRLedControls.Clear();

			for (int i = 0; i < emdrValues.NumberOfLedControls; i++)
			{
				AllEMDRLedControls.Add(new EMDRLedControl(emdrValues));
			}

			InitEMDRLedControls();
		}

		public List<EMDRLedControl> InitEMDRLedControls()
		{
			lock (EMDRLedControls)
			{
				FireClearEMDRLedControlsEvent();

				EMDRLedControls = AllEMDRLedControls.GetRange(0, emdrValues.NumberOfLeds);

				SetEMDRLedsOpacity(0);
			}

            OnInitEMDRLedControlsEvent?.Invoke(this);

            return EMDRLedControls;
		}

		public void FireClearEMDRLedControlsEvent()
		{
            OnClearEMDRLedControlsEvent?.Invoke(this);
        }

        bool IsLeftToRight = true;

		public void OnEMDRLedsTimerTimeout()
		{
			if (EMDRLedControls.Count <= 0)
				return;

			var control = EMDRLedControls[EMDRLedsOffset];
			//control.Visibility = Visibility.Visible;

			control.RunEMDRDotAnimation();

			if (EMDRLedsOffset == EMDRLedControls.Count - 1)
			{
				IsLeftToRight = false;
			}
			else if (EMDRLedsOffset is 0)
			{
				IsLeftToRight = true;
			}
			EMDRLedsOffset += IsLeftToRight ? 1 : -1;

			//EMDRLedsTimer.PauseGenericTimer(true);
		}

		public void FadeEMDRLeds(bool IsFade = true)
		{
			foreach (var control in EMDRLedControls)
				control.FadeEMDRDot(IsFade);
		}

		public void SetEMDRLedsOpacity(double Value)
		{
            foreach (var control in EMDRLedControls)
                control.TargetControl.Opacity = Value;
        }

		public void InitAnimations()
		{
			foreach (var control in EMDRLedControls)
				control.InitAnimationsDuration();
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
