using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactCardView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _emailText;
    [SerializeField] private TMP_Text _ipText;
    [SerializeField] private Image _avatarImage;
    [SerializeField] private Button _addFavoritesButton;
    [SerializeField] private Sprite _favoriteAddedSprite;
    [SerializeField] private Sprite _favoriteNotAddedSprite;
    [SerializeField] private Button _profileButton;

    private int _contactId;
    private Sprite _placeholderAvatar;

    public event Action<int> FavoriteClicked;
    public event Action<int> ProfileClicked;

    private void Awake()
    {
        if (_avatarImage != null)
            _placeholderAvatar = _avatarImage.sprite;

        if (_addFavoritesButton != null)
            _addFavoritesButton.onClick.AddListener(OnFavoriteClicked);

        if (_profileButton != null)
            _profileButton.onClick.AddListener(OnProfileClicked);
    }

    public void ShowPreview(int number)
    {
        _contactId = number;

        if (_nameText != null)
            _nameText.text = $"Contact {number:00}";
        if (_emailText != null)
            _emailText.text = $"contact{number:00}@example.com";
        if (_ipText != null)
            _ipText.text = "—";

        SetFavorite(false);
        SetAvatar(null);
    }

    public void SetFavorite(bool isFavorite)
    {
        if (_addFavoritesButton != null && _addFavoritesButton.image != null)
            _addFavoritesButton.image.sprite = isFavorite ? _favoriteAddedSprite : _favoriteNotAddedSprite;
    }

    public void SetAvatar(Sprite avatar)
    {
        if (_avatarImage != null)
            _avatarImage.sprite = avatar != null ? avatar : _placeholderAvatar;
    }

    private void OnFavoriteClicked()
    {
        FavoriteClicked?.Invoke(_contactId);
    }

    private void OnProfileClicked()
    {
        ProfileClicked?.Invoke(_contactId);
    }

    private void OnDestroy()
    {
        if (_addFavoritesButton != null)
            _addFavoritesButton.onClick.RemoveListener(OnFavoriteClicked);

        if (_profileButton != null)
            _profileButton.onClick.RemoveListener(OnProfileClicked);
    }
}
