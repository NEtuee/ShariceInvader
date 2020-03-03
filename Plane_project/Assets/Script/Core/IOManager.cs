using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class IOManager {

	public static void WriteStringToFile_NoMark(string[] str,string fileName, bool docu = true)
	{
		string path = docu ? PathForDocumentsFile(fileName) : fileName;
		FileStream file = new FileStream ( path, FileMode.Create, FileAccess.Write );
		StreamWriter sw = new StreamWriter( file );
		int line = str.Length;

		for(int i = 0; i < line; ++i)
		{
			sw.WriteLine(str[i]);
		}

		sw.Close();
		file.Close();
	}

	public static string[] ReadStringFromFile(string fileName)
	{
		if(File.Exists(fileName))
		{
			FileStream file = new FileStream(fileName,FileMode.Open,FileAccess.Read);
			StreamReader st = new StreamReader(file);

			if(file == null || st == null)
			{
				Debug.Log("file is does not exists");
				return null;
			}

			string[] s = null;
			s = st.ReadToEnd().Replace("\r",string.Empty).Split('\n');

			st.Close();
			file.Close();

			return s;
		}
		else
		{
			Debug.Log("file does not exists");
			return null;
		}
	}

	public static string ReadStringFromFile_NoSplit(string fileName)
	{
		string path = PathForDocumentsFile(fileName);

		if(File.Exists(path))
		{
			FileStream file = new FileStream(path,FileMode.Open,FileAccess.Read);
			StreamReader st = new StreamReader(file);

			if(file == null || st == null)
			{
				Debug.Log("file is does not exists");
				return null;
			}

			string s = null;
			s = st.ReadToEnd();

			st.Close();
			file.Close();

			return s;
		}
		else
		{
			Debug.Log("file is empty");
			return null;
		}
	}

	public static string PathForDocumentsFile(string str)
	{
		string path = "";
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Debug.Log("notyet");
		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			path = Application.persistentDataPath;
			path = path.Substring(0,path.LastIndexOf('/'));
		}
		else
		{
			path = Application.dataPath;
			path = path.Substring(0,path.LastIndexOf('/'));
		}

		return Path.Combine(path,str);
	}
}
