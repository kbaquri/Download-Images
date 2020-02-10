# Download-Images
Design a singleton library which can be used by any class to download images from web.
Design a game scene with several images with an attached script WebImage. Webimage internally calls this DownloadImage Singleton to download the image and update its image.

Deliver:
Unity Package (.unitypackage) exported from Unity.

WebImage.cs
Expose a string url and auto set the image to downloaded image from web. If no image is downloaded because of internet or other issues, it will default to existing image.

Key Points
Game performance goes down if multiple images are downloaded at same time. Let’s enforce a policy that game can only download 3 images at one specified point of time.
Downloaded images can contain alpha.

Advance
 Cache downloaded images on file system to avoid re download.
 Invalidate downloaded cache after 7 days
 Optional Memory Caching- Expose a boolean in WebImage specifying whether image should be cached in memory or not.

Extension
If any request is waiting for more than 10 seconds, let it go immediately. Even if it violates the 3 concurrent download rule. Rules in order of importance:
 Any download request shall not wait more than 10 seconds.
 There should not be more than 3 concurrent downloads. If request is waiting more than 10 seconds, it’s okay to violate this rule.
