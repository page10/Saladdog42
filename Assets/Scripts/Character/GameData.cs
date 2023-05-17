using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameData 
{
    public CharacterNameModel characterNameModel;

    public LoadedCharacterStatus loadedCharacterStatus;
    
    // Start is called before the first frame update
    public void Start()
    {
        //string characterNames = File.ReadAllText( "Json/CharacterNames.json");
        TextAsset characterNames = Resources.Load<TextAsset>("Json/CharacterNames");
        characterNameModel = JsonUtility.FromJson<CharacterNameModel>(characterNames.text);

        foreach (string name in characterNameModel.names)
        {
            Debug.Log(name);
        }
        
        TextAsset characterStatus = Resources.Load<TextAsset>("Json/CharacterStatus");
        loadedCharacterStatus = JsonUtility.FromJson<LoadedCharacterStatus>(characterStatus.text);
        
    }

}
