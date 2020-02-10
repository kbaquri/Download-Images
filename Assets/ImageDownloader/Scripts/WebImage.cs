using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebImage : MonoBehaviour
{
    public string url = "https://image.shutterstock.com/image-photo/sebechleby-slovakia-january-6-2015-260nw-243797071.jpg";
    public bool cache = true;
    private Image image;

    private void Awake()
    {
        if (gameObject.GetComponent<Image>() != null)
        {
            image = gameObject.GetComponent<Image>();
        }
        else
        {
            image = gameObject.AddComponent<Image>();
        }
    }
    private void Start()
    {
        ImageData imageData = new ImageData(image, url, cache);
        DownloadImage.Instance.GetImage(imageData);
    }
}
