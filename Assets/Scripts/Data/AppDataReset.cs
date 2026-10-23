using System;
using System.IO;
using UnityEngine;

public static class AppDataReset
{
    public const string FavoritesKey = "favorite_contact_ids";

    public static bool Clear()
    {
        try
        {
            string dataPath = Application.persistentDataPath;
            DeleteFile(Path.Combine(dataPath, "contacts.json"));
            DeleteFile(Path.Combine(dataPath, "avatar_assignments.json"));

            string avatarsPath = Path.Combine(dataPath, "avatars");
            if (Directory.Exists(avatarsPath))
                Directory.Delete(avatarsPath, true);

            PlayerPrefs.DeleteKey(FavoritesKey);
            PlayerPrefs.Save();
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"AppDataReset: data reset failed: {exception.Message}");
            return false;
        }
    }

    private static void DeleteFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}
