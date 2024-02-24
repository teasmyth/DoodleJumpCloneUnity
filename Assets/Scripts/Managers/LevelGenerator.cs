using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject normalPlatformPrefab;
    public GameObject tinyPlatformPrefab;
    public GameObject crashingPlatformPrefab;

    [Header("Settings")]
    public float spawnMaxXOffset;
    [Tooltip("If you want to have a minimum rotation degrees difference between two consequent platforms, give it a value")]
    public int minRotationBetweenLast;
    [Space(10)]
    public List<PlatformWave> platformWaves = new List<PlatformWave>();

    private int prevRot = 0;
    private int waveIndex = 1;
    private bool hasPowerup;
    private bool infiniteSpawn = false;
    private GameSettings settings;
    private PlayerStat pS;

    private Vector3 infiniteSpawnPos;

    void Start()
    {
        pS = PlayerStat.Instance;
        settings = GameSettings.Instance;
        LoadAllPlatformWaves();
    }


    private void Update()
    {
        if (infiniteSpawn && settings.gameState != GameSettings.GameState.Over)
        {
            InfiniteSpawn(settings.infiniteSpawnAhead, platformWaves[platformWaves.Count - 1]);
        }
    }

    void LoadAllPlatformWaves()
    {
        Vector3 nextStart = Vector3.zero;
        for (int i = 0; i < platformWaves.Count; i++)
        {
            Vector3 lastPos = nextStart;
            nextStart = LoadPlatformWave(platformWaves[i], lastPos, i == platformWaves.Count - 1);
        }
    }

    //Will return the position of from where should the next wave start.
    Vector3 LoadPlatformWave(PlatformWave platformWave, Vector3 startPos, bool lastWave)
    {
        platformWave.realThreshhold = platformWave.stoppingScoreThreshhold / settings.scoreMultiplier;

        if (minRotationBetweenLast > platformWave.maxRotation)
        {
            Debug.Log("Minimum difference rotation is too high!");
            return Vector3.zero;
        }
        if (100 - platformWave.crashingChance - platformWave.tinySpawnChance - platformWave.rotationChance < 0)
        {
            Debug.Log("Bad spawn cahnce inputs, the sum is more than 100% chance! Will not load.");
            return Vector3.zero;
        }

        Vector3 lastPos = startPos;
        prevRot = 0;

        GameObject waveParent = new GameObject();
        waveParent.name = "Wave " + waveIndex + " parent";
        waveIndex++;

        //Initial spawn
        bool platformHeightReached = false;
        lastPos.x = Random.Range(-spawnMaxXOffset, spawnMaxXOffset);
        lastPos.y += Random.Range(platformWave.minSpawnY, platformWave.maxSpawnY);
        GameObject firstPlatform = PickPlatform(platformWave, waveParent.transform, lastPos);
        firstPlatform.transform.SetParent(waveParent.transform);
        platformWave.firstPlatformTransform = firstPlatform.transform;
        firstPlatform.SetActive(true);
        if (platformWave.realThreshhold != 0)
        {
            while (!platformHeightReached)
            {
                lastPos.x = Random.Range(-platformWave.spawnMaxXOffset, platformWave.spawnMaxXOffset);
                lastPos.y += Random.Range(platformWave.minSpawnY, platformWave.maxSpawnY);
                platformHeightReached = lastPos.y >= platformWave.realThreshhold;
                if (platformHeightReached)
                {
                    infiniteSpawnPos = lastPos;
                    break;
                }
                GameObject platformToSpawn = PickPlatform(platformWave, waveParent.transform, lastPos);
                if (platformToSpawn != null)
                {
                    if (hasPowerup)
                    {
                        PickPowerup(platformWave, platformToSpawn);
                    }
                    platformToSpawn.SetActive(true);
                }
                else
                {
                    Debug.Log("Platform is null");
                }
            }
            infiniteSpawn = false;
            waveParent.transform.SetParent(transform);
        }
        else if (platformWave.realThreshhold == 0 && lastWave)
        {
            infiniteSpawn = true;
            GameObject lastParent = new GameObject();
            lastParent.name = "Infinite wave";
            lastParent.transform.SetParent(transform);
        }
        return lastPos;
    }

    void InfiniteSpawn(float distance, PlatformWave platformWave)
    {
        if (Vector2.Distance(pS.playerGO.transform.position, infiniteSpawnPos) < distance / settings.scoreMultiplier)
        {
            infiniteSpawnPos.x = Random.Range(-platformWave.spawnMaxXOffset, platformWave.spawnMaxXOffset);
            infiniteSpawnPos.y += Random.Range(platformWave.minSpawnY, platformWave.maxSpawnY);
            GameObject platformToSpawn = PickPlatform(platformWave, transform.GetChild(transform.childCount - 1).transform, infiniteSpawnPos);
            if (platformToSpawn != null)
            {
                if (hasPowerup)
                {
                    PickPowerup(platformWave, platformToSpawn);
                }
                platformToSpawn.SetActive(true);
            }
            else
            {
                Debug.Log("Platform is null");
            }
        }

    }

    /// <summary>
    /// Returns an *inactive* platform.
    /// </summary>
    /// <param name="platformWave"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private GameObject PickPlatform(PlatformWave platformWave, Transform parentTransform, Vector3 pos)
    {
        int random = Random.Range(1, 101);

        if (random <= platformWave.tinySpawnChance) //Tiny platform chance
        {
            hasPowerup = true;
            return InstantiatePlatform(tinyPlatformPrefab, parentTransform, pos);
        }
        else if (random > platformWave.tinySpawnChance && random <= platformWave.tinySpawnChance + platformWave.crashingChance) //Crashing platform chance
        {
            hasPowerup = true;
            return InstantiatePlatform(crashingPlatformPrefab, parentTransform, pos);
        }
        else if (random > platformWave.tinySpawnChance + platformWave.crashingChance && random <= platformWave.tinySpawnChance + platformWave.crashingChance + platformWave.rotationChance) //Rotated platform chance
        {
            hasPowerup = true;
            GameObject platform = InstantiatePlatform(normalPlatformPrefab, parentTransform, pos);
            platform.transform.rotation = PickNextRotation(minRotationBetweenLast, platformWave.maxRotation);
            return platform;
        }
        else if (random > platformWave.tinySpawnChance + platformWave.crashingChance + platformWave.rotationChance) //Normal platform chance
        {
            hasPowerup = false;
            if (platformWave.standardHasPowerups)
            {
                hasPowerup = true;
            }
            return InstantiatePlatform(normalPlatformPrefab, parentTransform, pos);
        }
        return null;
    }

    private GameObject InstantiatePlatform(GameObject platform, Transform parentTransform, Vector3 pos)
    {
        GameObject _ = Instantiate(platform, parentTransform);
        _.SetActive(false);
        _.transform.position = pos;
        _.name = platform.name;
        return _;
    }

    void PickPowerup(PlatformWave platformWave, GameObject platform)
    {
        if (100 < platformWave.crystalPowerupChance + platformWave.rootPowerupChance)
        {
            Debug.Log("Faulty powerup chances. Will not spawn powerups.");
            return;
        }
        int random = Random.Range(1, 101);
        if (random <= platformWave.crystalPowerupChance)
        {
            platform.GetComponent<Platform>().crystal.SetActive(true);
        }
        else if (random > platformWave.crystalPowerupChance && random <= platformWave.crystalPowerupChance + platformWave.rootPowerupChance)
        {
            platform.GetComponent<Platform>().root.SetActive(true);
        }
    }

    private Quaternion PickNextRotation(int minDif, int maxRotation)
    {
        List<int> rotationList = new List<int>();
        for (int i = -maxRotation; i <= maxRotation * 2; i++)
        {
            if (i != 0)
            {
                rotationList.Add(i);
            }
        }
        if (prevRot != 0 && minRotationBetweenLast != 0)
        {
            for (int i = -minDif + prevRot; i <= (prevRot + minDif) * 2; i++)
            {
                if (rotationList.Contains(i))
                {
                    rotationList.Remove(i);
                }
            }
        }
        int toReturn = rotationList[Random.Range(0, rotationList.Count)];
        return Quaternion.Euler(0, 0, toReturn);
    }

    [System.Serializable]
    public class PlatformWave
    {
        [Tooltip("Leaving it at 0 will make it go infinite")]
        public float stoppingScoreThreshhold;
        public float minSpawnY;
        public float maxSpawnY;
        public float spawnMaxXOffset;
        [Space(6)]
        [Header("Spawn rates: ")]
        [Space(3)]
        [Range(0, 100)] public float tinySpawnChance;
        [Range(0, 100)] public float crashingChance;
        [Range(0, 100)] public float rotationChance;
        [Range(0, 90)] public int maxRotation;

        [Header("Powerup Rates: ")]
        [Range(0, 100)] public float crystalPowerupChance;
        [Range(0, 100)] public float rootPowerupChance;
        public bool standardHasPowerups = false;
        [HideInInspector] public Transform firstPlatformTransform;
        [HideInInspector] public float realThreshhold;
    }
}