using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class GameData 
{
    public static CharacterNameModel characterNameModel;

    public static Dictionary<string, CharacterStatus> characterStatusDict = new Dictionary<string, CharacterStatus>();

    // Start is called before the first frame update
    public static void Start()
    {
        //string characterNames = File.ReadAllText( "Json/CharacterNames.json");
        TextAsset characterNames = Resources.Load<TextAsset>("Json/CharacterNames");
        characterNameModel = JsonUtility.FromJson<CharacterNameModel>(characterNames.text);

        foreach (string name in characterNameModel.names)
        {
            Debug.Log(name);
        }
        
        TextAsset characterStatus = Resources.Load<TextAsset>("Json/CharacterStatus");
        LoadedCharacterStatus loadedCharacterStatus = JsonUtility.FromJson<LoadedCharacterStatus>(characterStatus.text);
        foreach (CharacterStatus status in loadedCharacterStatus.CharacterStatus)
        {
            characterStatusDict.Add(status.id, status);
        }

        
    }

}
