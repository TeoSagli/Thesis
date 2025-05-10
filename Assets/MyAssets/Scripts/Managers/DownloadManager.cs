using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using static System.Net.WebRequestMethods;
using static OVRPlugin;

    public class DownloadManager : MonoBehaviour
    {
#if UNITY_EDITOR
    public static string AppBaseDirectory = Path.GetFullPath("./Files");   // Path.GetFullPath(".") it's the path to the main directory, at the same level of the Assets folder
#else
    public static string AppBaseDirectory = Path.Combine(Application.persistentDataPath, "Files");
#endif

    public static IEnumerator DownloadAndSaveMediaElement(int mediaId, Action<string> onDownloadCompleted, string extension)
    {
        string url = "https://api.test.xrv.app/Api/App/Media/" + mediaId + "/SignedUriPublic";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return new WaitForSeconds(.1f);
        }
        if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError)
        {
            onDownloadCompleted?.Invoke(null);
        }
        else
        {
            byte[] bytes = www.downloadHandler.data;
            string path = SaveDataLocally(bytes, $"{mediaId}.{extension}");
            onDownloadCompleted?.Invoke(path);
        }
    }

    public static string SaveDataLocally(byte[] data, string filename)
    {
        string path = GetLoacalPath(filename);
        try
        {
            System.IO.File.WriteAllBytes(path, data);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return path;
    }

    public static string GetLoacalPath(string fileName, string extension)
    {
        return Path.Combine(AppBaseDirectory, $"{fileName}.{extension}");
    }
    
    public static string GetLoacalPath(string fileName)
    {
        return Path.Combine(AppBaseDirectory, $"{fileName}");
    }
}
