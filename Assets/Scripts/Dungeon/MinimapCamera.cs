using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        if (DungeonManager.Instance.CurrentRoom != null)
        {
            float x = DungeonManager.Instance.CurrentRoom.transform.position.x / 32f;
            float y = DungeonManager.Instance.CurrentRoom.transform.position.z / 32f;

            x = Mathf.RoundToInt(x);
            y = Mathf.RoundToInt(y);

            transform.position = new Vector3(x, y, -10f);
        }
    }
}
