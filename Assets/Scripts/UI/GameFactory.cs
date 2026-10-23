using UnityEngine;

public class GameFactory : MonoBehaviour
{
    [SerializeField] private ContactCardView _contactCardPrefab;

    public float CardHeight => _contactCardPrefab != null ? ((RectTransform)_contactCardPrefab.transform).rect.height : 120f;

    public ContactCardView CreateContactCard(Transform parent)
    {
        ContactCardView card = Instantiate(_contactCardPrefab, parent, false);
        return card;
    }
}
