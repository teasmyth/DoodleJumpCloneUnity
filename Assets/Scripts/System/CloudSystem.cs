using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSystem : MonoBehaviour
{
    public Vector3 cameraPos;
    public List<Sprite> cloudSprites;

    [Header("Spawn Attributes")]
    public float maxSpawnHeight;
    public float minSpawnHeight;
    public float spawnOffset;
    public float sizeMin;
    public float sizeMax;

    [Header("Settings")]
    public float minCloudSpeed;
    public float maxCloudSpeed;
    [Tooltip("How many times a cloud spawns each second.")]
    public float spawnDelay;
    public int amountOfClouds;
    public int layerOrderToSpawnBehind = 20;

    private readonly List<int> dirPick = new List<int> { -1, 1 };
    public List<Cloud> clouds = new List<Cloud>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amountOfClouds; i++)
        {
            MakeCloud();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (Camera.main.WorldToViewportPoint(transform.GetChild(i).position).y > 1.1f || Camera.main.WorldToViewportPoint(transform.GetChild(i).position).y < -0.05f)
            {
                clouds.Remove(transform.GetChild(i).GetComponent<Cloud>());
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < clouds.Count; i++)
        {
            if (clouds[i] == null)
            {
                clouds.Remove(clouds[i]);
            }
        }

        for (int i = 0; i < clouds.Count; i++)
        {
            if (clouds[i] == null)
            {
                clouds.Remove(clouds[i]);
                break;
            }
            Cloud cloud = clouds[i];
            if (!cloud.wasOnScreen)
            {
                MoveCloud(clouds[i].gameObject);
            }
            if (IsItOnScreen(cloud))
            {
                MoveCloud(clouds[i].gameObject);
                cloud.wasOnScreen = true;
            }
            else if (!IsItOnScreen(cloud) && cloud.wasOnScreen)
            {
                MakeCloud();
                Destroy(cloud.gameObject);
            }
        }
    }

    void MakeCloud()
    {
        GameObject newGO = new GameObject();
        Cloud cloud = newGO.AddComponent<Cloud>();
        cloud.gameObject.SetActive(false);
        cloud.gameObject.transform.SetParent(transform);
        cloud.speed = Random.Range(minCloudSpeed, maxCloudSpeed);
        cloud.direction = dirPick[Random.Range(0, 2)];
        cloud.startingHeight = cameraPos.y + Mathf.Round(Random.Range(minSpawnHeight, maxSpawnHeight) * 100f) / 100f;
        float size = 1 + Random.Range(sizeMin, sizeMax);
        cloud.transform.localScale = new Vector3(size, size, 1);
        SpriteRenderer spriteRenderer = cloud.gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = cloudSprites[Random.Range(0, cloudSprites.Count - 1)];
        spriteRenderer.sortingOrder = layerOrderToSpawnBehind - transform.childCount;
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            spriteRenderer.flipX = true;
        }
        PolygonCollider2D col = cloud.gameObject.AddComponent<PolygonCollider2D>();
        cloud.leftEdgeDistance = Vector2.Distance(cloud.gameObject.transform.position, col.bounds.min);
        cloud.rightEdgeDistance = Vector2.Distance(cloud.gameObject.transform.position, col.bounds.max);
        col.enabled = false;
        cloud.gameObject.transform.position = new Vector3(spawnOffset * -cloud.direction, cloud.startingHeight, 1);
        clouds.Add(cloud);
        cloud.gameObject.SetActive(true);
    }

    [System.Serializable]
    public class Cloud : MonoBehaviour
    {
        public float speed;
        public float startingHeight;
        public int direction;
        public bool wasOnScreen = false;
        public float leftEdgeDistance;
        public float rightEdgeDistance;
    }

    void MoveCloud(GameObject cloudObject)
    {
        cloudObject.transform.position += new Vector3(cloudObject.GetComponent<Cloud>().speed * cloudObject.GetComponent<Cloud>().direction, 0, 0) * Time.deltaTime;
    }

    bool IsItOnScreen(Cloud cloud)
    {
        if (Camera.main.WorldToViewportPoint(cloud.gameObject.transform.position).y > 1.1f || Camera.main.WorldToViewportPoint(cloud.gameObject.transform.position).y < -0.05f)
        {
            return false;
        }

        Vector2 leftEdge = cloud.gameObject.transform.position - new Vector3(cloud.leftEdgeDistance, 0, 0);
        Vector2 rightEdge = cloud.gameObject.transform.position + new Vector3(cloud.rightEdgeDistance, 0, 0);

        //Cloud is on the left side - Has to check if right side is on screen
        if (Camera.main.WorldToViewportPoint(cloud.gameObject.transform.position).x < 0.5)
        {
            if (Camera.main.WorldToViewportPoint(rightEdge).x > -0.05f)
            {
                return true;
            }
        }
        //Cloud is on the right side
        else if (Camera.main.WorldToViewportPoint(cloud.gameObject.transform.position).x > 0.5)
        {
            if (Camera.main.WorldToViewportPoint(leftEdge).x < 1.05f)
            {
                return true;
            }
        }
        if (Camera.main.WorldToViewportPoint(cloud.gameObject.transform.position).x == 0.5)
        {
            return true;
        }
        return false;
    }
}