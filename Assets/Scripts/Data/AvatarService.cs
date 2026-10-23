using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarService
{
    private const int AvatarCount = 5;
    private readonly string _directory = Path.Combine(Application.persistentDataPath, "avatars");
    private readonly Sprite[] _sprites = new Sprite[AvatarCount];

    public Sprite GetSprite(int slot)
    {
        return slot >= 0 && slot < AvatarCount ? _sprites[slot] : null;
    }

    public IEnumerator Load()
    {
        Directory.CreateDirectory(_directory);

        for (int slot = 0; slot < AvatarCount; slot++)
        {
            string path = Path.Combine(_directory, $"avatar_{slot}.jpg");
            if (File.Exists(path))
            {
                try
                {
                    _sprites[slot] = CreateSprite(File.ReadAllBytes(path));
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"AvatarService: local avatar {slot} failed: {exception.Message}");
                }
            }

            if (_sprites[slot] != null)
                continue;

            string url = $"https://picsum.photos/200?random={Guid.NewGuid():N}";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 20;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                    _sprites[slot] = CreateSprite(request.downloadHandler.data);

                if (_sprites[slot] != null)
                    SaveAvatar(path, request.downloadHandler.data, slot);
            }

            if (_sprites[slot] != null)
                continue;

            int portraitIndex = UnityEngine.Random.Range(slot * 19, slot * 19 + 19);
            string gender = slot % 2 == 0 ? "women" : "men";
            string fallbackUrl = $"https://randomuser.me/api/portraits/{gender}/{portraitIndex}.jpg";
            using (UnityWebRequest request = UnityWebRequest.Get(fallbackUrl))
            {
                request.timeout = 20;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                    _sprites[slot] = CreateSprite(request.downloadHandler.data);

                if (_sprites[slot] != null)
                    SaveAvatar(path, request.downloadHandler.data, slot);
                else
                    Debug.LogWarning($"AvatarService: avatar {slot} could not be downloaded; placeholder will be shown.");
            }
        }
    }

    private void SaveAvatar(string path, byte[] bytes, int slot)
    {
        try
        {
            File.WriteAllBytes(path, bytes);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"AvatarService: avatar {slot} cache write failed: {exception.Message}");
        }
    }

    private Sprite CreateSprite(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return null;

        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        try
        {
            if (texture.LoadImage(bytes))
            {
                texture.name = "ContactAvatar";
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f), 100f);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"AvatarService: image decode failed: {exception.Message}");
        }

        if (texture != null)
            UnityEngine.Object.Destroy(texture);
        return null;
    }
}
