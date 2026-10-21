using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FavoritesScreen : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Button _homeButton;
    [SerializeField, Min(0)] private float _sidePadding = 24f;
    [SerializeField, Min(0)] private float _topPadding = 16f;
    [SerializeField, Min(0)] private float _cardSpacing = 16f;

    private readonly List<ContactCardView> _cards = new List<ContactCardView>();

    public event Action HomeRequested;
    public event Action<int> FavoriteRequested;
    public event Action<int> ProfileRequested;

    private void Awake()
    {
        if (_homeButton != null)
            _homeButton.onClick.AddListener(OnHomeClicked);
    }

    public void ShowFavorites(GameFactory factory, IEnumerable<int> favoriteIds)
    {
        if (_scrollRect == null || _scrollRect.content == null || factory == null)
        {
            Debug.LogError("FavoritesScreen: ScrollRect, Content and GameFactory must be assigned.", this);
            return;
        }

        ClearCards();

        List<int> sortedIds = new List<int>(favoriteIds);
        sortedIds.Sort();

        RectTransform content = _scrollRect.content;
        float y = _topPadding;

        foreach (int contactId in sortedIds)
        {
            ContactCardView card = factory.CreateContactCard(content, contactId);
            if (card == null)
                break;

            card.SetFavorite(true);
            card.FavoriteClicked += OnFavoriteClicked;
            card.ProfileClicked += OnProfileClicked;
            _cards.Add(card);

            RectTransform cardRect = (RectTransform)card.transform;
            float height = cardRect.sizeDelta.y;
            cardRect.anchorMin = new Vector2(0f, 1f);
            cardRect.anchorMax = new Vector2(1f, 1f);
            cardRect.pivot = new Vector2(0.5f, 1f);
            cardRect.sizeDelta = new Vector2(-2f * _sidePadding, height);
            cardRect.anchoredPosition = new Vector2(0f, -y);
            y += height + _cardSpacing;
        }

        float totalHeight = _cards.Count > 0 ? y - _cardSpacing + _topPadding : 0f;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        _scrollRect.verticalNormalizedPosition = 1f;
    }

    private void ClearCards()
    {
        foreach (ContactCardView card in _cards)
        {
            if (card == null)
                continue;

            card.FavoriteClicked -= OnFavoriteClicked;
            card.ProfileClicked -= OnProfileClicked;
            card.gameObject.SetActive(false);
            Destroy(card.gameObject);
        }

        _cards.Clear();
    }

    private void OnHomeClicked()
    {
        HomeRequested?.Invoke();
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
        if (_homeButton != null)
            _homeButton.onClick.RemoveListener(OnHomeClicked);

        ClearCards();
    }
}
