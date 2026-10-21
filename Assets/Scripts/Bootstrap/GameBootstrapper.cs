using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private GameFactory _gameFactory;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private GameObject _mainPage;
    [SerializeField] private FavoritesScreen _favoritesScreen;
    [SerializeField] private GameObject _favoritesPage;
    [SerializeField] private ProfileScreen _profileScreen;
    [SerializeField] private GameObject _profilePage;

    private AppEntryPoint _entryPoint;

    private void Awake()
    {
        if (_gameFactory == null || _mainScreen == null || _mainPage == null ||
            _favoritesScreen == null || _favoritesPage == null || _profileScreen == null || _profilePage == null)
        {
            Debug.LogError("GameBootstrapper: screen and factory references must be assigned.", this);
            enabled = false;
            return;
        }

        _entryPoint = new AppEntryPoint(_gameFactory, _mainScreen, _mainPage,
            _favoritesScreen, _favoritesPage, _profileScreen, _profilePage);
    }

    private void Start()
    {
        _entryPoint.Run();
    }

    private void OnDestroy()
    {
        _entryPoint?.Dispose();
    }
}
