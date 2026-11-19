using UnityEngine;

public class EndMapScript : MonoBehaviour
{
    public Material endFloorMat;

    public float tileSize = 10f;

    public int mapSize = 30;

    public float extendRatio = 0.5f;

    public Material fenceMat;

    public GameObject treePrefab;

    public float treeChance = 0.2f;

    public float treePositionJitter = 0.3f;

    public float treeRotationRandom = 360f;

    public Vector2 treeScaleRange = new Vector2(0.8f, 1.2f);

    void Start()
    {
        if (mapSize <= 0 || tileSize <= 0f) return;

        GenerateMap();
        GenerateFences();
        GenerateTrees();
    }

    private int ExtraTiles
    {
        get
        {
            if (extendRatio <= 0f) return 0;
            return Mathf.Max(1, Mathf.RoundToInt(mapSize * extendRatio));
        }
    }

    private float GridOffset
    {
        get { return (mapSize % 2 == 0) ? tileSize * 0.5f : 0f; }
    }

    private float GridPosition(int index)
    {
        return (index - mapSize / 2) * tileSize + GridOffset;
    }

    private void GenerateMap()
    {
        int extra = ExtraTiles;

        for (int ix = -extra; ix <= mapSize - 1 + extra; ix++)
        {
            for (int iz = -extra; iz <= mapSize - 1 + extra; iz++)
            {
                SpawnTile(new Vector3(GridPosition(ix), 0f, GridPosition(iz)), ix, iz);
            }
        }
    }

    private void SpawnTile(Vector3 localPos, int gridX, int gridZ)
    {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
        tile.name = $"EndFloor_{gridX}_{gridZ}";
        tile.transform.SetParent(transform, false);
        tile.transform.localPosition = localPos;
        tile.transform.localScale = new Vector3(tileSize / 10f, 1f, tileSize / 10f);

        if (endFloorMat != null)
        {
            tile.GetComponent<Renderer>().sharedMaterial = endFloorMat;
        }
    }

    private void GenerateFences()
    {
        float northEdge = GridPosition(mapSize - 1) + tileSize * 0.5f;
        float southEdge = GridPosition(0) - tileSize * 0.5f;

        float scale = tileSize / 10f;
        float centerY = tileSize * 0.5f;

        Transform root = new GameObject("Fences").transform;
        root.SetParent(transform, false);

        for (int ix = -ExtraTiles; ix <= mapSize - 1; ix++)
        {
            float along = GridPosition(ix);
            SpawnFencePanel(root, $"Fence_North_{ix}", new Vector3(along, centerY, northEdge), Vector3.back, scale);
            SpawnFencePanel(root, $"Fence_South_{ix}", new Vector3(along, centerY, southEdge), Vector3.forward, scale);
        }

        float eastEdge = northEdge;
        for (int iz = 0; iz <= mapSize - 1; iz++)
        {
            float along = GridPosition(iz);
            SpawnFencePanel(root, $"Fence_East_{iz}", new Vector3(eastEdge, centerY, along), Vector3.left, scale);
        }
    }

    private void SpawnFencePanel(Transform parent, string panelName, Vector3 pos, Vector3 inwardNormal, float scale)
    {
        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Plane);
        panel.name = panelName;
        panel.transform.SetParent(parent, true);
        panel.transform.position = pos;

        panel.transform.rotation = Quaternion.LookRotation(Vector3.down, inwardNormal);
        panel.transform.localScale = new Vector3(scale, 1f, scale);

        if (fenceMat != null)
        {
            panel.GetComponent<Renderer>().sharedMaterial = fenceMat;
        }
    }

    private void GenerateTrees()
    {
        if (treePrefab == null || treeChance <= 0f) return;

        int minX = -ExtraTiles;
        int maxX = mapSize - 1;
        int minZ = 0;
        int maxZ = mapSize - 1;

        int centerX = FindCenterIndex(minX, maxX);
        int centerZ = FindCenterIndex(minZ, maxZ);

        Transform root = new GameObject("Trees").transform;
        root.SetParent(transform, false);

        for (int ix = minX; ix <= maxX; ix++)
        {
            if (IsReservedIndex(ix, minX, maxX, centerX)) continue;

            for (int iz = minZ; iz <= maxZ; iz++)
            {
                if (IsReservedIndex(iz, minZ, maxZ, centerZ)) continue;
                if (Random.value >= treeChance) continue;

                SpawnTree(root, ix, iz);
            }
        }
    }

    private bool IsReservedIndex(int index, int minIndex, int maxIndex, int centerLow)
    {
        if (index == minIndex || index == maxIndex) return true;
        return index == centerLow || index == centerLow + 1;
    }

    private int FindCenterIndex(int minIndex, int maxIndex)
    {
        int low = minIndex;
        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (GridPosition(i) >= 0f) break;
            low = i;
        }
        return low;
    }

    private void SpawnTree(Transform parent, int gridX, int gridZ)
    {
        float jitter = Mathf.Clamp01(treePositionJitter) * tileSize * 0.5f;
        Vector3 pos = new Vector3(
            GridPosition(gridX) + Random.Range(-jitter, jitter),
            0f,
            GridPosition(gridZ) + Random.Range(-jitter, jitter)
        );

        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, treeRotationRandom), 0f);

        GameObject tree = Instantiate(treePrefab, pos, rotation, parent);
        tree.name = $"Tree_{gridX}_{gridZ}";

        float scale = Random.Range(
            Mathf.Min(treeScaleRange.x, treeScaleRange.y),
            Mathf.Max(treeScaleRange.x, treeScaleRange.y)
        );
        tree.transform.localScale *= scale;
    }
}
