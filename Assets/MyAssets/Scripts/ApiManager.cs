using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{

    public static IEnumerator GetJson(Action<bool, string> callback,  string deckNum)
    {
        var apiCall = $"https://api.test.xrv.app/Api/App/Deck/{deckNum}/DetailPublic";
        //Create the request
        var request = new UnityWebRequest(apiCall, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        //Send the request and wait for the response
        yield return request.SendWebRequest();

        //Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(true, request.downloadHandler.text);
        }
        else
        {
            callback?.Invoke(false, request.error);
        }
    }
}
