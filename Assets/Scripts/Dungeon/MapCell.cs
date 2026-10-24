using UnityEngine;

public class MapCell : MonoBehaviour
{
    [SerializeField] private Sprite[] roomSprite;
    [SerializeField] private Sprite[] roomIcons;
    [SerializeField] private SpriteRenderer roomImage;
    [SerializeField] private SpriteRenderer iconImage;
    private int value;
    private bool showIcon = false;

    public void InitializeCell(bool north, bool east, bool south, bool west, int roomType)
    {
        if (north) value += 1;
        if (east) value += 2;
        if (south) value += 4;
        if (west) value += 8;

        roomImage.sprite = roomSprite[value];

        if (roomType > 0)
        {
            iconImage.sprite = roomIcons[roomType - 1];
            showIcon = true;
        }

        roomImage.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(false);
    }

    public void SetCurrentRoom()
    {
        roomImage.gameObject.SetActive(true);

        if (showIcon)
        {
            iconImage.gameObject.SetActive(true);
        }
    }
}
