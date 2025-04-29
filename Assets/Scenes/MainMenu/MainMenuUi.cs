using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUi : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string challangeSceneName = "Challange";
    [SerializeField] private string scoreBoardSceneName = "ScoreBoard";

    [Header("UI Element Refferences")] 
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _challengeButton;
    [SerializeField] private Button _scoreBoardButton;

    [SerializeField] private TMP_Text _username;

    private void Start()
    {
        _startGameButton.onClick.AddListener(() =>
        {
            Debug.Log("Starting game");
            SceneManager.LoadScene(gameSceneName);
        });
        _scoreBoardButton.onClick.AddListener(() =>
        {

            SceneManager.LoadScene(scoreBoardSceneName);
        });        
        _challengeButton.onClick.AddListener(() =>
        {

            SceneManager.LoadScene(challangeSceneName);
        });
    }

    private void OnInitalizedServices()
    {
        UnityServices.Initialized -= OnInitalizedServices;
        // Make sure ui is updated when signin state changes
        AuthenticationService.Instance.SignedIn += OnSignIn;
        AuthenticationService.Instance.SignedOut += OnSignOut;
        AuthenticationService.Instance.SignInFailed += OnSignInFailed;
        AuthenticationService.Instance.Expired += Expired;
        AuthenticationService.Instance.SignInCodeExpired += Expired;
        RefreshUi();
    }

    void OnEnable()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            OnInitalizedServices();
        }
        else
        {
            UnityServices.Initialized += OnInitalizedServices;
        }

        RefreshUi();
    }

    private void OnDisable()
    {
        UnityServices.Initialized -= OnInitalizedServices;
        if (AuthenticationService.Instance != null)
        {
            AuthenticationService.Instance.SignedIn -= OnSignIn;
            AuthenticationService.Instance.SignedOut -= OnSignOut;
            AuthenticationService.Instance.SignInFailed -= OnSignInFailed;
            AuthenticationService.Instance.Expired -= Expired;
            AuthenticationService.Instance.SignInCodeExpired -= Expired;
        }
    }

    private void Expired()
    {
        RefreshUi();
    }

    private void OnSignInFailed(RequestFailedException obj)
    {
        RefreshUi();
    }

    private void OnSignIn()
    {
        RefreshUi();
    }

    private void OnSignOut()
    {
        RefreshUi();
    }

    void RefreshUi()
    {
        UpdateUsername();
    }

    private void UpdateUsername()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            _username.text = "Services not initialized.";
            return;
        }

        if (AuthenticationService.Instance == null)
        {
            _username.text = "No Authenticator available.";
            return;
        }

        _username.text = "";
        _username.text = AuthenticationService.Instance.IsSignedIn ? "Signed In" : "Signed Out";
        if (AuthenticationService.Instance.IsSignedIn)
        {
            bool isAnonymous = AuthenticationService.Instance?.PlayerInfo?.Identities?.Count == 0;
            if (isAnonymous)
            {
                _username.text += "\n(Anonymous Account)";
                return;
            }

            _username.text += string.IsNullOrEmpty(AuthenticationService.Instance?.PlayerName)
                ? $"\nID:[{AuthenticationService.Instance?.PlayerId ?? "No ID"}]"
                : $"\n{AuthenticationService.Instance.PlayerName}";
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}