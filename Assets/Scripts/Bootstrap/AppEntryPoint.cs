using System.Collections.Generic;
using UnityEngine;

public class AppEntryPoint
{
    private readonly GameFactory _gameFactory;
    private readonly MainScreen _mainScreen;
    private readonly GameObject _mainPage;
    private readonly FavoritesScreen _favoritesScreen;
    private readonly GameObject _favoritesPage;
    private readonly ProfileScreen _profileScreen;
    private readonly GameObject _profilePage;
    private readonly HashSet<int> _favoriteIds = new HashSet<int>();
    private const string FavoritesKey = "favorite_contact_ids";
    private bool _returnToFavorites;

    public AppEntryPoint(GameFactory gameFactory, MainScreen mainScreen, GameObject mainPage, FavoritesScreen favoritesScreen, GameObject favoritesPage, ProfileScreen profileScreen, GameObject profilePage)
    {
        _gameFactory = gameFactory;
        _mainScreen = mainScreen;
        _mainPage = mainPage;
        _favoritesScreen = favoritesScreen;
        _favoritesPage = favoritesPage;
        _profileScreen = profileScreen;
        _profilePage = profilePage;
    }

    public void Run()
    {
        LoadFavorites();
        _mainScreen.FavoriteRequested += OnFavoriteRequested;
        _mainScreen.ProfileRequested += OnProfileRequested;
        _mainScreen.FavoritesRequested += OnFavoritesRequested;
        _favoritesScreen.HomeRequested += OnHomeRequested;
        _favoritesScreen.FavoriteRequested += OnFavoriteRequested;
        _favoritesScreen.ProfileRequested += OnProfileRequested;
        _profileScreen.FavoriteRequested += OnFavoriteRequested;
        _profileScreen.BackRequested += OnBackRequested;
        _favoritesPage.SetActive(false);
        _profilePage.SetActive(false);
        _mainPage.SetActive(true);
        _mainScreen.ShowPreviewCards(_gameFactory, 15);
        foreach (int contactId in _favoriteIds)
            _mainScreen.SetFavoriteState(contactId, true);
    }

    public void Dispose()
    {
        _mainScreen.FavoriteRequested -= OnFavoriteRequested;
        _mainScreen.ProfileRequested -= OnProfileRequested;
        _mainScreen.FavoritesRequested -= OnFavoritesRequested;
        _favoritesScreen.HomeRequested -= OnHomeRequested;
        _favoritesScreen.FavoriteRequested -= OnFavoriteRequested;
        _favoritesScreen.ProfileRequested -= OnProfileRequested;
        _profileScreen.FavoriteRequested -= OnFavoriteRequested;
        _profileScreen.BackRequested -= OnBackRequested;
    }

    private void OnFavoriteRequested(int contactId)
    {
        bool isFavorite = _favoriteIds.Add(contactId);
        if (!isFavorite)
            _favoriteIds.Remove(contactId);

        SaveFavorites();

        _mainScreen.SetFavoriteState(contactId, isFavorite);
        _profileScreen.SetFavorite(isFavorite);
        if (_favoritesPage.activeSelf)
            _favoritesScreen.ShowFavorites(_gameFactory, _favoriteIds);
    }

    private void OnProfileRequested(int contactId)
    {
        _returnToFavorites = _favoritesPage.activeSelf;
        _mainPage.SetActive(false);
        _favoritesPage.SetActive(false);
        _profilePage.SetActive(true);
        _profileScreen.ShowPreview(contactId, _favoriteIds.Contains(contactId));
    }

    private void OnBackRequested()
    {
        _profilePage.SetActive(false);
        if (_returnToFavorites)
            ShowFavoritesPage();
        else
            _mainPage.SetActive(true);
    }

    private void OnFavoritesRequested()
    {
        _mainPage.SetActive(false);
        ShowFavoritesPage();
    }

    private void OnHomeRequested()
    {
        _favoritesPage.SetActive(false);
        _mainPage.SetActive(true);
    }

    private void ShowFavoritesPage()
    {
        _favoritesPage.SetActive(true);
        _favoritesScreen.ShowFavorites(_gameFactory, _favoriteIds);
    }

    private void LoadFavorites()
    {
        string json = PlayerPrefs.GetString(FavoritesKey, string.Empty);
        if (string.IsNullOrEmpty(json))
            return;

        FavoriteIdsData data = JsonUtility.FromJson<FavoriteIdsData>(json);
        if (data?.ids == null)
            return;

        foreach (int contactId in data.ids)
            _favoriteIds.Add(contactId);
    }

    private void SaveFavorites()
    {
        FavoriteIdsData data = new FavoriteIdsData { ids = new int[_favoriteIds.Count] };
        _favoriteIds.CopyTo(data.ids);
        PlayerPrefs.SetString(FavoritesKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class FavoriteIdsData
    {
        public int[] ids;
    }
}
