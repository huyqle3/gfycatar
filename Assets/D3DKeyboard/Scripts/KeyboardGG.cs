//Author and copyright owner: Matrix Inception Inc.
//Date: 2016-10-31
//This script is attached to each key, and prompts an action when a key is selected.
//

using UnityEngine;
using System.Collections;

public class KeyboardGG : MonoBehaviour {

    public GameObject KeyboardOne;
    KeyboardMain keyboardMain;

    //inputString keys track of typed message
    string inputString; 
    string keyName;
    int keySoundIndex;
    AudioSource audioSource;
    Color keyColor;
    float lastTypeTime;
    

	// Use this for initialization
	void Start () {
        keyboardMain = KeyboardOne.GetComponent<KeyboardMain>();      
        audioSource = KeyboardOne.GetComponent<AudioSource>();
        keyColor = GetComponent<MeshRenderer>().material.GetColor("_Color");
    }
	
	// Update is called once per frame
	void Update () {
        if(Time.realtimeSinceStartup - lastTypeTime > 0.1f && Time.realtimeSinceStartup - lastTypeTime < 0.15f)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", keyColor);
        }
    }

    void OnSelect()
    {
        keySoundIndex = (int)Mathf.Round(Random.Range(-0.49f, keyboardMain.keySounds.Length - 0.51f));
        audioSource.clip = keyboardMain.keySounds[keySoundIndex];
        audioSource.loop = false;
        audioSource.Play();

        GetComponent<MeshRenderer>().material.SetColor("_Color", keyColor+new Color(0.2f,0.2f,0.2f,0f));
        lastTypeTime = Time.realtimeSinceStartup;
            
        keyName = gameObject.name;
        inputString = keyboardMain.InputDisplay.GetComponent<TextMesh>().text;

        switch (keyName)
        {
            case "keyBackspace":
                if (inputString.Length > 0) {
                    //check whether backspace should remove a line
                    if(inputString.Length>1 && inputString.Substring(inputString.Length-2,2)== System.Environment.NewLine)
                    {
                        keyboardMain.InputDisplay.transform.localPosition += Vector3.up * (-0.07f);
                        inputString = inputString.Substring(0, inputString.Length - 2);
                    }
                    else { 
                        inputString = inputString.Substring(0, inputString.Length - 1);
                    }
                }
                break;
            case "keyShift":
                keyboardMain.OnShift();
                break;
            case "keySpace":
                inputString += " ";
                break;
            case "keyReturn":
                inputString += System.Environment.NewLine;
                keyboardMain.InputDisplay.transform.localPosition += Vector3.up*0.07f;
                break;
            case "keyDone":
                keyboardMain.OnDone();
                break;
            default:
                if (keyboardMain.ShiftOn)
                {
                    inputString += keyName.Substring(4, 1);
                }
                else
                {
                    inputString += keyName.Substring(3, 1);
                }
                break;
        }

        keyboardMain.InputDisplay.GetComponent<TextMesh>().text = inputString;
        inputString = null;
    }


}
