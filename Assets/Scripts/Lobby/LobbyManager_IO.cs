using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Responses;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Facebook.Unity;

public class LobbyManager_IO : MonoBehaviour {

    private LobbyManager_Master lm_master;
    public InputField username, password;

    // Use this for initialization
    void OnEnable() {
        SetInitial();
        lm_master.EventAuthReq += AuthReq;
        lm_master.EventRegReq += RegReq;
        lm_master.EventGoogleSignIn += GoogleStart;
        lm_master.EventFacebookSignIn += FacebookStart;
    }

    private void OnDisable() {
        lm_master.EventAuthReq -= AuthReq;
        lm_master.EventRegReq -= RegReq;
        lm_master.EventGoogleSignIn -= GoogleStart;
        lm_master.EventFacebookSignIn -= FacebookStart;
    }

    private void SetInitial() {
        lm_master = GetComponent<LobbyManager_Master>();
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
            lm_master.CallEventUpdateText("Account Created\n");
        else if (!(bool)_resp.NewPlayer)
            lm_master.CallEventUpdateText("Account already exists\n");
    }

    private void OnAuthentication(AuthenticationResponse _resp) {
        if (!_resp.HasErrors) {
            lm_master.CallEventUpdateText("GS Successful Authentication\n");
            lm_master.CallEventGoToMain();
            GameSparksManager.Instance().isLoggedIn = true;
            lm_master.CallEventUpdateText(GameSparksManager.Instance().isLoggedIn.ToString());
        } else if (_resp.HasErrors)
            lm_master.CallEventUpdateText("GS Error Autenticating\n");
    }

    private void OnGoogleSignIn(bool success) {
        if (success) {
            GameSparksManager.Instance().GameSparksGoogle(OnGameSparksGoogle);
        }
    }

    private void OnGameSparksGoogle(AuthenticationResponse _resp) {
        if (!_resp.HasErrors) {
            lm_master.CallEventUpdateText("Sucessfully linked GS & GPS\n");
            lm_master.CallEventGoToMain();
            GameSparksManager.Instance().isLoggedIn = true;
            lm_master.CallEventUpdateText(GameSparksManager.Instance().isLoggedIn.ToString());
        } else if (_resp.HasErrors) {
            lm_master.CallEventUpdateText("Failed to link GS & GPS\n");
            PlayGamesPlatform.Instance.SignOut();
        }
    }

    private void OnFacebook(AuthenticationResponse _resp) {
        if (!_resp.HasErrors) {
            lm_master.CallEventUpdateText("Sucessfully linked GS & Facebook\n");
            lm_master.CallEventGoToMain();
            GameSparksManager.Instance().isLoggedIn = true;
            lm_master.CallEventUpdateText(GameSparksManager.Instance().isLoggedIn.ToString());
        } else if (_resp.HasErrors) {
            lm_master.CallEventUpdateText("Failed to link GS & Facebook\n");
            FB.LogOut();
        }
    }
}
