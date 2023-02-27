using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("pageContentString", "first12Chars", "tableBounds", "canDrag")]
	public class ES3UserType_NotebookPage : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_NotebookPage() : base(typeof(NotebookPage)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (NotebookPage)obj;
			
			writer.WritePrivateField("pageContentString", instance);
			writer.WritePrivateField("first12Chars", instance);
			writer.WritePrivateField("tableBounds", instance);
			writer.WritePrivateField("canDrag", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (NotebookPage)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "pageContentString":
					instance = (NotebookPage)reader.SetPrivateField("pageContentString", reader.Read<System.String>(), instance);
					break;
					case "first12Chars":
					instance = (NotebookPage)reader.SetPrivateField("first12Chars", reader.Read<System.String>(), instance);
					break;
					case "tableBounds":
					instance = (NotebookPage)reader.SetPrivateField("tableBounds", reader.Read<UnityEngine.Bounds>(), instance);
					break;
					case "canDrag":
					instance = (NotebookPage)reader.SetPrivateField("canDrag", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_NotebookPageArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_NotebookPageArray() : base(typeof(NotebookPage[]), ES3UserType_NotebookPage.Instance)
		{
			Instance = this;
		}
	}
}