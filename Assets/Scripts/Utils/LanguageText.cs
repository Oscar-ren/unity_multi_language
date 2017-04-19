using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Language
{
	[RequireComponent(typeof (Text))]
	[AddComponentMenu("Language/LanguageText")]

	public class LanguageText : MonoBehaviour {
		[HideInInspector] public string Language;
		[HideInInspector] public string File;
		[HideInInspector] public string Key;
		[HideInInspector] public string Value;

		public LanguageService Localization;
		// Use this for initialization

		void Start()
		{
			Localization = LanguageService.Instance;
			Language = Localization.Language.Name;
			File = Localization.StringsByFile.Select(o => o.Key).ToArray()[0];
			Text label = GetComponent<Text>();
			label.text = Localization.GetFromFile(File, Key, label.text);
			LanguageInfo.SubPubSystem.Subscribe ("Language", ChangeLanguage);
		}

		void ChangeLanguage() {
			File = Localization.StringsByFile.Select(o => o.Key).ToArray()[0];
			Text label = GetComponent<Text>();
			label.text = Localization.GetFromFile(File, Key, label.text);
		}

		void OnDestroy() {
			LanguageInfo.SubPubSystem.UnSubscribe ("Language", ChangeLanguage);
		}
	}
}

