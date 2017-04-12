using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using Persistence;
using UnityEngine;
public static class LoadJson {

	static FileStream filestream;

	public static pe_SavefileNames loadSavefileNames(){
		pe_SavefileNames pe_savefileNames = new pe_SavefileNames ();
		pe_savefileNames.pe_savefilenames = new string[0];
		try
		{
			string jsonstr = string.Empty;
			using (StreamReader sr = new StreamReader("savefileIndex.json", Encoding.UTF8, true))
			{
				while(!sr.EndOfStream)
				{
					Debug.LogWarning("Waring : Loading ==》" +(jsonstr +=sr.ReadLine()));
				}
			}
			Debug.Log("Json: "+jsonstr);
			return JsonMapper.ToObject<pe_SavefileNames>(jsonstr);
		}
		catch (Exception ex)
		{
			ex.Message.ToString();
			return pe_savefileNames;
		}
	}

	public static pe_GameState loadGameState(){
		try
		{
			string jsonstr = string.Empty;
			using (StreamReader sr = new StreamReader(Application.dataPath + "savefile.json", Encoding.UTF8, true))
			{
				while(!sr.EndOfStream)
				{
					Debug.LogWarning("Waring : Loading ==》" +(jsonstr +=sr.ReadLine()));

				}

			}
			Debug.Log("Json: "+jsonstr);
			return JsonMapper.ToObject<pe_GameState>(jsonstr);
		}
		catch (Exception ex)
		{
			ex.Message.ToString();
			return null;
		}
	}

	public static pe_GameState loadGameState(string fileName){
		try
		{
			string jsonstr = string.Empty;
			using (StreamReader sr = new StreamReader(fileName+".json", Encoding.UTF8, true))
			{
				while(!sr.EndOfStream)
				{
					Debug.LogWarning("Waring : Loading ==》" +(jsonstr +=sr.ReadLine()));

				}

			}
			Debug.Log("Json: "+jsonstr);
			return JsonMapper.ToObject<pe_GameState>(jsonstr);
		}
		catch (Exception ex)
		{
			ex.Message.ToString();
			return null;
		}
	}

	public static pe_Players loadPlayers(){
		return null;
	}
}
