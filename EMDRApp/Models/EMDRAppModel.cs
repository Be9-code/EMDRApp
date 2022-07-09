using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.XPath;
using AppCore.Helpers;
using XMLXSL;
using AppCore.Models;
using EMDRApp.Controls;
using EMDRApp.Controllers;

namespace EMDRApp.Models
{
	public class EMDRAppModel : BaseModel
	{
		#region Variables

		public string EMDRXmlFile;

		#endregion

		#region Properties

		public static EMDRAppModel Instance
		{
			get { return _Instance ??= new EMDRAppModel(); }
			set { _Instance = value; }
		}
		static EMDRAppModel _Instance = null;

		public XmlNode EMDRAppXmlDocRoot
		{
			get { return _EMDRAppXmlDocRoot ??= InitEMDRAppXML(); }
			set { _EMDRAppXmlDocRoot = value; }
		}
		XmlNode _EMDRAppXmlDocRoot = null;

		public XMLXSLBase EMDRAppXmlBase
		{
			get { return _EMDRAppXmlBase ??= new XMLXSLBase(); }
			set { _EMDRAppXmlBase = value; }
		}
		public XMLXSLBase _EMDRAppXmlBase = null;

		#endregion

		#region Events

		#endregion

		public EMDRAppModel()
		{
			Instance = this;
			EMDRXmlFile = AppConfig.Instance.AdjustPathForXmlRoot("EMDR.xml");
		}

		public void InitEMDRValues(EMDRValues emdrValues)
		{
			string XPath = "//Settings/LEDs";
			XmlElement xmlNode = GetEMDRXMLNode(XPath);

			emdrValues.Speed = BaseModel.GetXmlNodeDoubleAttribute(xmlNode, "Speed");
			emdrValues.SpeedCoefficient = BaseModel.GetXmlNodeDoubleAttribute(xmlNode, "SpeedCoefficient");
			emdrValues.SpeedMultiplier = BaseModel.GetXmlNodeDoubleAttribute(xmlNode, "SpeedMultiplier");
			emdrValues.SpeedIncrement = BaseModel.GetXmlNodeDoubleAttribute(xmlNode, "SpeedIncrement");
			emdrValues.SlowestMsecs = BaseModel.GetXmlNodeDoubleAttribute(xmlNode, "SlowestMsecs");
			emdrValues.NumberOfLeds = BaseModel.GetXmlNodeIntAttribute(xmlNode, "NumberOfLeds");
			emdrValues.NumberOfLedControls = BaseModel.GetXmlNodeIntAttribute(xmlNode, "NumberOfLedControls");
			emdrValues.NumberPerGroup = BaseModel.GetXmlNodeIntAttribute(xmlNode, "NumberPerGroup");
			emdrValues.FadeinMSecs = BaseModel.GetXmlNodeDoubleAttribute(xmlNode, "FadeinMSecs");
			emdrValues.FadeoutMSecs = BaseModel.GetXmlNodeDoubleAttribute(xmlNode, "FadeoutMSecs");
			emdrValues.Scale = BaseModel.GetXmlNodeDoubleAttribute(xmlNode, "Scale");
			emdrValues.DotSize = BaseModel.GetXmlNodeDoubleAttribute(xmlNode, "DotSize");
			emdrValues.LEDColor = BaseModel.GetXmlNodeAttribute(xmlNode, "LEDColor");

		}
		public void SaveEMDRValues(EMDRValues emdrValues)
		{
			string XPath = "//Settings/LEDs";
			XmlElement xmlNode = GetEMDRXMLNode(XPath);

			BaseModel.SetXmlNodeDoubleAttribute(xmlNode, "Speed", emdrValues.Speed);
			BaseModel.SetXmlNodeDoubleAttribute(xmlNode, "SpeedCoefficient", emdrValues.SpeedCoefficient);
			BaseModel.SetXmlNodeDoubleAttribute(xmlNode, "SpeedMultiplier", emdrValues.SpeedMultiplier);
			BaseModel.SetXmlNodeDoubleAttribute(xmlNode, "SpeedIncrement", emdrValues.SpeedIncrement);
			BaseModel.SetXmlNodeDoubleAttribute(xmlNode, "SlowestMsecs", emdrValues.SlowestMsecs);
			BaseModel.SetXmlNodeIntAttribute(xmlNode, "NumberOfLeds", emdrValues.NumberOfLeds);
			BaseModel.SetXmlNodeIntAttribute(xmlNode, "NumberOfLedControls", emdrValues.NumberOfLedControls);
			BaseModel.SetXmlNodeIntAttribute(xmlNode, "NumberPerGroup", emdrValues.NumberPerGroup);
			BaseModel.SetXmlNodeDoubleAttribute(xmlNode, "FadeinMSecs", emdrValues.FadeinMSecs);
			BaseModel.SetXmlNodeDoubleAttribute(xmlNode, "FadeoutMSecs", emdrValues.FadeoutMSecs);
			BaseModel.SetXmlNodeDoubleAttribute(xmlNode, "Scale", emdrValues.Scale);
			BaseModel.SetXmlNodeDoubleAttribute(xmlNode, "DotSize", emdrValues.DotSize);
			BaseModel.SetXmlNodeAttribute(xmlNode, "LEDColor", emdrValues.LEDColor);

			SaveEMDRValuesXML();
		}

		internal void SetDefaultSettings()
		{
			string XPath = "//Settings[not(@Defaults='true')]";
			XmlElement xmlSettingsNode = GetEMDRXMLNode(XPath);

			if (xmlSettingsNode != null)
				EMDRAppXmlDocRoot.RemoveChild(xmlSettingsNode);

			XPath = "//Settings[@Defaults='true']";
			XmlElement xmlNode = GetEMDRXMLNode(XPath);
			if (xmlNode != null)
            {
				XmlElement xmlNewNode = (XmlElement)xmlNode.CloneNode(true);
				xmlNewNode.RemoveAttribute("Defaults");
				EMDRAppXmlDocRoot.PrependChild(xmlNewNode);
			}

			SaveEMDRValuesXML();
		}

		private void SaveEMDRValuesXML()
		{
			EMDRAppXmlBase.SaveXMLToFile(EMDRXmlFile, true, true);
		}

		#region Initialization

		public XmlNode InitEMDRAppXML()
		{
			EMDRAppXmlBase.InitXMLFromFile(EMDRXmlFile);
			_EMDRAppXmlDocRoot = _EMDRAppXmlBase.DocRoot;
			return _EMDRAppXmlDocRoot;
		}

		#endregion

		public XmlElement GetWindowSettingsXMLNode(string Name, string WindowName = "",
													string UIObjName = "", string UIChildObjName = "")
		{
			string MachineName = AppConfig.MachineName;
			string XPath = $"//WindowSettings[@Name='{Name}' and @MachineName='{MachineName}'" +
								$"and EnabledTest]";

			XPath = XMLXSLBase.InitEnabledTest(XPath);

			if (!string.IsNullOrEmpty(WindowName))
				XPath += $"/Settings[@WindowName='{WindowName}' or @Name='{WindowName}']";

			if (!string.IsNullOrEmpty(UIObjName))
				XPath += $"/UIObj[@Name='{UIObjName}']";

			if (!string.IsNullOrEmpty(UIChildObjName))
				XPath += $"/UIChildObj[@Name='{UIChildObjName}']";

			XmlElement xmlNode = (XmlElement)EMDRAppXmlDocRoot.SelectSingleNode(XPath);
			return xmlNode;
		}

		public string GetEMDRXmlAttribute(string XPath, string AttributeName, bool Remove = false)
		{
			string AttributeValue = GetEMDRXmlAttribute(XPath, AttributeName);

			if (Remove)
			{
				XmlElement xmlNode = GetEMDRXMLNode(XPath);
				if (xmlNode != null)
					xmlNode.RemoveAttribute(AttributeName);
			}

			return AttributeValue;
		}

		public string GetEMDRXmlAttribute(string XPath, string AttributeName)
		{
			string AttributeValue = "";
			XmlElement xmlNode = GetEMDRXMLNode(XPath);
			if (xmlNode != null)
			{
				AttributeValue = xmlNode.GetAttribute(AttributeName);
			}
			return AttributeValue;
		}

		public XmlElement GetEMDRXMLNode(string XPath)
		{
			XmlElement xmlNode = (XmlElement)EMDRAppXmlDocRoot.SelectSingleNode(XPath);
			return xmlNode;
		}

		public XmlNodeList GetEMDRXMLNodes(string XPath)
		{
			XmlNodeList xmlNodeList = EMDRAppXmlDocRoot.SelectNodes(XPath);
			return xmlNodeList;
		}

    }

	#region Support classes

	public class EMDRValues : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public EMDRValues()
		{
		}

		public int NumberOfLedControls
		{
			get { return _NumberOfLedControls; }
			set 
			{ 
				_NumberOfLedControls = value;
				OnPropertyChanged(nameof(NumberOfLedControls)); 
			}
		}
		public int _NumberOfLedControls;

		public int NumberOfLeds
		{
			get { return _NumberOfLeds; }
			set { _NumberOfLeds = value; OnPropertyChanged(nameof(NumberOfLeds)); }
		}
		public int _NumberOfLeds;

		public double FadeinMSecs { get; set; }
		public double FadeoutMSecs { get; set; }
		public int NumberPerGroup { get; set; }
		public double Speed { get; set; }
		public double SpeedCoefficient { get; set; }
		public double SpeedMultiplier { get; set; }
		public double SpeedIncrement { get; set; }
		public double SlowestMsecs { get; set; }

		public string LEDColor
		{
			get { return _LEDColor; }
			set { _LEDColor = value; OnPropertyChanged(nameof(LEDColor)); }
		}
		public string _LEDColor;

		public double Scale
		{
			get { return _Scale; }
			set { _Scale = value; OnPropertyChanged(nameof(Scale)); }
		}
		public double _Scale;

		public double DotSize
		{
			get { return _DotSize; }
			set { _DotSize = value; OnPropertyChanged(nameof(DotSize)); }
		}
		public double _DotSize;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

#endregion

#region XPath strings

public static class EMDRXPath
	{
		public const string EMDR = "//EMDR";
	}

	#endregion

}
