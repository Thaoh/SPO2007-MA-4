using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class UGSAuthenticator : MonoBehaviour
{
    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            PlayerAccountService.Instance.SignedIn += SignInWithUnityAuth;
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }

    public void SignIn()
    {
        StartPlayerAccountsSignInAsync();
    }

    public async void SignInAnonymous()
    {
        if (!PlayerAccountService.Instance.IsSignedIn)
        {
            await SignUpAnonymouslyAsync();
        }
        else
        {
            Debug.Log(PlayerAccountService.Instance.AccessToken);
        }
    }
    
    async void StartPlayerAccountsSignInAsync()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            // If the player is already signed into Unity Player Accounts, proceed directly to the Unity Authentication sign-in.
            SignInWithUnityAuth();
            return;
        }

        try
        {
            // This will open the system browser and prompt the user to sign in to Unity Player Accounts
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (PlayerAccountsException ex) {
            // Compare error code to PlayerAccountsErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    
    async void SignInWithUnityAuth()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }
    
    async Task LinkWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
            Debug.Log("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            // Prompt the player with an error message.
            Debug.LogError("This user is already linked with another account. Log in instead.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    
    async Task SignUpAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
}
