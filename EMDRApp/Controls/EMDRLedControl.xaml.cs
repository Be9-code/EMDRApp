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

namespace EMDRApp.Controls
{
	public partial class EMDRLedControl : UserControl, INotifyPropertyChanged
	{
		#region Variables

		public EMDRValues emdrValues;
		public FrameworkElement TargetControl;

		public DoubleAnimation FadeinAnimation;
		public DoubleAnimation FadeoutAnimation;

		public double BaseHeight;
		public double BaseWidth;
		public double BaseDotHeight;
		public double BaseDotWidth;
		public double BaseEMDRDotBorder1Height;
		public double BaseEMDRDotBorder1Width;
		public double BaseEMDRDotBorder2Height;
		public double BaseEMDRDotBorder2Width;

		#endregion

		#region Properties

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public EMDRLedControl(EMDRValues emdrValues)
			: base()
		{
			InitializeComponent();
			InitControl(emdrValues);
		}

		public EMDRLedControl()
			: base()
		{
			InitializeComponent();
		}

		#region Initialization

		public void InitControl(EMDRValues emdrValues)
		{
			this.emdrValues = emdrValues;
			DataContext = emdrValues;

			BaseHeight = this.Height;
			BaseWidth = this.Width;
			BaseDotHeight = EMDRDot.Height;
			BaseDotWidth = EMDRDot.Width;
			BaseEMDRDotBorder1Height = EMDRDotBorder1.Height;
			BaseEMDRDotBorder1Width = EMDRDotBorder1.Width;
			BaseEMDRDotBorder2Height = EMDRDotBorder2.Height;
			BaseEMDRDotBorder2Width = EMDRDotBorder2.Width;

			TargetControl = EMDRDotGrid;

			FadeinAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(emdrValues.FadeinMSecs));
			FadeoutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(emdrValues.FadeoutMSecs));

			FadeinAnimation.Completed += On_FadeInAnimationCompleted;
			FadeoutAnimation.Completed += On_FadeOutAnimationCompleted;
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

		// https://stackoverflow.com/questions/69205632/how-to-imitate-outerglowbitmapeffect-using-wpf-effects
		internal void RunEMDRDotAnimation(bool IsRun = true)
        {
			TargetControl.BeginAnimation(UIElement.OpacityProperty, FadeinAnimation);
        }

        public void InitAnimationsDuration()
        {
			if (emdrValues.FadeinMSecs > 0 )
				FadeinAnimation.Duration = TimeSpan.FromMilliseconds(emdrValues.FadeinMSecs);
			if (emdrValues.FadeoutMSecs > 0 )
				FadeoutAnimation.Duration = TimeSpan.FromMilliseconds(emdrValues.FadeoutMSecs);
        }

        private void On_FadeInAnimationCompleted(object sender, EventArgs e)
        {
            //TargetControl.Opacity = 0;
            TargetControl.BeginAnimation(UIElement.OpacityProperty, FadeoutAnimation);
		}

        private void On_FadeOutAnimationCompleted(object sender, EventArgs e)
        {
			//TargetControl.Opacity = 0;
			//TargetControl.Visibility = Visibility.Hidden;
		}

		internal void FadeEMDRDot(bool IsFade = true)
		{
			TargetControl.BeginAnimation(UIElement.OpacityProperty, IsFade ? FadeoutAnimation : FadeinAnimation);
		}

		internal void ScaleControl(double ScaleValue)
		{
			this.Height = BaseHeight * ScaleValue;
			this.Width = BaseHeight * ScaleValue;
		}

		internal void ScaleDotSize(double ScaleValue)
		{
			EMDRDot.Height = BaseDotHeight * ScaleValue;
			EMDRDot.Width = BaseDotWidth * ScaleValue;

			EMDRDotBorder1.Height = BaseEMDRDotBorder1Height * ScaleValue;
			EMDRDotBorder1.Width = BaseEMDRDotBorder1Width * ScaleValue;
			EMDRDotBorder2.Height = BaseEMDRDotBorder2Height * ScaleValue;
			EMDRDotBorder2.Width = BaseEMDRDotBorder2Width * ScaleValue;
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
