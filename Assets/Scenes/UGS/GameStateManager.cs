using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField textField, numberField;
    [SerializeField] private TMP_Text nameText, scoreText;

    public UnityEvent OnDataSaved;
    public UnityEvent OnDataLoaded;


    public void SaveDataButtonClick()
    {
        _ = SaveData();
        OnDataSaved.Invoke();
    }
    
    public async Task SaveData()
    {
        try
        {
            if (textField.text != "" && numberField.text != "")
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(textField.text);
                var playerData = new Dictionary<string, object>{
                    {"Name", textField.text},
                    {"Score", numberField.text}
                };
                await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
                Debug.Log($"Saved data {string.Join(',', playerData)}");
                
            }
            else
            {
                return;
            }
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }

    public async void LoadData()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
            "Name", "Score"
        });

        if (playerData.TryGetValue("Name", out var firstKey)) {
            nameText.text = firstKey.Value.GetAs<string>();
        }

        if (playerData.TryGetValue("Score", out var secondKey)) {
            scoreText.text = secondKey.Value.GetAs<string>();
        }
        OnDataLoaded.Invoke();
    }
}