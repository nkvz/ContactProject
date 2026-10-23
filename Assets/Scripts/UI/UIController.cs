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

    private GameObject _contactsStatusPanel;
    private TMP_Text _contactsStatusText;
    private Button _retryContactsButton;

    public event Action HomeRequested;
    public event Action FavoritesRequested;
    public event Action BackRequested;
    public event Action ResetDataRequested;
    public event Action RetryContactsRequested;

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
        CreateContactsStatusPanel();
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

    public void ShowContactsLoading()
    {
        _contactsStatusPanel.SetActive(true);
        _contactsStatusPanel.transform.SetAsLastSibling();
        _contactsStatusText.text = "Loading employee data...";
        _retryContactsButton.gameObject.SetActive(false);
    }

    public void ShowContactsError(string message)
    {
        _contactsStatusPanel.SetActive(true);
        _contactsStatusPanel.transform.SetAsLastSibling();
        _contactsStatusText.text = message;
        _retryContactsButton.gameObject.SetActive(true);
    }

    public void HideContactsLoading()
    {
        _contactsStatusPanel.SetActive(false);
    }

    private void CreateContactsStatusPanel()
    {
        Canvas canvas = _mainPage.GetComponentInParent<Canvas>();
        GameObject panel = new GameObject("ContactsStatusPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = (RectTransform)panel.transform;
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.94f);
        _contactsStatusPanel = panel;

        GameObject message = new GameObject("Message", typeof(RectTransform), typeof(TextMeshProUGUI));
        message.transform.SetParent(panel.transform, false);
        RectTransform messageRect = (RectTransform)message.transform;
        messageRect.anchorMin = new Vector2(0.1f, 0.5f);
        messageRect.anchorMax = new Vector2(0.9f, 0.5f);
        messageRect.sizeDelta = new Vector2(0f, 180f);
        messageRect.anchoredPosition = new Vector2(0f, 70f);
        _contactsStatusText = message.GetComponent<TextMeshProUGUI>();
        _contactsStatusText.fontSize = 38f;
        _contactsStatusText.alignment = TextAlignmentOptions.Center;
        _contactsStatusText.raycastTarget = false;

        GameObject retry = new GameObject("RetryButton", typeof(RectTransform), typeof(Image), typeof(Button));
        retry.transform.SetParent(panel.transform, false);
        RectTransform retryRect = (RectTransform)retry.transform;
        retryRect.anchorMin = new Vector2(0.5f, 0.5f);
        retryRect.anchorMax = new Vector2(0.5f, 0.5f);
        retryRect.sizeDelta = new Vector2(320f, 100f);
        retryRect.anchoredPosition = new Vector2(0f, -90f);
        retry.GetComponent<Image>().color = new Color(0.22f, 0.36f, 0.55f, 1f);
        _retryContactsButton = retry.GetComponent<Button>();
        _retryContactsButton.onClick.AddListener(OnRetryContactsClicked);

        GameObject label = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        label.transform.SetParent(retry.transform, false);
        RectTransform labelRect = (RectTransform)label.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        TextMeshProUGUI labelText = label.GetComponent<TextMeshProUGUI>();
        labelText.text = "RETRY";
        labelText.fontSize = 38f;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.raycastTarget = false;
        panel.SetActive(false);
    }

    private void OnRetryContactsClicked()
    {
        RetryContactsRequested?.Invoke();
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
        if (_retryContactsButton != null)
            _retryContactsButton.onClick.RemoveListener(OnRetryContactsClicked);
    }
}
