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

    private int _contactId;
    private Sprite _placeholderIcon;

    public event Action<int> FavoriteRequested;

    private void Awake()
    {
        _placeholderIcon = _icon.sprite;
        _addFavoritesButton.onClick.AddListener(OnFavoriteClicked);
    }

    public void Show(Contact contact, bool isFavorite, Sprite avatar)
    {
        _contactId = contact.id;
        _nameText.text = contact.FullName;
        _genderText.text = $"Gender: {contact.gender}";
        _emailText.text = contact.email;
        _ipText.text = contact.ip_address;

        SetIcon(avatar);
        SetFavorite(isFavorite);
    }

    public void SetIcon(Sprite avatar)
    {
        _icon.sprite = avatar != null ? avatar : _placeholderIcon;
    }

    public void SetFavorite(bool isFavorite)
    {
        _addFavoritesButton.image.sprite = isFavorite ? _favoriteAddedSprite : _favoriteNotAddedSprite;
    }

    private void OnFavoriteClicked()
    {
        FavoriteRequested?.Invoke(_contactId);
    }

    private void OnDestroy()
    {
        _addFavoritesButton.onClick.RemoveListener(OnFavoriteClicked);
    }
}
