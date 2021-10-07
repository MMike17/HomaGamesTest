using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>Static file managing system</summary>
public static class DataManager
{
	static string debugFlag = "<b>[DataManager]</b> : ";
	static string localPath = Application.persistentDataPath;
	static LogLevel debugLevel = LogLevel.LOG;

	public enum LogLevel
	{
		NONE = 0,
		ERROR = 1,
		LOG = 2
	}

	/// <summary>Decide which messages should get loged to console</summary>
	public static void SetLogLevel(LogLevel level)
	{
		debugLevel = level;
	}

#region Folders

	/// <summary>Creates folder with the given relative path (if folder does not already exist)</summary>
	public static void CreateFolder(string folderRelativePath)
	{
		string folderCompletePath = Path.Combine(localPath, folderRelativePath);

		if(Directory.Exists(folderCompletePath))
			Log("Folder at path " + folderCompletePath + " already exists");
		else
			Directory.CreateDirectory(folderCompletePath);
	}

	/// <summary>Deletes folder with the given relative path (if folder exists)</summary>
	public static void DeleteFolder(string folderRelativePath)
	{
		string folderCompletePath = Path.Combine(localPath, folderRelativePath);

		if(Directory.Exists(folderCompletePath))
		{
			Directory.Delete(folderCompletePath, true);
			Log("Delete folder at path " + folderCompletePath);
		}
		else
			LogError("Folder at path " + folderCompletePath + " could not be found");
	}

	/// <summary>Renames folder with the given relative path (if folder exists)</summary>
	public static void RenameFolder(string folderRelativePath, string newFolderRelativePath)
	{
		string folderCompletePath = Path.Combine(localPath, folderRelativePath);
		string newFolderCompletePath = Path.Combine(localPath, newFolderRelativePath);

		if(Directory.Exists(folderCompletePath))
		{
			Directory.Move(folderCompletePath, newFolderCompletePath);

			string[] folderRelativePathParts = newFolderRelativePath.Split('/');
			Log("Renamed folder at path " + folderCompletePath + " to new name " + folderRelativePathParts[folderRelativePathParts.Length - 1]);
		}
		else
			LogError("Folder at path " + folderCompletePath + " could not be found");
	}

#endregion

#region Files

	/// <summary>Returns true if the file exists</summary>
	public static bool DoesFileExist(string relativePath)
	{
		string completePath = Path.Combine(localPath, relativePath) + ".bin";

		return File.Exists(completePath);
	}

	/// <summary>Saves object to file (if the object is serializable)</summary>
	public static void SaveObject<T>(T data, string relativePath)
	{
		string completePath = Path.Combine(localPath, relativePath) + ".bin";

		try
		{
			File.WriteAllText(completePath, EncryptDecrypt(JsonUtility.ToJson(data)));
		}
		catch (SystemException exception)
		{
			LogError("Couldn't save file with path " + completePath + "\n" + exception.ToString());
			return;
		}

		Log("Saved data to file at path " + completePath);
	}

	/// <summary>Loads object from file (if file exists)</summary>
	public static T LoadObjectAtPath<T>(string relativePath)
	{
		string completePath = relativePath.Contains(localPath) ? relativePath : Path.Combine(localPath, relativePath) + ".bin";

		if(File.Exists(completePath))
		{
			T data;

			try
			{
				data = JsonUtility.FromJson<T>(EncryptDecrypt(File.ReadAllText(completePath)));
			}
			catch (SystemException exception)
			{
				LogError("Could not load data from file at path " + completePath + "\n" + exception.ToString());
				return default(T);
			}

			Log("Data was successfuly loaded from file at path " + completePath);
			return data;
		}
		else
		{
			LogError("The file at path " + completePath + " was not found");
			return default(T);
		}
	}

	/// <summary>Loads all objects from files in folder (if folder exists)</summary>
	public static T[] LoadAllObjectsInFolder<T>(string folderRelativePath)
	{
		string folderCompletePath = Path.Combine(localPath, folderRelativePath);

		if(Directory.Exists(folderCompletePath))
		{
			List<T> dataList = new List<T>();

			foreach (string filePath in Directory.GetFiles(folderCompletePath))
				dataList.Add(LoadObjectAtPath<T>(filePath));

			return dataList.ToArray();
		}
		else
		{
			LogError("Folder at path " + folderRelativePath + " could not be found");
			return null;
		}
	}

	/// <summary>Deletes file with the given relative path (if file exists)</summary>
	public static void DeleteFile(string relativePath)
	{
		string completePath = Path.Combine(localPath, relativePath) + ".bin";

		if(File.Exists(completePath))
		{
			File.Delete(completePath);
			Log("Deleted file at path " + completePath);
		}
		else
			LogError("File at path " + completePath + " could not be found");
	}

	/// <summary>Renames file with the given relative path (if file exists)</summary>
	public static void RenameFile(string relativePath, string newRelativePath)
	{
		string completePath = Path.Combine(localPath, relativePath) + ".bin";
		string newCompletePath = Path.Combine(localPath, newRelativePath) + ".bin";

		if(File.Exists(completePath))
		{
			File.Move(completePath, newCompletePath);

			string[] relativePathParts = newRelativePath.Split('/');
			Log("Renamed file at path " + completePath + " to new name " + relativePathParts[relativePathParts.Length - 1]);
		}
		else
			LogError("File at path " + completePath + " could not be found");
	}

#endregion

	static string EncryptDecrypt(string data)
	{
		int key = 129;
		string result = string.Empty;

		for (int i = 0; i < data.Length; i++)
		{
			result += ((char) (data[i] ^ key));
		}

		return result;
	}

#region LogMethods

	static void Log(string message)
	{
		if(debugLevel == LogLevel.LOG)
			Debug.Log(debugFlag + message);
	}

	static void LogError(string message)
	{
		if(debugLevel >= LogLevel.ERROR)
			Debug.LogError(debugFlag + message);
	}

#endregion
}