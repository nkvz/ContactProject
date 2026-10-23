using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactCardView : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _emailText;
    [SerializeField] private TMP_Text _ipText;
    [SerializeField] private Image _avatarImage;
    [SerializeField] private Button _addFavoritesButton;
    [SerializeField] private Image _favoriteImage;
    [SerializeField] private Sprite _favoriteAddedSprite;
    [SerializeField] private Sprite _favoriteNotAddedSprite;
    [SerializeField] private Button _profileButton;

    private int _contactId;
    private Sprite _placeholderAvatar;

    public int ContactId => _contactId;

    public event Action<int> FavoriteClicked;
    public event Action<int> ProfileClicked;

    private void Awake()
    {
        _placeholderAvatar = _avatarImage.sprite;
        _addFavoritesButton.onClick.AddListener(OnFavoriteClicked);
        _profileButton.onClick.AddListener(OnProfileClicked);
    }

    public void Show(Contact contact, bool isFavorite, Sprite avatar, bool showBackground)
    {
        _contactId = contact.id;
        _backgroundImage.enabled = showBackground;
        _nameText.text = contact.FullName;
        _emailText.text = contact.email;
        _ipText.text = contact.ip_address;
        
        SetFavorite(isFavorite);
        SetAvatar(avatar);
    }

    public void SetFavorite(bool isFavorite)
    {
        _favoriteImage.sprite = isFavorite ? _favoriteAddedSprite : _favoriteNotAddedSprite;
    }

    public void SetAvatar(Sprite avatar)
    {
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
        _addFavoritesButton.onClick.RemoveListener(OnFavoriteClicked);
        _profileButton.onClick.RemoveListener(OnProfileClicked);
    }
}
