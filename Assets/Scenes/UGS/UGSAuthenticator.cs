using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class UGSAuthenticator : MonoBehaviour
{
    // Define a void Action for the authentication success event
    public static event Action OnAuthFinishedSuccess;
    
    // Static fields to ensure state is shared across all instances
    private static bool _isInitialized = false;
    private static bool _authInProgress = false;
    private static readonly object _lockObject = new object();
    
    // Add a delay between authentication attempts
    private const int AUTH_RETRY_DELAY_MS = 500;
    private const int MAX_AUTH_ATTEMPTS = 3;

    public static UGSAuthenticator Instance;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(this);
    }

    private async void Start()
    {
        // Initialize services if needed
        if (!_isInitialized)
        {
            try
            {
                Debug.Log("[Authentication] Initializing Unity Services");
                await UnityServices.InitializeAsync();
                _isInitialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Authentication] Unity Services initialization failed: {e.Message}");
                return;
            }
        }
        
        // Try to authenticate
        await AttemptAuthentication();
    }

    private async Task AttemptAuthentication()
    {
        // First check if we're already signed in
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log($"[Authentication] Already signed in as {AuthenticationService.Instance.PlayerName} (ID: {AuthenticationService.Instance.PlayerId})");
            TriggerAuthSuccess();
            return;
        }
        
        // Don't attempt to authenticate if another authentication is in progress
        if (IsAuthInProgress())
        {
            Debug.Log("[Authentication] Authentication already in progress, skipping");
            return;
        }
        
        // Try anonymous authentication with retries
        await TryAnonymousAuthentication();
    }
    
    // Safely trigger the authentication success event on the main thread
    private void TriggerAuthSuccess()
    {
        // Make sure we're on the main thread for UI updates
        if (OnAuthFinishedSuccess != null)
        {
            OnAuthFinishedSuccess.Invoke();
        }
    }
    
    private bool IsAuthInProgress()
    {
        lock (_lockObject)
        {
            return _authInProgress;
        }
    }
    
    private void SetAuthInProgress(bool inProgress)
    {
        lock (_lockObject)
        {
            _authInProgress = inProgress;
        }
    }

    private async Task TryAnonymousAuthentication()
    {
        // Set authentication in progress
        SetAuthInProgress(true);
        
        try
        {
            // Try multiple times with delays between attempts
            for (int attempt = 1; attempt <= MAX_AUTH_ATTEMPTS; attempt++)
            {
                try
                {
                    // Wait a moment before trying again (except first attempt)
                    if (attempt > 1)
                    {
                        Debug.Log($"[Authentication] Retrying authentication (attempt {attempt}/{MAX_AUTH_ATTEMPTS})");
                        await Task.Delay(AUTH_RETRY_DELAY_MS);
                    }
                    
                    // Check again if we're signed in before attempting
                    if (AuthenticationService.Instance.IsSignedIn)
                    {
                        Debug.Log("[Authentication] Successfully signed in");
                        TriggerAuthSuccess();
                        return;
                    }
                    
                    Debug.Log("[Authentication] Attempting anonymous sign-in");
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    
                    Debug.Log($"[Authentication] Successfully signed in as {AuthenticationService.Instance.PlayerName} (ID: {AuthenticationService.Instance.PlayerId})");
                    TriggerAuthSuccess();
                    return;
                }
                catch (AuthenticationException ex) when (ex.Message.Contains("already signing in"))
                {
                    // If we get the "already signing in" error, wait and then check if we're signed in
                    Debug.Log("[Authentication] Sign-in already in progress, waiting...");
                    await Task.Delay(AUTH_RETRY_DELAY_MS);
                    
                    // Check if sign-in completed while we were waiting
                    if (AuthenticationService.Instance.IsSignedIn)
                    {
                        Debug.Log("[Authentication] Successfully signed in after waiting");
                        TriggerAuthSuccess();
                        return;
                    }
                    
                    // If we've tried enough times, rethrow
                    if (attempt == MAX_AUTH_ATTEMPTS)
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Authentication] Error during attempt {attempt}: {ex.Message}");
                    
                    // If we've tried enough times, rethrow
                    if (attempt == MAX_AUTH_ATTEMPTS)
                    {
                        throw;
                    }
                }
            }
        }
        finally
        {
            // Always reset authentication status when done
            SetAuthInProgress(false);
        }
    }

    // Public method for manual sign-in button
    public void SignIn()
    {
        _ = AttemptAuthentication();
    }
}
