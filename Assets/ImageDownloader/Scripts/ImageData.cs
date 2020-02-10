using System;
using UnityEngine.UI;

[Serializable]
public class ImageData
{
    public Image image;
    public string url;
    public bool cache;

    public ImageData(Image image, string url, bool cache)
    {
        this.image = image;
        this.url = url;
        this.cache = cache;
    }
}