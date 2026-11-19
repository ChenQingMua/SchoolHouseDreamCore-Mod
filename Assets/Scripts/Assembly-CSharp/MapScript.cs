using UnityEngine;
using System.Collections.Generic;

public class MapScript : MonoBehaviour
{
    public List<GameObject> roomPrefabs = new List<GameObject>();
    public GameObject endRoomPrefab;
    public GameObject doorPrefab;

    public bool endRoomDebug = false;

    public int width = 4;
    public int depth = 5;
    public float tileSize = 10f;

    public int viewRadius = 1;
    public int destroyRadius = 3;

    public Material floorMat;
    public Material wallMat;
    public Material ceilMat;
    public Material lightMat;
    public Material blackboardMat;
    public Material posterMat;
    public Material windowMat;
    public Material endFloorMat;

    public int minLights = 1;
    public int maxLights = 3;

    public Transform player;

    private Dictionary<Vector2Int, GameObject> activeRooms = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int lastPlayerGrid;
    private int worldSeed = 12345;

    private Dictionary<string, bool> edgeWindowStates = new Dictionary<string, bool>();
    private List<int> roomDeck = new List<int>();

    void Start()
    {
        worldSeed = Random.Range(0, 999999);
        ShuffleRoomDeck();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player != null)
        {
            lastPlayerGrid = GetGridCoord(player.position);
            GenerateAround(lastPlayerGrid);
        }
    }

    void Update()
    {
        if (player == null) return;
        Vector2Int currentGrid = GetGridCoord(player.position);
        if (currentGrid != lastPlayerGrid)
        {
            lastPlayerGrid = currentGrid;
            GenerateAround(currentGrid);
        }
    }

    private Vector2Int GetGridCoord(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / (width * tileSize)),
            Mathf.RoundToInt(worldPos.z / (depth * tileSize))
        );
    }

    private void GenerateAround(Vector2Int centerGrid)
    {
        for (int x = -viewRadius; x <= viewRadius; x++)
        {
            for (int z = -viewRadius; z <= viewRadius; z++)
            {
                Vector2Int targetGrid = new Vector2Int(centerGrid.x + x, centerGrid.y + z);
                if (!activeRooms.ContainsKey(targetGrid))
                {
                    BuildRoom(targetGrid, targetGrid == centerGrid);
                }
            }
        }

        List<Vector2Int> toDestroy = new List<Vector2Int>();
        foreach (var kvp in activeRooms)
        {
            Vector2Int roomGrid = kvp.Key;
            int distX = Mathf.Abs(roomGrid.x - centerGrid.x);
            int distZ = Mathf.Abs(roomGrid.y - centerGrid.y);
            if (Mathf.Max(distX, distZ) > destroyRadius)
            {
                toDestroy.Add(roomGrid);
            }
        }

        foreach (var grid in toDestroy)
        {
            Destroy(activeRooms[grid]);
            activeRooms.Remove(grid);
        }
    }

    private void BuildRoom(Vector2Int grid, bool isStartRoom)
    {
        GameObject roomRoot = new GameObject($"Room_{grid.x}_{grid.y}");
        float totalWidth = width * tileSize;
        float totalDepth = depth * tileSize;

        Vector3 roomCenter = new Vector3(grid.x * totalWidth, 0, grid.y * totalDepth);
        roomRoot.transform.position = roomCenter;
        activeRooms.Add(grid, roomRoot);

        bool spawnEnd = false;
        if (endRoomPrefab != null && Random.value < 0.05f)
        {
            if (endRoomDebug)
            {
                spawnEnd = true;
            }
            else
            {
                bool validDistance = (Mathf.Abs(roomCenter.x) + Mathf.Abs(roomCenter.z)) >= 2400f;
                bool validTime = Time.time >= 1200f;
                spawnEnd = validDistance && validTime;
            }
        }

        int lightCount = Random.Range(minLights, maxLights + 1);

        bool hasBlackboard = Random.value > 0.5f && blackboardMat != null;
        int blackboardFace = Random.Range(0, 4);

        bool hasPoster = Random.value < 0.25f && posterMat != null;
        int posterFace = Random.Range(0, 4);
        int posterIndex = Random.Range(0, 4);

        bool hasWindow = Random.value < 0.15f && windowMat != null;
        int windowFace = Random.Range(0, 4);
        int windowIndex = Random.Range(0, 4);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float posX = -totalWidth / 2f + tileSize / 2f + x * tileSize;
                float posZ = -totalDepth / 2f + tileSize / 2f + z * tileSize;

                GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.transform.SetParent(roomRoot.transform);
                floor.transform.localPosition = new Vector3(posX, 0, posZ);
                floor.transform.localScale = Vector3.one;

                Material currentFloorMat = spawnEnd ? endFloorMat : floorMat;
                if (currentFloorMat != null) floor.GetComponent<Renderer>().material = currentFloorMat;
                else floor.GetComponent<Renderer>().enabled = false;
                floor.name = "Logic_Floor";

                GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ceiling.transform.SetParent(roomRoot.transform);
                ceiling.transform.localPosition = new Vector3(posX, 10f, posZ);
                ceiling.transform.localRotation = Quaternion.Euler(180, 0, 0);
                ceiling.transform.localScale = Vector3.one;

                if (spawnEnd)
                {
                    if (lightMat != null) ceiling.GetComponent<Renderer>().material = lightMat;
                    else ceiling.GetComponent<Renderer>().enabled = false;
                }
                else if (lightCount > 0 && lightMat != null && Random.value < 0.2f)
                {
                    ceiling.GetComponent<Renderer>().material = lightMat;
                    lightCount--;
                }
                else
                {
                    if (ceilMat != null) ceiling.GetComponent<Renderer>().material = ceilMat;
                    else ceiling.GetComponent<Renderer>().enabled = false;
                }
                ceiling.name = "Logic_Ceiling";
            }
        }

        int doorNorthX = GetDoorIndex(grid, Vector2Int.up, width);
        int doorSouthX = GetDoorIndex(grid, Vector2Int.down, width);
        int doorEastZ = GetDoorIndex(grid, Vector2Int.right, depth);
        int doorWestZ = GetDoorIndex(grid, Vector2Int.left, depth);

        bool blackboardPlaced = false;
        bool posterPlaced = false;
        bool windowPlaced = false;

        for (int x = 0; x < width; x++)
        {
            float posX = -totalWidth / 2f + tileSize / 2f + x * tileSize;

            if (x != doorSouthX)
            {
                Material mat = GetWallMaterial(grid, x, ref blackboardPlaced, ref posterPlaced, ref windowPlaced,
                    hasBlackboard, blackboardFace, 1, hasPoster, posterFace, posterIndex,
                    hasWindow, windowFace, windowIndex, 1, Vector2Int.down);
                BuildWall(roomRoot.transform, new Vector3(posX, 5, -totalDepth / 2f), Quaternion.Euler(90, 0, 0), mat);
            }

            if (x != doorNorthX)
            {
                Material mat = GetWallMaterial(grid, x, ref blackboardPlaced, ref posterPlaced, ref windowPlaced,
                    hasBlackboard, blackboardFace, 0, hasPoster, posterFace, posterIndex,
                    hasWindow, windowFace, windowIndex, 0, Vector2Int.up);
                BuildWall(roomRoot.transform, new Vector3(posX, 5, totalDepth / 2f), Quaternion.Euler(90, 180, 0), mat);
            }
        }

        for (int z = 0; z < depth; z++)
        {
            float posZ = -totalDepth / 2f + tileSize / 2f + z * tileSize;

            if (z != doorWestZ)
            {
                Material mat = GetWallMaterial(grid, z, ref blackboardPlaced, ref posterPlaced, ref windowPlaced,
                    hasBlackboard, blackboardFace, 3, hasPoster, posterFace, posterIndex,
                    hasWindow, windowFace, windowIndex, 3, Vector2Int.left);
                BuildWall(roomRoot.transform, new Vector3(-totalWidth / 2f, 5, posZ), Quaternion.Euler(90, 90, 0), mat);
            }

            if (z != doorEastZ)
            {
                Material mat = GetWallMaterial(grid, z, ref blackboardPlaced, ref posterPlaced, ref windowPlaced,
                    hasBlackboard, blackboardFace, 2, hasPoster, posterFace, posterIndex,
                    hasWindow, windowFace, windowIndex, 2, Vector2Int.right);
                BuildWall(roomRoot.transform, new Vector3(totalWidth / 2f, 5, posZ), Quaternion.Euler(90, -90, 0), mat);
            }
        }

        if (doorPrefab != null)
        {
            Vector3 northDoorPos = new Vector3(roomCenter.x + (-totalWidth / 2f + tileSize / 2f + doorNorthX * tileSize), 0, roomCenter.z + totalDepth / 2f);
            SpawnDoorAt(northDoorPos, Quaternion.Euler(0, 180, 0), roomRoot.transform, "Door_North");

            Vector3 eastDoorPos = new Vector3(roomCenter.x + totalWidth / 2f, 0, roomCenter.z + (-totalDepth / 2f + tileSize / 2f + doorEastZ * tileSize));
            SpawnDoorAt(eastDoorPos, Quaternion.Euler(0, -90, 0), roomRoot.transform, "Door_East");
        }

        if (spawnEnd)
        {
            GameObject decor = Instantiate(endRoomPrefab, roomRoot.transform);
            CleanupPrefab(decor);

            float baseRotation = 90f;
            float randomFlip = Random.value > 0.5f ? 180f : 0f;

            decor.transform.localRotation = Quaternion.Euler(0, baseRotation + randomFlip, 0);
            decor.transform.localPosition = Vector3.zero;

            CenterDecor(decor);
        }
        else if (roomPrefabs != null && roomPrefabs.Count > 0)
        {
            int prefabIndex = 0;

            if (!isStartRoom)
            {
                if (roomDeck.Count == 0) ShuffleRoomDeck();
                prefabIndex = roomDeck[0];
                roomDeck.RemoveAt(0);
            }
            else
            {
                roomDeck.Remove(0);
            }

            GameObject selectedPrefab = roomPrefabs[prefabIndex];

            if (selectedPrefab != null)
            {
                GameObject decor = Instantiate(selectedPrefab, roomRoot.transform);
                CleanupPrefab(decor);

                float baseRotation = 90f;
                float randomFlip = isStartRoom ? 0f : (Random.value > 0.5f ? 180f : 0f);

                decor.transform.localRotation = Quaternion.Euler(0, baseRotation + randomFlip, 0);
                decor.transform.localPosition = Vector3.zero;

                CenterDecor(decor);
            }
        }
    }

    private void ShuffleRoomDeck()
    {
        roomDeck.Clear();
        for (int i = 0; i < roomPrefabs.Count; i++)
        {
            roomDeck.Add(i);
        }

        for (int i = 0; i < roomDeck.Count; i++)
        {
            int temp = roomDeck[i];
            int randomIndex = Random.Range(i, roomDeck.Count);
            roomDeck[i] = roomDeck[randomIndex];
            roomDeck[randomIndex] = temp;
        }
    }

    private Material GetWallMaterial(Vector2Int grid, int index, ref bool blackboardPlaced, ref bool posterPlaced, ref bool windowPlaced,
        bool hasBlackboard, int blackboardFace, int currentFace, bool hasPoster, int posterFace, int posterIndex,
        bool hasWindow, int windowFace, int windowIndex, int dirFace, Vector2Int dirOffset)
    {
        Material mat = wallMat;

        if (hasBlackboard && blackboardFace == currentFace && !blackboardPlaced)
        {
            mat = blackboardMat;
            blackboardPlaced = true;
            return mat;
        }

        if (hasPoster && posterFace == currentFace && posterIndex == index && !posterPlaced)
        {
            mat = posterMat;
            posterPlaced = true;
            return mat;
        }

        if (windowMat != null)
        {
            Vector2Int neighborGrid = grid + dirOffset;

            int minX = Mathf.Min(grid.x, neighborGrid.x);
            int minY = Mathf.Min(grid.y, neighborGrid.y);
            int maxX = Mathf.Max(grid.x, neighborGrid.x);
            int maxY = Mathf.Max(grid.y, neighborGrid.y);
            string edgeKey = $"{minX}_{minY}_{maxX}_{maxY}_{index}";

            if (!edgeWindowStates.ContainsKey(edgeKey))
            {
                bool isWindow = false;
                if (hasWindow && windowFace == currentFace && windowIndex == index)
                {
                    isWindow = true;
                }
                edgeWindowStates[edgeKey] = isWindow;
            }

            if (edgeWindowStates[edgeKey] && !windowPlaced)
            {
                windowPlaced = true;
                return windowMat;
            }
        }

        return mat;
    }

    private void BuildWall(Transform parent, Vector3 localPos, Quaternion localRot, Material customMat = null)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Plane);
        wall.transform.SetParent(parent);
        wall.transform.localPosition = localPos;
        wall.transform.localRotation = localRot;
        wall.transform.localScale = Vector3.one;

        Material finalMat = customMat != null ? customMat : wallMat;

        if (finalMat != null) wall.GetComponent<Renderer>().material = finalMat;
        else wall.GetComponent<Renderer>().enabled = false;

        if (finalMat == blackboardMat) wall.name = "Logic_Blackboard";
        else if (finalMat == posterMat) wall.name = "Logic_Poster";
        else if (finalMat == windowMat) wall.name = "Logic_Window";
        else wall.name = "Logic_Wall";
    }

    private void SpawnDoorAt(Vector3 targetPos, Quaternion targetRot, Transform parent, string doorName)
    {
        GameObject door = Instantiate(doorPrefab, targetPos, targetRot, parent);
        door.name = doorName;

        Collider[] colliders = door.GetComponentsInChildren<Collider>();
        foreach (var c in colliders) c.enabled = false;

        Renderer[] renderers = door.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            Vector3 visualCenter = bounds.center;
            Vector3 offset = targetPos - visualCenter;
            offset.y = 0;

            door.transform.position += offset;
        }

        foreach (var c in colliders) c.enabled = true;
    }

    private int GetDoorIndex(Vector2Int gridA, Vector2Int dir, int maxSize)
    {
        Vector2Int gridB = gridA + dir;
        int minX = Mathf.Min(gridA.x, gridB.x);
        int minY = Mathf.Min(gridA.y, gridB.y);
        int maxX = Mathf.Max(gridA.x, gridB.x);
        int maxY = Mathf.Max(gridA.y, gridB.y);

        unchecked
        {
            int hash = worldSeed;
            hash = hash * 31 + minX;
            hash = hash * 31 + minY;
            hash = hash * 31 + maxX;
            hash = hash * 31 + maxY;

            Random.State oldState = Random.state;
            Random.InitState(hash);
            int result = Random.Range(0, maxSize);
            Random.state = oldState;

            return result;
        }
    }

    private void CleanupPrefab(GameObject decor)
    {
        for (int i = decor.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = decor.transform.GetChild(i);
            string name = child.name;

            if (name.Contains("Wall") || name.Contains("Corner") || name.Contains("Floor") || name.Contains("Ceiling") || name.Contains("Door"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CenterDecor(GameObject decor)
    {
        Renderer[] renderers = decor.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 worldCenter = bounds.center;
        Vector3 localCenter = decor.transform.parent.InverseTransformPoint(worldCenter);

        Vector3 offset = new Vector3(-localCenter.x, 0, -localCenter.z);
        decor.transform.localPosition += offset;
    }
}
