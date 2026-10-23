using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ContactRepository
{
    private const string DataUrl = "https://drive.usercontent.google.com/download?id=1YvE6Y5-vxVWXrrYyb83ssqMfVUUkndLw&export=download";

    private readonly string _cachePath = Path.Combine(Application.persistentDataPath, "contacts.json");
    private readonly Dictionary<int, Contact> _contactsById = new Dictionary<int, Contact>();
    private readonly List<Contact> _contacts = new List<Contact>();

    public IReadOnlyList<Contact> Contacts => _contacts;
    public ContactLoadResult LastResult { get; private set; }

    public bool TryGet(int contactId, out Contact contact)
    {
        return _contactsById.TryGetValue(contactId, out contact);
    }

    public IEnumerator Load()
    {
        LastResult = ContactLoadResult.None;
        if (File.Exists(_cachePath))
        {
            bool loadedFromCache = false;
            try
            {
                loadedFromCache = Parse(File.ReadAllText(_cachePath));
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"ContactRepository: cache read failed: {exception.Message}");
            }

            if (loadedFromCache)
            {
                LastResult = ContactLoadResult.Success;
                yield break;
            }
        }

        using (UnityWebRequest request = UnityWebRequest.Get(DataUrl))
        {
            request.timeout = 60;
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                LastResult = ContactLoadResult.DownloadFailed;
                Debug.LogError($"ContactRepository: download failed: {request.error}");
                yield break;
            }

            string json = request.downloadHandler.text;
            if (!Parse(json))
            {
                LastResult = ContactLoadResult.InvalidData;
                Debug.LogError("ContactRepository: downloaded file is not valid contact JSON.");
                yield break;
            }

            try
            {
                File.WriteAllText(_cachePath, json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"ContactRepository: cache write failed: {exception.Message}");
            }

            LastResult = ContactLoadResult.Success;
        }
    }

    private bool Parse(string json)
    {
        ContactData loaded;
        try
        {
            loaded = JsonUtility.FromJson<ContactData>(json);
        }
        catch (Exception)
        {
            return false;
        }

        if (loaded?.data == null || loaded.data.Length == 0)
            return false;

        _contacts.Clear();
        _contactsById.Clear();
        foreach (Contact contact in loaded.data)
        {
            if (contact == null || contact.id <= 0 || _contactsById.ContainsKey(contact.id))
                continue;

            _contacts.Add(contact);
            _contactsById.Add(contact.id, contact);
        }

        return _contacts.Count > 0;
    }
}

public enum ContactLoadResult
{
    None,
    Success,
    DownloadFailed,
    InvalidData
}
