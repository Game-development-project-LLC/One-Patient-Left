using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

public class UgsBootstrap : MonoBehaviour
{
    public static UgsBootstrap Instance { get; private set; }

    public bool IsInitialized { get; private set; }
    public event Action Initialized;

    private async void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized) return;

        try
        {
            await UnityServices.InitializeAsync();
            IsInitialized = true;
            Initialized?.Invoke();
            Debug.Log("UGS: Initialized");
        }
        catch (Exception e)
        {
            Debug.LogError("UGS: Initialize failed: " + e);
        }
    }
}
