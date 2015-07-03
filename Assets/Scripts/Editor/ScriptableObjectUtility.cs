using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility {
	static void CreateAsset<T>() where T : ScriptableObject {
		T asset = ScriptableObject.CreateInstance<T>();

		string path = GetSelectionPath ();
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
		
		AssetDatabase.CreateAsset(asset, assetPathAndName);
		
		AssetDatabase.SaveAssets();
		Selection.activeObject = asset;
	}
	static string GetSelectionPath() {
		string path = "Assets";
		foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
			path = AssetDatabase.GetAssetPath(obj);
			if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
				path = Path.GetDirectoryName(path);
				break;
			}
		}
		return path;
	}

	/*[MenuItem("Assets/Create/GameStrings")]
	public static void CreateGameStrings() {
		ScriptableObjectUtility.CreateAsset<GameStrings>();
	}*/
}