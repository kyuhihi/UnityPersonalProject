using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public int _gridSizeX = 10;
    public float _cellSize = 10f;
    private Dictionary<Vector2Int, List<GameObject>> _grid = new Dictionary<Vector2Int, List<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // ������Ʈ ���
    public void Register(GameObject obj)
    {
        Vector2Int cell = GetCell(obj.transform.position);
        if (!_grid.ContainsKey(cell))
            _grid[cell] = new List<GameObject>();
        if (!_grid[cell].Contains(obj))
            _grid[cell].Add(obj);
    }

    // ������Ʈ ����
    public void Unregister(GameObject obj)
    {
        Vector2Int cell = GetCell(obj.transform.position);
        if (_grid.ContainsKey(cell))
            _grid[cell].Remove(obj);
    }

    // ��ġ�� �� ��ǥ ��ȯ
    public Vector2Int GetCell(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / _cellSize);
        int y = Mathf.FloorToInt(pos.z / _cellSize);
        return new Vector2Int(x, y);
    }

    // �ֺ� ��(�ڱ� �� + ���� 8����) ������Ʈ ��ȯ
    public List<GameObject> GetNearbyObjects(Vector3 pos)
    {
        List<GameObject> result = new List<GameObject>();
        Vector2Int center = GetCell(pos);
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int cell = new Vector2Int(center.x + dx, center.y + dy);
                if (_grid.ContainsKey(cell))
                    result.AddRange(_grid[cell]);
            }
        }
        return result;
    }

    // �� �̵� ó��
    public void MoveObject(GameObject obj, Vector2Int fromCell, Vector2Int toCell)
    {
        if (_grid.ContainsKey(fromCell))
            _grid[fromCell].Remove(obj);
        if (!_grid.ContainsKey(toCell))
            _grid[toCell] = new List<GameObject>();
        if (!_grid[toCell].Contains(obj))
            _grid[toCell].Add(obj);
    }

    // ����׿�: �� �׸���
    private void OnDrawGizmos()
    {
        // ��ü �׸��� �׸���
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeX; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                Vector3 cellCenter = new Vector3((x + 0.5f) * _cellSize, 0, (y + 0.5f) * _cellSize);

                // ��ü�� �ִ� ���� ������, ���� ���� �þȻ�
                if (_grid.ContainsKey(cell) && _grid[cell].Count > 0)
                    Gizmos.color = new Color(1, 0, 0, 0.4f); // ����
                else
                    Gizmos.color = new Color(0, 1, 1, 0.2f); // �þ�

                Gizmos.DrawCube(cellCenter, new Vector3(_cellSize, 0.1f, _cellSize));
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(cellCenter, new Vector3(_cellSize, 0.1f, _cellSize));
            }
        }
    }

}
