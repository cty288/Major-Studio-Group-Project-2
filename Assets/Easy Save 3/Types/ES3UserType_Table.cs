using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("newspaperPrefab", "bountyHunterGiftPrefab", "photoPrefabList")]
	public class ES3UserType_Table : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Table() : base(typeof(Table)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Table)obj;
			
			writer.WritePrivateFieldByRef("newspaperPrefab", instance);
			writer.WritePrivateFieldByRef("bountyHunterGiftPrefab", instance);
			writer.WritePrivateField("photoPrefabList", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Table)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "newspaperPrefab":
					instance = (Table)reader.SetPrivateField("newspaperPrefab", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "bountyHunterGiftPrefab":
					instance = (Table)reader.SetPrivateField("bountyHunterGiftPrefab", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "photoPrefabList":
					instance = (Table)reader.SetPrivateField("photoPrefabList", reader.Read<System.Collections.Generic.List<UnityEngine.GameObject>>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_TableArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TableArray() : base(typeof(Table[]), ES3UserType_Table.Instance)
		{
			Instance = this;
		}
	}
}