using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("HeadBodyPartPrefabs", "MainBodyPartPrefabs", "LegBodyPartPrefabs", "SpecialBodyPartPrefabs")]
	public class ES3UserType_AlienBodyPartCollections : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AlienBodyPartCollections() : base(typeof(AlienBodyPartCollections)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (AlienBodyPartCollections)obj;
			
			writer.WriteProperty("HeadBodyPartPrefabs", instance.HeadBodyPartPrefabs, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(BodyPartCollection)));
			writer.WriteProperty("MainBodyPartPrefabs", instance.MainBodyPartPrefabs, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(BodyPartCollection)));
			writer.WriteProperty("LegBodyPartPrefabs", instance.LegBodyPartPrefabs, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(BodyPartCollection)));
			writer.WriteProperty("SpecialBodyPartPrefabs", instance.SpecialBodyPartPrefabs, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(BodyPartCollection)));
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (AlienBodyPartCollections)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "HeadBodyPartPrefabs":
						instance.HeadBodyPartPrefabs = reader.Read<BodyPartCollection>();
						break;
					case "MainBodyPartPrefabs":
						instance.MainBodyPartPrefabs = reader.Read<BodyPartCollection>();
						break;
					case "LegBodyPartPrefabs":
						instance.LegBodyPartPrefabs = reader.Read<BodyPartCollection>();
						break;
					case "SpecialBodyPartPrefabs":
						instance.SpecialBodyPartPrefabs = reader.Read<BodyPartCollection>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_AlienBodyPartCollectionsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AlienBodyPartCollectionsArray() : base(typeof(AlienBodyPartCollections[]), ES3UserType_AlienBodyPartCollections.Instance)
		{
			Instance = this;
		}
	}
}