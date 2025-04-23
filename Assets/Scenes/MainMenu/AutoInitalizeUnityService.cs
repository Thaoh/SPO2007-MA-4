using System;
using Unity.Services.Core;
using UnityEngine;

public static class AutoInitalizeUnityService
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        InitUnity();
    }

    private static async void InitUnity()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}