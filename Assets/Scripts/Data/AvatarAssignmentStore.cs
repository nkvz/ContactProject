using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AvatarAssignmentStore
{
    private readonly string _path = Path.Combine(Application.persistentDataPath, "avatar_assignments.json");
    private readonly Dictionary<int, int> _slotsByContactId = new Dictionary<int, int>();

    public void Initialize(IReadOnlyList<Contact> contacts)
    {
        Load();
        bool changed = false;
        int previousSlot = -1;

        foreach (Contact contact in contacts)
        {
            if (!_slotsByContactId.TryGetValue(contact.id, out int slot))
            {
                int random = UnityEngine.Random.Range(0, 4);
                slot = previousSlot >= 0 && random >= previousSlot ? random + 1 : random;
                _slotsByContactId.Add(contact.id, slot);
                changed = true;
            }

            previousSlot = slot;
        }

        if (changed)
            Save();
    }

    public int GetSlot(int contactId)
    {
        return _slotsByContactId.TryGetValue(contactId, out int slot) ? slot : -1;
    }

    private void Load()
    {
        if (!File.Exists(_path))
            return;

        try
        {
            AvatarAssignmentData data = JsonUtility.FromJson<AvatarAssignmentData>(File.ReadAllText(_path));
            if (data?.ids == null || data.slots == null || data.ids.Length != data.slots.Length)
                return;

            for (int i = 0; i < data.ids.Length; i++)
            {
                if (data.slots[i] >= 0 && data.slots[i] < 5)
                    _slotsByContactId[data.ids[i]] = data.slots[i];
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"AvatarAssignmentStore: cache read failed: {exception.Message}");
        }
    }

    private void Save()
    {
        AvatarAssignmentData data = new AvatarAssignmentData
        {
            ids = new int[_slotsByContactId.Count],
            slots = new int[_slotsByContactId.Count]
        };

        int index = 0;
        foreach (KeyValuePair<int, int> entry in _slotsByContactId)
        {
            data.ids[index] = entry.Key;
            data.slots[index] = entry.Value;
            index++;
        }

        try
        {
            File.WriteAllText(_path, JsonUtility.ToJson(data));
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"AvatarAssignmentStore: cache write failed: {exception.Message}");
        }
    }

    [Serializable]
    private class AvatarAssignmentData
    {
        public int[] ids;
        public int[] slots;
    }
}
