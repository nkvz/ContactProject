using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _mainPage;
    [SerializeField] private GameObject _favoritesPage;
    [SerializeField] private GameObject _profilePage;
    [SerializeField] private GameObject _navigationBar;
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _favoritesButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _resetDataButton;
    [SerializeField] private TMP_Text _topPanelText;
    [SerializeField] private Image _navLeft;
    [SerializeField] private Image _navRight;
    [SerializeField] private Sprite _spriteNavLeft;
    [SerializeField] private Sprite _spriteNavRight;

    public event Action HomeRequested;
    public event Action FavoritesRequested;
    public event Action BackRequested;
    public event Action ResetDataRequested;

    public bool IsFavoritesVisible => _favoritesPage.activeSelf;
    public bool IsProfileVisible => _profilePage.activeSelf;
    public bool IsConfigured => _mainPage != null && _favoritesPage != null && _profilePage != null && _navigationBar != null && _homeButton != null && _favoritesButton != null && _backButton != null && _topPanelText != null && _navLeft != null && _navRight != null;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        _homeButton.interactable = true;
        _favoritesButton.interactable = true;
        _homeButton.transition = Selectable.Transition.None;
        _favoritesButton.transition = Selectable.Transition.None;

        _homeButton.onClick.AddListener(OnHomeClicked);
        _favoritesButton.onClick.AddListener(OnFavoritesClicked);
        _backButton.onClick.AddListener(OnBackClicked);
        _resetDataButton.onClick.AddListener(OnResetDataClicked);
    }

    public void ShowMain()
    {
        _topPanelText.text = "EMPLOYEE DATA";
        _favoritesPage.SetActive(false);
        _profilePage.SetActive(false);
        _mainPage.SetActive(true);
        _navigationBar.SetActive(true);
        SetNavigationState(true, false);
    }

    public void ShowFavorites()
    {
        _topPanelText.text = "FAVORITE";
        _mainPage.SetActive(false);
        _profilePage.SetActive(false);
        _favoritesPage.SetActive(true);
        _navigationBar.SetActive(true);
        SetNavigationState(false, true);
    }

    public void ShowProfile()
    {
        _topPanelText.text = "PROFILE";
        _mainPage.SetActive(false);
        _favoritesPage.SetActive(false);
        _profilePage.SetActive(true);
        _navigationBar.SetActive(true);
        SetNavigationState(false, false);
    }

    private void SetNavigationState(bool isMainSelected, bool isFavoritesSelected)
    {
        _navLeft.sprite = isMainSelected ? null : _spriteNavLeft;
        _navRight.sprite = isFavoritesSelected ? null : _spriteNavRight;
        _navLeft.color = Color.white;
        _navRight.color = Color.white;
    }

    private void OnHomeClicked()
    {
        HomeRequested?.Invoke();
    }

    private void OnFavoritesClicked()
    {
        FavoritesRequested?.Invoke();
    }

    private void OnBackClicked()
    {
        BackRequested?.Invoke();
    }

    private void OnResetDataClicked()
    {
        ResetDataRequested?.Invoke();
    }

    private void OnDestroy()
    {
        _homeButton.onClick.RemoveListener(OnHomeClicked);
        _favoritesButton.onClick.RemoveListener(OnFavoritesClicked);
        _backButton.onClick.RemoveListener(OnBackClicked);
        _resetDataButton.onClick.RemoveListener(OnResetDataClicked);
    }
}
