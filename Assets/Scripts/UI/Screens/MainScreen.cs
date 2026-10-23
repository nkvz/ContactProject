using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField, Min(0)] private float _sidePadding = 24f;
    [SerializeField, Min(0)] private float _topPadding = 16f;
    [SerializeField, Min(0)] private float _cardSpacing = 16f;

    private ContactScrollList _list;

    public event Action<int> FavoriteRequested;
    public event Action<int> ProfileRequested;

    public void ShowContacts(GameFactory factory, IReadOnlyList<Contact> contacts, Func<int, Sprite> avatarForContact, Func<int, bool> isFavorite)
    {
        if (_list == null)
        {
            _list = new ContactScrollList(_scrollRect, factory, avatarForContact, isFavorite, _sidePadding, _topPadding, _cardSpacing);
            _list.FavoriteClicked += OnFavoriteClicked;
            _list.ProfileClicked += OnProfileClicked;
        }

        _list.SetContacts(contacts, true);
    }

    public void RefreshVisible()
    {
        _list?.RefreshVisible();
    }

    public void SetFavoriteState(int contactId, bool isFavorite)
    {
        _list?.SetFavoriteState(contactId, isFavorite);
    }

    private void OnFavoriteClicked(int contactId)
    {
        FavoriteRequested?.Invoke(contactId);
    }

    private void OnProfileClicked(int contactId)
    {
        ProfileRequested?.Invoke(contactId);
    }

    private void OnDestroy()
    {
        if (_list == null)
            return;

        _list.FavoriteClicked -= OnFavoriteClicked;
        _list.ProfileClicked -= OnProfileClicked;
        _list.Dispose();
    }
}
