using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContactScrollList
{
    private readonly ScrollRect _scrollRect;
    private readonly GameFactory _factory;
    private readonly Func<int, Sprite> _avatarForContact;
    private readonly Func<int, bool> _isFavorite;
    private readonly float _sidePadding;
    private readonly float _topPadding;
    private readonly float _cardSpacing;
    private readonly List<ContactCardView> _pool = new List<ContactCardView>();

    private IReadOnlyList<Contact> _contacts = Array.Empty<Contact>();
    private float _cardHeight;
    private int _firstVisibleIndex = -1;

    public event Action<int> FavoriteClicked;
    public event Action<int> ProfileClicked;

    public ContactScrollList(ScrollRect scrollRect, GameFactory factory, Func<int, Sprite> avatarForContact, Func<int, bool> isFavorite, float sidePadding, float topPadding, float cardSpacing)
    {
        _scrollRect = scrollRect;
        _factory = factory;
        _avatarForContact = avatarForContact;
        _isFavorite = isFavorite;
        _sidePadding = sidePadding;
        _topPadding = topPadding;
        _cardSpacing = cardSpacing;
        _cardHeight = Mathf.Max(1f, factory.CardHeight);
        _scrollRect.onValueChanged.AddListener(OnScroll);
    }

    public void SetContacts(IReadOnlyList<Contact> contacts, bool resetScroll)
    {
        _contacts = contacts ?? Array.Empty<Contact>();
        RectTransform content = _scrollRect.content;
        float height = _topPadding * 2f + _contacts.Count * _cardHeight + Mathf.Max(0, _contacts.Count - 1) * _cardSpacing;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        if (resetScroll)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0f);
            _scrollRect.verticalNormalizedPosition = 1f;
        }

        Canvas.ForceUpdateCanvases();
        if (!resetScroll)
        {
            float viewportHeight = _scrollRect.viewport != null ? _scrollRect.viewport.rect.height : ((RectTransform)_scrollRect.transform).rect.height;
            float maxScroll = Mathf.Max(0f, height - viewportHeight);
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, Mathf.Clamp(content.anchoredPosition.y, 0f, maxScroll));
        }

        EnsurePool();
        Refresh(true);
    }

    public void RefreshVisible()
    {
        EnsurePool();
        Refresh(true);
    }

    public void SetFavoriteState(int contactId, bool isFavorite)
    {
        foreach (ContactCardView card in _pool)
        {
            if (card.gameObject.activeSelf && card.ContactId == contactId)
                card.SetFavorite(isFavorite);
        }
    }

    public void Dispose()
    {
        _scrollRect.onValueChanged.RemoveListener(OnScroll);
        foreach (ContactCardView card in _pool)
        {
            if (card == null)
                continue;

            card.FavoriteClicked -= OnFavoriteClicked;
            card.ProfileClicked -= OnProfileClicked;
        }
    }

    private void EnsurePool()
    {
        float stride = _cardHeight + _cardSpacing;
        float viewportHeight = _scrollRect.viewport != null ? _scrollRect.viewport.rect.height : ((RectTransform)_scrollRect.transform).rect.height;
        int needed = Mathf.Min(_contacts.Count, Mathf.CeilToInt(viewportHeight / stride) + 4);

        while (_pool.Count < needed)
        {
            ContactCardView card = _factory.CreateContactCard(_scrollRect.content);
            if (card == null)
                break;

            card.FavoriteClicked += OnFavoriteClicked;
            card.ProfileClicked += OnProfileClicked;
            _pool.Add(card);
        }
    }

    private void Refresh(bool force)
    {
        float stride = _cardHeight + _cardSpacing;
        float scrollY = Mathf.Max(0f, _scrollRect.content.anchoredPosition.y);
        int first = Mathf.Clamp(Mathf.FloorToInt((scrollY - _topPadding) / stride) - 1, 0, Mathf.Max(0, _contacts.Count - 1));

        if (!force && first == _firstVisibleIndex)
            return;

        _firstVisibleIndex = first;
        for (int poolIndex = 0; poolIndex < _pool.Count; poolIndex++)
        {
            int contactIndex = first + poolIndex;
            ContactCardView card = _pool[poolIndex];
            if (contactIndex >= _contacts.Count)
            {
                card.gameObject.SetActive(false);
                continue;
            }

            Contact contact = _contacts[contactIndex];
            card.gameObject.SetActive(true);
            card.Show(contact, _isFavorite(contact.id), _avatarForContact(contact.id), contactIndex % 2 == 0);

            RectTransform rect = (RectTransform)card.transform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(-2f * _sidePadding, _cardHeight);
            rect.anchoredPosition = new Vector2(0f, -_topPadding - contactIndex * stride);
        }
    }

    private void OnScroll(Vector2 position)
    {
        Refresh(false);
    }

    private void OnFavoriteClicked(int contactId)
    {
        FavoriteClicked?.Invoke(contactId);
    }

    private void OnProfileClicked(int contactId)
    {
        ProfileClicked?.Invoke(contactId);
    }
}
