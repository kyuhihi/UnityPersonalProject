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
    // 오브젝트 등록
    public void Register(GameObject obj)
    {
        Vector2Int cell = GetCell(obj.transform.position);
        if (!_grid.ContainsKey(cell))
            _grid[cell] = new List<GameObject>();
        if (!_grid[cell].Contains(obj))
            _grid[cell].Add(obj);
    }

    // 오브젝트 제거
    public void Unregister(GameObject obj)
    {
        Vector2Int cell = GetCell(obj.transform.position);
        if (_grid.ContainsKey(cell))
            _grid[cell].Remove(obj);
    }

    // 위치로 셀 좌표 반환
    public Vector2Int GetCell(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / _cellSize);
        int y = Mathf.FloorToInt(pos.z / _cellSize);
        return new Vector2Int(x, y);
    }

    // 주변 셀(자기 셀 + 인접 8방향) 오브젝트 반환
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

    // 셀 이동 처리
    public void MoveObject(GameObject obj, Vector2Int fromCell, Vector2Int toCell)
    {
        if (_grid.ContainsKey(fromCell))
            _grid[fromCell].Remove(obj);
        if (!_grid.ContainsKey(toCell))
            _grid[toCell] = new List<GameObject>();
        if (!_grid[toCell].Contains(obj))
            _grid[toCell].Add(obj);
    }

    // 디버그용: 셀 그리기
    private void OnDrawGizmos()
    {
        // 전체 그리드 그리기
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeX; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                Vector3 cellCenter = new Vector3((x + 0.5f) * _cellSize, 0, (y + 0.5f) * _cellSize);

                // 객체가 있는 셀은 빨간색, 없는 셀은 시안색
                if (_grid.ContainsKey(cell) && _grid[cell].Count > 0)
                    Gizmos.color = new Color(1, 0, 0, 0.4f); // 빨강
                else
                    Gizmos.color = new Color(0, 1, 1, 0.2f); // 시안

                Gizmos.DrawCube(cellCenter, new Vector3(_cellSize, 0.1f, _cellSize));
                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(cellCenter, new Vector3(_cellSize, 0.1f, _cellSize));
            }
        }
    }

}
