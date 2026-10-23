using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppEntryPoint
{
    private readonly GameFactory _gameFactory;
    private readonly MainScreen _mainScreen;
    private readonly ProfileScreen _profileScreen;
    private readonly FavoritesScreen _favoritesScreen;
    private readonly UIController _uiController;
    private readonly ContactRepository _contacts = new ContactRepository();
    private readonly AvatarService _avatars = new AvatarService();
    private readonly AvatarAssignmentStore _avatarAssignments = new AvatarAssignmentStore();
    private readonly HashSet<int> _favoriteIds = new HashSet<int>();
    private readonly List<Contact> _favoriteContacts = new List<Contact>();

    private bool _returnToFavorites;
    private int _currentProfileId;

    public AppEntryPoint(GameFactory gameFactory, MainScreen mainScreen, ProfileScreen profileScreen, FavoritesScreen favoritesScreen, UIController uiController)
    {
        _gameFactory = gameFactory;
        _mainScreen = mainScreen;
        _profileScreen = profileScreen;
        _favoritesScreen = favoritesScreen;
        _uiController = uiController;
    }

    public IEnumerator Initialize()
    {
        LoadFavorites();
        _mainScreen.FavoriteRequested += OnFavoriteRequested;
        _mainScreen.ProfileRequested += OnProfileRequested;
        _favoritesScreen.FavoriteRequested += OnFavoriteRequested;
        _favoritesScreen.ProfileRequested += OnProfileRequested;
        _profileScreen.FavoriteRequested += OnFavoriteRequested;
        _uiController.HomeRequested += OnHomeRequested;
        _uiController.FavoritesRequested += OnFavoritesRequested;
        _uiController.BackRequested += OnBackRequested;

        _uiController.ShowMain();

        yield return _contacts.Load();
        _avatarAssignments.Initialize(_contacts.Contacts);
        _mainScreen.ShowContacts(_gameFactory, _contacts.Contacts, GetAvatar, IsFavorite);

        if (_uiController.IsFavoritesVisible)
            RefreshFavorites();

        yield return _avatars.Load();
        _mainScreen.RefreshVisible();
        _favoritesScreen.RefreshVisible();

        if (_uiController.IsProfileVisible && _contacts.TryGet(_currentProfileId, out Contact contact))
            _profileScreen.SetIcon(GetAvatar(contact.id));
    }

    public void Dispose()
    {
        _mainScreen.FavoriteRequested -= OnFavoriteRequested;
        _mainScreen.ProfileRequested -= OnProfileRequested;
        _favoritesScreen.FavoriteRequested -= OnFavoriteRequested;
        _favoritesScreen.ProfileRequested -= OnProfileRequested;
        _profileScreen.FavoriteRequested -= OnFavoriteRequested;
        _uiController.HomeRequested -= OnHomeRequested;
        _uiController.FavoritesRequested -= OnFavoritesRequested;
        _uiController.BackRequested -= OnBackRequested;
    }

    private Sprite GetAvatar(int contactId)
    {
        return _avatars.GetSprite(_avatarAssignments.GetSlot(contactId));
    }

    private bool IsFavorite(int contactId)
    {
        return _favoriteIds.Contains(contactId);
    }

    private void OnFavoriteRequested(int contactId)
    {
        if (!_contacts.TryGet(contactId, out Contact contact))
            return;

        bool isFavorite = _favoriteIds.Add(contact.id);
        if (!isFavorite)
            _favoriteIds.Remove(contact.id);

        SaveFavorites();
        _mainScreen.SetFavoriteState(contact.id, isFavorite);
        _favoritesScreen.SetFavoriteState(contact.id, isFavorite);

        if (_uiController.IsProfileVisible && _currentProfileId == contact.id)
            _profileScreen.SetFavorite(isFavorite);

        if (_uiController.IsFavoritesVisible)
            RefreshFavorites();
    }

    private void OnProfileRequested(int contactId)
    {
        if (!_contacts.TryGet(contactId, out Contact contact))
            return;

        _currentProfileId = contact.id;
        _returnToFavorites = _uiController.IsFavoritesVisible;
        _uiController.ShowProfile();
        _profileScreen.Show(contact, IsFavorite(contact.id), GetAvatar(contact.id));
    }

    private void OnBackRequested()
    {
        if (_returnToFavorites)
            ShowFavoritesPage();
        else
            OnHomeRequested();
    }

    private void OnFavoritesRequested()
    {
        ShowFavoritesPage();
    }

    private void OnHomeRequested()
    {
        _uiController.ShowMain();
        _mainScreen.RefreshVisible();
    }

    private void ShowFavoritesPage()
    {
        _uiController.ShowFavorites();
        RefreshFavorites();
    }

    private void RefreshFavorites()
    {
        _favoriteContacts.Clear();
        foreach (Contact contact in _contacts.Contacts)
        {
            if (_favoriteIds.Contains(contact.id))
                _favoriteContacts.Add(contact);
        }

        _favoritesScreen.ShowFavorites(_gameFactory, _favoriteContacts, GetAvatar, IsFavorite);
    }

    private void LoadFavorites()
    {
        string json = PlayerPrefs.GetString(AppDataReset.FavoritesKey, string.Empty);
        if (string.IsNullOrEmpty(json))
            return;

        try
        {
            FavoriteIdsData data = JsonUtility.FromJson<FavoriteIdsData>(json);
            if (data?.ids == null)
                return;

            foreach (int contactId in data.ids)
                _favoriteIds.Add(contactId);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"AppEntryPoint: favorites cache is invalid: {exception.Message}");
        }
    }

    private void SaveFavorites()
    {
        FavoriteIdsData data = new FavoriteIdsData { ids = new int[_favoriteIds.Count] };
        _favoriteIds.CopyTo(data.ids);
        PlayerPrefs.SetString(AppDataReset.FavoritesKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    [Serializable]
    private class FavoriteIdsData
    {
        public int[] ids;
    }
}
