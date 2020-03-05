using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager> {

	private Dictionary<string ,Sprite> sprite = new Dictionary<string, Sprite>();
	private Dictionary<string ,Sprite[]> spriteSet = new Dictionary<string, Sprite[]>();
	private Dictionary<string, AudioClip> audio = new Dictionary<string, AudioClip>();
	private Dictionary<string, Material> material = new Dictionary<string, Material>();
	private Dictionary<string, string[]> saveData = new Dictionary<string, string[]>();

	private static string spritesFilePath = "Sprites/";
	private static string[] spriteSetFilePath = 
	{"Sprites/SpriteSet/", "Sprites/SpriteSet/Effects/",
	"Sprites/SpriteSet/Planes/"};
	private static string prefabFilePath = "Prefab/";
	private static string audioFilePath = "Audio/";
	private static string materialPath = "Material/";

	private static Type spriteType = typeof(Sprite);
	private static Type audioType = typeof(AudioClip);
	private static Type GameObjectType = typeof(GameObject);
	private static Type MaterialType = typeof(Material);
	private static Type textType = typeof(TextAsset);

	private Material pixelSnap;
	private Sprite _skipSprite;
	private bool _skip = false;

	public Material GetPixelSnapMaterial()
	{
		if(pixelSnap == null)
			pixelSnap = Load("Material/PixelSnap", typeof(Material)) as Material;

		return pixelSnap;
	}

	public void EnableSkip(Sprite sprite)
	{
		_skipSprite = sprite;
		_skip = true;
	}

	public void DisableSkip()
	{
		_skip = false;
	}

	public GameObject GetPrefab(string fileName)
	{
		string path = prefabFilePath + fileName;
		GameObject obj = Load(path,GameObjectType) as GameObject;

		return obj;
	}

	public Sprite GetSprite(string fileName)
	{
		if(_skip)
			return _skipSprite;

		if(sprite.ContainsKey(fileName))
			return sprite[fileName];

		string path = spritesFilePath + fileName;
		
		if(Load(path, spriteType) != null)
		{
			if(Load(path, spriteType) as Sprite == null)
				Debug.Log("what the fuck");
		}

		Sprite obj = Load(path,spriteType) as Sprite;
		if(obj == null)
		{
			Debug.Log("file does not exist : " + path);
			return null;
		}
		sprite.Add(fileName,obj);

		return obj;
	}

	public Sprite[] GetSpriteAll(string folderName, int type = 0)
	{
		string cut = folderName.Substring(folderName.IndexOf("Resources") + 10);
		if(spriteSet.ContainsKey(cut))
			return spriteSet[cut];

		string path = cut;
		UnityEngine.Object[] obj = LoadAll(path, spriteType);
		if(obj.Length == 0)
		{
			Debug.Log("file does not exist");
			Debug.Log(cut);
			return null;
		}

		Sprite[] sprites = new Sprite[obj.Length];
		for(int i = 0; i < obj.Length; ++i)
		{
			sprites[i] = obj[i] as Sprite;
		}

		spriteSet.Add(cut,sprites);

		return sprites;
	}

	public Sprite[] GetSpriteSet(string folderName, int type = 0)
	{
		if(spriteSet.ContainsKey(folderName))
			return spriteSet[folderName];

		string path = spriteSetFilePath[type] + folderName;
		UnityEngine.Object[] obj = LoadAll(path, spriteType);
		if(obj.Length == 0)
		{
			Debug.Log("file does not exist");
			return null;
		}

		Sprite[] sprites = new Sprite[obj.Length];
		for(int i = 0; i < obj.Length; ++i)
		{
			sprites[i] = obj[i] as Sprite;
		}

		spriteSet.Add(folderName,sprites);

		return sprites;
	}

	public AudioClip GetAudioClip(string fileName)
	{
		if(audio.ContainsKey(fileName))
			return audio[fileName];

		string path = audioFilePath + fileName;

		AudioClip obj = Load(path, audioType) as AudioClip;
		if(obj == null)
		{
			Debug.Log("file does not exist");
			return null;
		}
		audio.Add(fileName,obj);

		return obj;
	}

	public Material GetMaterial(string fileName)
	{
		if(material.ContainsKey(fileName))
			return material[fileName];

		string path = materialPath + fileName;

		Material obj = Load(path,MaterialType) as Material;
		if(obj == null)
		{
			Debug.Log("file does not exist");
			return null;
		}
		material.Add(fileName,obj);

		return obj;
	}

	public string[] GetSaveData(string fileName)
	{
		if(saveData.ContainsKey(fileName))
			return saveData[fileName];

		TextAsset text = Load(fileName,textType) as TextAsset;
		if(text == null)
		{
			Debug.Log("file does not exist");
			return null;
		}

		string[] s = null;
		s = text.text.Replace("\r",string.Empty).Split('\n');

		return s;
	}

	public void UnloadAllAset()
	{

	}

	public bool UnLoadAudioClip(string fileName)
	{
		string path = audioFilePath + fileName;
		if(audio.ContainsKey(path))
		{
			AudioClip aud = audio[path];
			audio.Remove(path);
			UnLoad(aud);

			return true;
		}

		return false;
	}

	public bool UnLoadSpriteSet(string fileName)
	{
		string path = audioFilePath + fileName;
		if(sprite.ContainsKey(path))
		{
			Sprite[] res = spriteSet[path];
			spriteSet.Remove(path);
			for(int i = 0; i < res.Length; ++i)
				UnLoad(res[i]);
			
			return true;
		}

		return false;
	}

	public bool UnLoadSprite(string fileName)
	{
		string path = audioFilePath + fileName;
		if(sprite.ContainsKey(path))
		{
			Sprite res = sprite[path];
			sprite.Remove(path);
			UnLoad(res);

			return true;
		}

		return false;
	}

	public void UnLoadUnused()
	{
		Resources.UnloadUnusedAssets();
	}

	public void UnLoad(UnityEngine.Object obj)
	{
		Resources.UnloadAsset(obj);
	}

	public UnityEngine.Object Load(string path, Type type)
	{
		return Resources.Load(path, type);
	}

	public UnityEngine.Object[] LoadAll(string path, Type type)
	{
		return Resources.LoadAll(path,type);
	}
}
