using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Xml;
using SimpleJSON;
namespace Language
{
	public class LanguageInfo : IEquatable<LanguageInfo>
	{
		public string Name;
		public static SubPubSystem SubPubSystem = new SubPubSystem();
		public LanguageInfo(){}

		public LanguageInfo(string name)
		{
			Name = name;
		}

		public static readonly LanguageInfo English = new LanguageInfo("English");

		public bool Equals(LanguageInfo other)
		{
			return other.Name == Name;
		}
	}

	[ExecuteInEditMode]
	public class LanguageService {

		private static LanguageService _instance;
		public static LanguageService Instance
		{
			get { return _instance ?? (_instance = new LanguageService()); }
		}
		public List<string> Files { get; set; }
		public Dictionary<string, Dictionary<string, string>> StringsByFile { get; set; }
		public Dictionary<string, string> Strings { get; set; }

		public List<LanguageInfo> Languages = new List<LanguageInfo>
		{
			LanguageInfo.English,
		};

		public List<String> LanguageNames = new List<String> ();

		private LanguageInfo _language = new LanguageInfo {Name = "English" };
		public LanguageInfo Language
		{
			get { return _language; }
			set
			{
				if (!HasLanguage(value))
				{
					Debug.LogError("Invalid Language " + value);
				}

				_language = value;
				ReadLanguageFiles();
				LanguageInfo.SubPubSystem.Publish("Language");
			}
		}
		bool HasLanguage(LanguageInfo language)
		{
			foreach (var systemLanguage in Languages)
			{
				if (systemLanguage.Equals(language))
					return true;
			}
			return false;
		}

		public LanguageService()
		{
			LoadContent();
		}

		public void LoadContent()
		{		
			TextAsset config = Resources.Load<TextAsset>("ConfigFile/LocalizationConfig");
			var jsonArray = JSONNode.Parse (config.text);
			LoadAllLanguages ((JSONClass)jsonArray);
		}

		void LoadAllLanguages(JSONClass jsonClass){
			var d = LanguageInfo.English;
			foreach(KeyValuePair<string, JSONNode> json in jsonClass)
			{
				var language = new LanguageInfo (json.Value);
				LanguageNames.Add (language.Name);
				Languages.Add(language);
				if (json.Key == "Default"){
					d = language;
				}					
			}
			Language = d;
		}

		public string GetFromFile(string groupId, string key, string fallback)
		{
			if (!StringsByFile.ContainsKey(groupId))
			{
				Debug.LogWarning("Localization File Not Found : " + groupId);
				return fallback;
			}
			var group = StringsByFile[groupId];
			if (!group.ContainsKey(key))
			{
				Debug.LogWarning("Localization Key Not Found : " + key);
				return fallback;
			}
			return group[key];
		}

		// 读取Josn文件
		void ReadLanguageFiles()
		{
			Strings = new Dictionary<string, string>();
			StringsByFile = new Dictionary<string, Dictionary<string, string>>();
			Files = new List<string>();

			string path = "Lang/" + Language.Name;
			TextAsset[] resources = Resources.LoadAll<TextAsset>(path);
			if (!resources.Any())
			{
				Debug.LogError("Localization Files Not Found : " + path);
			}
			foreach (TextAsset resource in resources)
			{
				ReadTextAsset(resource);
			}
		}

		// 将TextAsset内容读取到字典中
		void ReadTextAsset(TextAsset resource)
		{
			XmlDocument xml = new XmlDocument();
			XmlReaderSettings set = new XmlReaderSettings();
			set.IgnoreComments = true;//这个设置是忽略xml注释文档的影响。有时候注释会影响到xml的读取
			xml.Load(XmlReader.Create(new MemoryStream(resource.bytes),set));
			//得到objects节点下的所有子节点
			XmlNodeList xmlNodeList = xml.SelectSingleNode("contents").ChildNodes;
			xmlNodeList = xmlNodeList [0].ChildNodes;
			Files.Add(resource.name);
			StringsByFile.Add(resource.name, new Dictionary<string, string>());

			for (int i = 0; i < xmlNodeList.Count; i++) {
				string id = xmlNodeList[i].Attributes["id"].Value;
				string value = xmlNodeList[i].InnerText;


				if (Strings.ContainsKey (id)) {
					Debug.LogWarning("Duplicate string : " + resource + " : " + id);
					return;
				}
				else
					Strings.Add(id, value);
				StringsByFile[resource.name].Add(id, value);
			}
		}

		// 根据key获取相应的语言内容
		public string GetStringByKey(string key, string fallback)
		{
			if (!Strings.ContainsKey(key))
			{
				Debug.LogWarning(string.Format("Localization Key Not Found {0} : {1} ", Language.Name, key));
				return fallback;
			}
			return Strings[key]; 
		}

		public void UpdateText(GameObject textObj, string key, params string[] value)
		{
			Text label = textObj.transform.GetComponent<Text>();
			string newText = GetStringByKey (key, label.text);
			if (value.Length > 0){
				for(int i = 0; i < value.Length; i++){
					newText = string.Format (newText, value[i]);
				}
			}
			label.text = newText;
		}	

		public static void GetDirectorys(string strPath, ref List<string> lstDirect)  
		{  
			DirectoryInfo diFliles = new DirectoryInfo(strPath);  
			DirectoryInfo[] diArr = diFliles.GetDirectories();  
			//DirectorySecurity directorySecurity = null;  
			foreach (DirectoryInfo di in diArr)  
			{  
				try  
				{  
					//directorySecurity = new DirectorySecurity(di.FullName, AccessControlSections.Access);  
					//if (!directorySecurity.AreAccessRulesProtected)  
					//{  
					lstDirect.Add(di.FullName);  
					GetDirectorys(di.FullName, ref lstDirect);  
					//}  
				}  
				catch   
				{  
					continue;  
				}  
			}  
		}  
		// 遍历当前目录及子目录    
		// <param name="strPath">文件路径</param>  
		// <returns>所有文件</returns>  
		public static IList<FileInfo> GetFiles(string strPath)  
		{  
			List<FileInfo> lstFiles = new List<FileInfo>();  
			List<string> lstDirect = new List<string>();  
			lstDirect.Add(strPath);  
			DirectoryInfo diFliles = null;  
			GetDirectorys(strPath, ref lstDirect);  
			foreach (string str in lstDirect)  
			{  
				try  
				{  
					diFliles = new DirectoryInfo(str);  
					lstFiles.AddRange(diFliles.GetFiles());  
				}  
				catch   
				{  
					continue;  
				}  
			}  
			return lstFiles;  
		}  
	}
}
