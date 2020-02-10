using System.Reflection;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class DownloadImage : MonoBehaviour
{
    string dirPath;
    private static DownloadImage _instance;
    private static GameObject instanceGameObject;

    Dictionary<Image, ImageData> downloadList = new Dictionary<Image, ImageData>();
    static int count = 0;

    private static System.DateTime startDate;
    private static System.DateTime today;

    public static DownloadImage Instance
    {
        get
        {
            if (_instance == null)
            {
                instanceGameObject = new GameObject("Download Image");
                instanceGameObject.AddComponent<DownloadImage>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        //Initialize Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(GetComponent<DownloadImage>());
        }
        else
        {
            _instance = this;
        }

        //Initialize Cache Directory Path
        dirPath = Application.persistentDataPath + "/CacheImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        //Clear Cache after 7 days
        SetStartDate();
        if (GetDaysPassed() > 7)
        {
            DeleteFolder();
            ResetStartDate();
        }
    }

    private void OnDestroy() { if (this == _instance) { _instance = null; } }

    public void GetImage(ImageData imageData)
    {
        if (count < 3)
            StartCoroutine(Image(imageData));
        else
            StartCoroutine(SaveForLater(imageData));
    }

    //Download or Load Image from cache
    IEnumerator Image(ImageData imageData)
    {
        // Debug.Log(image.gameObject.name + " " + count + " " + downloadList.Count);
        if (HasPNG(imageData.url))
        {
            imageData.image.sprite = LoadPNG(imageData.url);
        }
        else
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(imageData.url))
            {
                count++;
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    Debug.Log(unityWebRequest.error);
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(unityWebRequest);
                    SaveTextureAsPNG(texture, imageData.url);
                    imageData.image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                    count--;
                    if (count < 3 && downloadList.Count > 0)
                    {
                        StartCoroutine(Image(downloadList.First().Value));
                        downloadList.Remove(downloadList.First().Key);
                    }
                }
            }
        }
    }

    //Save Extra download image request more than 3
    IEnumerator SaveForLater(ImageData imageData)
    {
        downloadList.Add(imageData.image, imageData);
        yield return new WaitForSeconds(10);
        if (downloadList.ContainsKey(imageData.image))
        {
            StartCoroutine(Image(imageData));
            downloadList.Remove(imageData.image);
        }
    }

    //Save Texture in Cache
    private void SaveTextureAsPNG(Texture2D _texture, string url)
    {
        string fileName = string.Format("{0:X}.png", url.GetHashCode());
        byte[] _bytes = _texture.EncodeToPNG();
        File.WriteAllBytes(dirPath + fileName, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + dirPath + fileName);
    }

    //Check if image already exist in cache
    private bool HasPNG(string url)
    {
        string fileName = string.Format("{0:X}.png", url.GetHashCode());
        if (File.Exists(dirPath + fileName))
            return true;
        else
            return false;
    }

    //Load image from cache according to url
    private Sprite LoadPNG(string url)
    {
        string fileName = string.Format("{0:X}.png", url.GetHashCode());
        Texture2D texture = null;
        byte[] fileData;

        if (File.Exists(dirPath + fileName))
        {
            fileData = File.ReadAllBytes(dirPath + fileName);
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

        return sprite;
    }

    //Delete Cache Folder
    private void DeleteFolder()
    {
        FileUtil.DeleteFileOrDirectory(dirPath);
    }

    //Set Start date for cheching cache generation
    void SetStartDate()
    {
        if (PlayerPrefs.HasKey("DateInitialized")) //if we have the start date saved, we'll use that
            startDate = System.Convert.ToDateTime(PlayerPrefs.GetString("DateInitialized"));
        else //otherwise...
        {
            startDate = System.DateTime.Now; //save the start date ->
            PlayerPrefs.SetString("DateInitialized", startDate.ToString());
        }
    }

    //Reset Start date for cheching cache generation
    void ResetStartDate()
    {
        startDate = System.DateTime.Now; //save the start date ->
        PlayerPrefs.SetString("DateInitialized", startDate.ToString());
    }

    //Get elapsed time from start date to current date
    public double GetDaysPassed()
    {
        today = System.DateTime.Now;

        //days between today and start date -->
        System.TimeSpan elapsed = today.Subtract(startDate);

        double days = elapsed.TotalDays;
        // return days.ToString("0");
        return days;
    }

}