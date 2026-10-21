using UnityEngine;

public class GameFactory : MonoBehaviour
{
    [SerializeField] private ContactCardView _contactCardPrefab;

    public ContactCardView CreateContactCard(Transform parent, int number)
    {
        if (_contactCardPrefab == null)
        {
            Debug.LogError("GameFactory: ContactCardView prefab is not assigned.", this);
            return null;
        }

        ContactCardView card = Instantiate(_contactCardPrefab, parent, false);
        card.ShowPreview(number);
        return card;
    }
}
