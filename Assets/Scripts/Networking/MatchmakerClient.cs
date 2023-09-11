/*
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
#if UNITY_EDITOR
#endif

public class MatchmakerClient : MonoBehaviour {

    private void OnEnable() 
    {
        ServerStartUp.ClientInstance += SignIn;
    }

    private void OnDisable() 
    {
        ServerStartUp.ClientInstance -= SignIn;
    }

    private async void SignIn() {

    }

    private async Task ClientSignIn(string serviceProfileName = null) 
    {
        if (serviceProfileName != null) 
        {
            serviceProfileName = $"{serviceProfileName}{GetCloneNumberSuffix()}";
            var initOptions = new InitializationOptions();
            initOptions.SetProfile(serviceProfileName);
            await UnityServices.InitializeAsync(initOptions);
        }
        else 
        {
            await UnityServices.InitializeAsync();
        }
        
    }

    private string GetCloneNumberSuffix() 
    {

    }
}
*/