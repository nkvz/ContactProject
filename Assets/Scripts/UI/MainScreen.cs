using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Button _favoritesButton;
    [SerializeField, Min(0)] private float _sidePadding = 24f;
    [SerializeField, Min(0)] private float _topPadding = 16f;
    [SerializeField, Min(0)] private float _cardSpacing = 16f;

    private bool _initialized;
    private readonly Dictionary<int, ContactCardView> _cardsById = new Dictionary<int, ContactCardView>();

    public event Action<int> FavoriteRequested;
    public event Action<int> ProfileRequested;
    public event Action FavoritesRequested;

    private void Awake()
    {
        if (_favoritesButton != null)
            _favoritesButton.onClick.AddListener(OnFavoritesClicked);
    }

    public void ShowPreviewCards(GameFactory factory, int count)
    {
        if (_initialized)
            return;

        if (_scrollRect == null || _scrollRect.content == null)
        {
            Debug.LogError("MainScreen: ScrollRect and Content must be assigned.", this);
            return;
        }

        if (factory == null)
        {
            Debug.LogError("MainScreen: GameFactory is missing.", this);
            return;
        }

        RectTransform content = _scrollRect.content;
        float y = _topPadding;

        for (int i = 0; i < count; i++)
        {
            ContactCardView card = factory.CreateContactCard(content, i + 1);
            if (card == null)
                return;

            card.FavoriteClicked += OnFavoriteClicked;
            card.ProfileClicked += OnProfileClicked;
            _cardsById.Add(i + 1, card);

            RectTransform cardRect = (RectTransform)card.transform;
            float height = cardRect.sizeDelta.y;
            cardRect.anchorMin = new Vector2(0f, 1f);
            cardRect.anchorMax = new Vector2(1f, 1f);
            cardRect.pivot = new Vector2(0.5f, 1f);
            cardRect.sizeDelta = new Vector2(-2f * _sidePadding, height);
            cardRect.anchoredPosition = new Vector2(0f, -y);
            y += height + _cardSpacing;
        }

        float totalHeight = count > 0 ? y - _cardSpacing + _topPadding : 0f;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        _scrollRect.verticalNormalizedPosition = 1f;
        _initialized = true;
    }

    public void SetFavoriteState(int contactId, bool isFavorite)
    {
        if (_cardsById.TryGetValue(contactId, out ContactCardView card))
            card.SetFavorite(isFavorite);
    }

    private void OnFavoriteClicked(int contactId)
    {
        FavoriteRequested?.Invoke(contactId);
    }

    private void OnProfileClicked(int contactId)
    {
        ProfileRequested?.Invoke(contactId);
    }

    private void OnFavoritesClicked()
    {
        FavoritesRequested?.Invoke();
    }

    private void OnDestroy()
    {
        if (_favoritesButton != null)
            _favoritesButton.onClick.RemoveListener(OnFavoritesClicked);

        foreach (ContactCardView card in _cardsById.Values)
        {
            if (card == null)
                continue;

            card.FavoriteClicked -= OnFavoriteClicked;
            card.ProfileClicked -= OnProfileClicked;
        }
    }
}
