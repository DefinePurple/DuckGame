using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameSparks.Api.Responses;
using Facebook.Unity;
using GooglePlayGames;

public class IOManager : MonoBehaviour {
    public InputField username, password;
    public Button authButton, regButton, googlePlusButton, googlePlayButton, facebookButton;
    public Text details;

    private void Start() {
        #region Login Screen Button Listeners
        authButton.onClick.AddListener(() => {
            UpdateText("Authenticating...\n");
            AuthReq();
        });

        regButton.onClick.AddListener(() => {
            UpdateText("Registering...\n");
            RegReq();
        });

        googlePlusButton.onClick.AddListener(() => {
            UpdateText("Signing in with Google+...\n");
            GooglePlusStart();
        });

        googlePlayButton.onClick.AddListener(() => {
            UpdateText("Signing in with Google Play...\n");
            GooglePlayStart();
        });

        facebookButton.onClick.AddListener(() => {
            UpdateText("Signing in with Facebook...\n");
            FacebookStart();
        });
        #endregion
    }

    #region GameSparks
    private void AuthReq() {
        GameSparksManager.Instance().AuthenticateUser(username.text, password.text, OnAuthentication);
    }

    private void RegReq() {
        GameSparksManager.Instance().RegisterUser(username.text, password.text, OnRegistration);
    }

    private void OnRegistration(RegistrationResponse _resp) {
        if ((bool)_resp.NewPlayer)
            UpdateText("Account Created\n");
        else if (!(bool)_resp.NewPlayer)
            UpdateText("Account already exists\n");
    }

    private void OnAuthentication(AuthenticationResponse _resp) {
        if (!_resp.HasErrors) {
            UpdateText("GS Successful Authentication\n");
            GoToMain();
        } else if (_resp.HasErrors)
            UpdateText("GS Error Autenticating\n");
    }
    #endregion

    #region Google
    private void GooglePlusStart() {
        GameSparksManager.Instance().GoogleSignIn(OnGooglePlusSignIn);
    }

    private void GooglePlayStart() {
        GameSparksManager.Instance().GoogleSignIn(OnGooglePlaySignIn);
    }

    private void OnGooglePlusSignIn(bool success) {
        if (success) {
            GameSparksManager.Instance().GameSparksGooglePlus(OnGameSparksGoogle);
        }
    }

    private void OnGooglePlaySignIn(bool success) {
        if (success) {
            GameSparksManager.Instance().GameSparksGooglePlay(OnGameSparksGoogle);
        }
    }

    private void OnGameSparksGoogle(AuthenticationResponse _resp) {
        if (!_resp.HasErrors) {
            UpdateText("Sucessfully linked GS with Google\n");
            GoToMain();
        } else if (_resp.HasErrors) {
            UpdateText("Failed to link GS with Google\n");
            UpdateText(_resp.Errors.JSON.ToString());
            PlayGamesPlatform.Instance.SignOut();
        }
    }
    #endregion

    #region Facebook
    private void FacebookStart() {
        GameSparksManager.Instance().FacebookSignIn(OnFacebook);
    }

    private void OnFacebook(AuthenticationResponse _resp) {
        if (!_resp.HasErrors) {
            UpdateText("Sucessfully linked GS & Facebook\n");
            GoToMain();
            GameSparksManager.Instance().isLoggedIn = true;
            UpdateText(GameSparksManager.Instance().isLoggedIn.ToString());
        } else if (_resp.HasErrors) {
            UpdateText("Failed to link GS & Facebook\n");
            FB.LogOut();
        }
    }
    #endregion

    private void GoToMain() {
        SceneManager.LoadScene("Menu");
    }

    private void UpdateText(string words) {
        details.text += words;
    }

    private void ResetIO() {
        username.text = "";
        password.text = "";
    }
}