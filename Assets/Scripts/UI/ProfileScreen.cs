using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreen : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _addFavoritesButton;
    [SerializeField] private Sprite _favoriteAddedSprite;
    [SerializeField] private Sprite _favoriteNotAddedSprite;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _genderText;
    [SerializeField] private TMP_Text _emailText;
    [SerializeField] private TMP_Text _ipText;
    [SerializeField] private Button _backButton;

    private int _contactId;
    private Sprite _placeholderIcon;

    public event Action<int> FavoriteRequested;
    public event Action BackRequested;

    private void Awake()
    {
        if (_icon != null)
            _placeholderIcon = _icon.sprite;

        if (_addFavoritesButton != null)
            _addFavoritesButton.onClick.AddListener(OnFavoriteClicked);

        if (_backButton != null)
            _backButton.onClick.AddListener(OnBackClicked);
    }

    public void ShowPreview(int contactId, bool isFavorite, Sprite avatar = null)
    {
        _contactId = contactId;

        if (_nameText != null)
            _nameText.text = $"Contact {contactId:00}";
        if (_genderText != null)
            _genderText.text = "Gender: Male";
        if (_emailText != null)
            _emailText.text = $"contact{contactId:00}@example.com";
        if (_ipText != null)
            _ipText.text = "—";

        SetIcon(avatar);
        SetFavorite(isFavorite);
    }

    public void SetIcon(Sprite avatar)
    {
        if (_icon != null)
            _icon.sprite = avatar != null ? avatar : _placeholderIcon;
    }

    public void SetFavorite(bool isFavorite)
    {
        if (_addFavoritesButton != null && _addFavoritesButton.image != null)
            _addFavoritesButton.image.sprite = isFavorite ? _favoriteAddedSprite : _favoriteNotAddedSprite;
    }

    private void OnFavoriteClicked()
    {
        FavoriteRequested?.Invoke(_contactId);
    }

    private void OnBackClicked()
    {
        BackRequested?.Invoke();
    }

    private void OnDestroy()
    {
        if (_addFavoritesButton != null)
            _addFavoritesButton.onClick.RemoveListener(OnFavoriteClicked);

        if (_backButton != null)
            _backButton.onClick.RemoveListener(OnBackClicked);
    }
}
