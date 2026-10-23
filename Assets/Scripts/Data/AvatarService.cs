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

    public event Action<int> AvatarLoaded;

    public Sprite GetSprite(int slot)
    {
        return slot >= 0 && slot < AvatarCount ? _sprites[slot] : null;
    }

    public IEnumerator Load()
    {
        Directory.CreateDirectory(_directory);
        UnityWebRequest[] requests = new UnityWebRequest[AvatarCount];
        bool[] completed = new bool[AvatarCount];
        bool[] usingFallback = new bool[AvatarCount];
        int remaining = AvatarCount;

        try
        {
            for (int slot = 0; slot < AvatarCount; slot++)
            {
                string path = GetPath(slot);
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
                {
                    completed[slot] = true;
                    remaining--;
                    AvatarLoaded?.Invoke(slot);
                    continue;
                }

                requests[slot] = StartRequest($"https://picsum.photos/200?random={Guid.NewGuid():N}");
            }

            while (remaining > 0)
            {
                for (int slot = 0; slot < AvatarCount; slot++)
                {
                    UnityWebRequest request = requests[slot];
                    if (completed[slot] || request == null || !request.isDone)
                        continue;

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        byte[] bytes = request.downloadHandler.data;
                        _sprites[slot] = CreateSprite(bytes);
                        if (_sprites[slot] != null)
                        {
                            SaveAvatar(GetPath(slot), bytes, slot);
                            AvatarLoaded?.Invoke(slot);
                        }
                    }

                    request.Dispose();
                    requests[slot] = null;

                    if (_sprites[slot] != null || usingFallback[slot])
                    {
                        completed[slot] = true;
                        remaining--;
                        if (_sprites[slot] == null)
                            Debug.LogWarning($"AvatarService: avatar {slot} could not be downloaded; placeholder will be shown.");
                        continue;
                    }

                    usingFallback[slot] = true;
                    int portraitIndex = UnityEngine.Random.Range(slot * 19, slot * 19 + 19);
                    string gender = slot % 2 == 0 ? "women" : "men";
                    requests[slot] = StartRequest($"https://randomuser.me/api/portraits/{gender}/{portraitIndex}.jpg");
                }

                if (remaining > 0)
                    yield return null;
            }
        }
        finally
        {
            foreach (UnityWebRequest request in requests)
                request?.Dispose();
        }
    }

    private UnityWebRequest StartRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 12;
        request.SendWebRequest();
        return request;
    }

    private string GetPath(int slot)
    {
        return Path.Combine(_directory, $"avatar_{slot}.jpg");
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
            if (texture.LoadImage(bytes, true))
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
