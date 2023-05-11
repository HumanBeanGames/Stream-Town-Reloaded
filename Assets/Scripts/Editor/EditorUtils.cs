using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Globals
{
	public static class EditorUtils
	{
		/// <summary>
		/// Creates and returns a texture with the specified color.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Texture2D MakeTexture(int width, int height, Color color)
		{
			Color[] pix = new Color[width * height];

			for (int i = 0; i < pix.Length; i++)
				pix[i] = color;

			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();

			return result;
		}

		/// <summary>
		/// Draws a line in an editor tool.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="height"></param>
		public static void DrawGUILine(Color color, int height = 1)
		{
			GUILayout.BeginHorizontal();
			{
				Rect rect = EditorGUILayout.GetControlRect(false, height);
				rect.height = height;
				EditorGUI.DrawRect(rect, color);
			}
			GUILayout.EndHorizontal();
		}

		/// <summary>
		/// Returns a ScriptableObject Asset File.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="asset"></param>
		/// <param name="filter"></param>
		/// <param name="folder"></param>
		/// <returns></returns>
		public static bool GetAssetFile<T>(out ScriptableObjectAssetData<T> asset, string filter = "Default", string folder = "Assets/ScriptableObjects") where T : ScriptableObject
		{
			bool assetFound = false;
			string[] GUIDs = AssetDatabase.FindAssets(filter, new[] { folder });

			if (GUIDs.Length > 0)
			{
				assetFound = true;
				asset.GUID = GUIDs[0];
				asset.AssetPath = AssetDatabase.GUIDToAssetPath(GUIDs[0]);
				asset.Asset = AssetDatabase.LoadAssetAtPath(asset.AssetPath, typeof(T)) as T;
			}
			else
			{
				asset.GUID = null;
				asset.AssetPath = null;
				asset.Asset = null;
			}

			return assetFound;
		}

		/// <summary>
		/// Returns a list of ScriptableObject asset files.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assets"></param>
		/// <param name="filter"></param>
		/// <param name="folder"></param>
		/// <returns></returns>
		public static bool GetAssetFiles<T>(out List<ScriptableObjectAssetData<T>> assets, string filter = "Default", string folder = "Assets/ScriptableObJects") where T : ScriptableObject
		{
			string[] GUIDs = AssetDatabase.FindAssets(filter);

			if (GUIDs == null || GUIDs.Length == 0)
			{
				assets = null;
				return false;
			}

			assets = new List<ScriptableObjectAssetData<T>>();

			foreach (var GUID in GUIDs)
			{
				ScriptableObjectAssetData<T> asset = new ScriptableObjectAssetData<T>();
				asset.GUID = GUID;
				asset.AssetPath = AssetDatabase.GUIDToAssetPath(GUID);
				asset.Asset = AssetDatabase.LoadAssetAtPath(asset.AssetPath, typeof(T)) as T;
				assets.Add(asset);
			}

			return true;
		}

		/// <summary>
		/// Returns all folders names at a given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string[] GetFolderNamesInPath(string path)
		{
			var info = new DirectoryInfo(path);

			var directoryData = info.GetDirectories();

			if (directoryData == null || directoryData.Length == 0)
				return null;

			string[] folderNames = new string[directoryData.Length];

			for (int i = 0; i < folderNames.Length; i++)
			{
				folderNames[i] = directoryData[i].Name;
			}

			return folderNames;
		}

		/// <summary>
		/// Gets the names of all files in a given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string[] GetFileNamesInPath(string path)
		{
			var info = new DirectoryInfo(path);
			return GetFileNamesInPath(info);
		}

		/// <summary>
		/// Gets the name of all files in a given directory.
		/// </summary>
		/// <param name="dirInfo"></param>
		/// <returns></returns>
		public static string[] GetFileNamesInPath(DirectoryInfo dirInfo)
		{
			if (dirInfo == null)
				return null;

			var files = dirInfo.GetFiles();

			if (files == null || files.Length == 0)
				return null;

			string[] fileNames = new string[(int)(files.Length * 0.5f)];

			for (int i = 0, j = 0; i < fileNames.Length; j++)
			{
				if (files[j].Name.Contains(".meta"))
					continue;

				fileNames[i] = files[j].Name;
				fileNames[i] = fileNames[i].Remove(fileNames[i].Length - 6);
				fileNames[i] = fileNames[i].Split('_')[1];
				i++;
			}

			return fileNames;
		}
	}
}