using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EMDR.Controls
{
	public partial class ControlsDictionary : ResourceDictionary
	{
		#region Properties

		public static ControlsDictionary Instance
		{
			get { return _Instance ??= new ControlsDictionary(); }
			set { _Instance = value; }
		}
		static ControlsDictionary _Instance = null;


		#endregion

		public ControlsDictionary()
		{
			Instance = this;
			InitializeComponent();
		}
	}
}
