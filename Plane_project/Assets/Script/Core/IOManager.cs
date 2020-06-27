using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class IOManager {

	public class INIDataInfo
	{
		public string title;
		public string data;

		public INIDataInfo(string t, string d) {title = t; data = d;}
	};

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

	public static Dictionary<string,INIDataInfo[]> ReadiniFile(string fileName)
	{
		var data = ReadStringFromFile(fileName);
		if(data == null)
			return null;

		var blockList = new Dictionary<string,INIDataInfo[]>();
		var dataList = new List<INIDataInfo>();
		string title = "";

		foreach(var line in data)
		{
			if(line == "")
			{
				continue;
			}
			if(line[0] == '[')
			{
				if(title != "" && dataList.Count != 0)
				{
					blockList[title] = dataList.ToArray();
					dataList.Clear();
				}
				
				title = line.Substring(1,line.Length - 2);
				blockList.Add(title,null);
			}
			else
			{
				var split = line.Split('=');
				dataList.Add(new INIDataInfo(split[0],split[1]));
			}
		}

		if(title != "" && dataList.Count != 0)
		{
			blockList[title] = dataList.ToArray();
			dataList.Clear();
		}

		return blockList;
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
