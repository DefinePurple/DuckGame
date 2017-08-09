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
    public Button authButton, regButton, googleButton, facebookButton;
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

        googleButton.onClick.AddListener(() => {
            UpdateText("Signing in with Google...\n");
            GoogleStart();
        });

        facebookButton.onClick.AddListener(() => {
            UpdateText("Signing in with Facebook...\n");
            FacebookStart();
        });
        #endregion
    }

    private void AuthReq() {
        GameSparksManager.Instance().AuthenticateUser(username.text, password.text, OnAuthentication);
    }

    private void RegReq() {
        GameSparksManager.Instance().RegisterUser(username.text, password.text, OnRegistration);
    }

    private void GoogleStart() {
        GameSparksManager.Instance().GoogleSignIn(OnGoogleSignIn);
    }

    private void FacebookStart() {
        GameSparksManager.Instance().FacebookSignIn(OnFacebook);
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

    private void OnGoogleSignIn(bool success) {
        UpdateText(success.ToString() + "\n");
        if (success) {
            GameSparksManager.Instance().GameSparksGoogle(OnGameSparksGoogle);
        }
    }

    private void OnGameSparksGoogle(AuthenticationResponse _resp) {
        if (!_resp.HasErrors) {
            UpdateText("Sucessfully linked GS & GPS\n");
            GoToMain();
        } else if (_resp.HasErrors) {
            UpdateText("Failed to link GS & GPS\n");
            PlayGamesPlatform.Instance.SignOut();
        }
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
