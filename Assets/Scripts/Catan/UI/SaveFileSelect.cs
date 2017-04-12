using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileSelect : MonoBehaviour {

	public Dropdown fileOptions;
	public List<string> filenames;

	void Start(){
		setOptions (LoadJson.loadSavefileNames ().pe_savefilenames);
	}

	// Use this for initialization
	public void setOptions(string[] options){
		fileOptions.ClearOptions ();
		for(int i=0;i<options.Length;i++){
			filenames.Add(options[i]);
		}
		fileOptions.AddOptions (filenames);
	}
	// Update is called once per frame
	public string getSaveSelection(){
		return filenames[fileOptions.value];
	}
}
