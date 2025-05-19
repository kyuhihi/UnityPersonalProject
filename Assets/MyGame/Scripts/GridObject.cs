using UnityEngine;

public class GridObject : MonoBehaviour
{
    private Vector2Int _currentCell;

    private void Start()
    {
        _currentCell = GridManager.Instance.GetCell(transform.position);
        GridManager.Instance.Register(gameObject);
    }

    private void LateUpdate()
    {
        Vector2Int newCell = GridManager.Instance.GetCell(transform.position);
        if (newCell != _currentCell)
        {
            // ¼¿ À§Ä¡°¡ ¹Ù²ð ¶§¸¸ Unregister/ Register
            GridManager.Instance.MoveObject(gameObject, _currentCell, newCell);
            _currentCell = newCell;
        }
    }

    private void OnDisable()
    {
        GridManager.Instance.Unregister(gameObject);
    }

    // ¸Ó¸® À§¿¡ ¼¿ ÁÂÇ¥ Ç¥½Ã
    private void OnDrawGizmos()
    {
        Vector3 headPos = transform.position + Vector3.up * 2f;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.yellow;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(headPos, $"Cell: {_currentCell}", style);
#endif
    }
}
