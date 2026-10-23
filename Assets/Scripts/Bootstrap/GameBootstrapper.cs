using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private GameFactory _gameFactory;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private ProfileScreen _profileScreen;
    [SerializeField] private FavoritesScreen _favoritesScreen;
    [SerializeField] private UIController _uiController;

    private AppEntryPoint _entryPoint;

    private void Awake()
    {
        _entryPoint = new AppEntryPoint(_gameFactory, _mainScreen, _profileScreen, _favoritesScreen, _uiController);
        _uiController.ResetDataRequested += OnResetDataRequested;
    }

    private IEnumerator Start()
    {
        if (_entryPoint != null)
            yield return _entryPoint.Initialize();
    }

    private void OnDestroy()
    {
        _uiController.ResetDataRequested -= OnResetDataRequested;
        _entryPoint?.Dispose();
    }

    private void OnResetDataRequested()
    {
        if (!AppDataReset.Clear())
            return;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
