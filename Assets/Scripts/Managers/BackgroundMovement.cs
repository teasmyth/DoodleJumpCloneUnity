using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    public float fadeInSpeed = 2f;
    public float fadeInSmoothness = 0.05f;
    public float scrollSpeedOnDeath;
    public List<Background> backgrounds = new List<Background>();
    public List<GameObject> swappingObjects = new List<GameObject>();

    public int threshold1;
    private bool threshHold1Passed = false;
    public int threshold2;
    private bool threshHold2Passed = false;

    private Vector2 cameraPos;
    private Vector2 prevCameraPos;

    private GameSettings settings;
    private void Start()
    {
        settings = GameSettings.Instance;
        for (int i = 0; i < backgrounds.Count; i++)
        {
            backgrounds[i].activeSprite = backgrounds[i].defaultSprite;
            backgrounds[i].childSprite = backgrounds[i].altSprite1;

            swappingObjects[i].GetComponent<SpriteRenderer>().sprite = backgrounds[i].activeSprite;
            swappingObjects[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = backgrounds[i].childSprite;
            Color c = swappingObjects[i].transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().color;
            c.a = 0;
            swappingObjects[i].transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().color = c;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        cameraPos = Camera.main.transform.position;
        float diff = cameraPos.y - prevCameraPos.y;

        for (int i = 0; i < swappingObjects.Count; i++)
        {
            if (settings.gameState == GameSettings.GameState.Playing)
            {
                if (swappingObjects[i].transform.localPosition.y <= -21.6f)
                {
                    swappingObjects[i].transform.localPosition += new Vector3(0, 10.8f * swappingObjects.Count, 0);
                    LoadBackground(swappingObjects[i]);
                }
                swappingObjects[i].transform.position = Vector3.Lerp(swappingObjects[i].transform.position, swappingObjects[i].transform.position += new Vector3(0, -diff), 1);
            }
            else if (settings.gameState == GameSettings.GameState.Over)
            {
                swappingObjects[i].transform.position = Vector3.Lerp(swappingObjects[i].transform.position, swappingObjects[i].transform.position += new Vector3(0, scrollSpeedOnDeath), 1);
                if (swappingObjects[i].transform.localPosition.y >= 21.6f)
                {
                    swappingObjects[i].transform.localPosition += new Vector3(0, -10.8f * swappingObjects.Count, 0);
                    LoadBackground(swappingObjects[i]);
                }

            }
        }
        prevCameraPos = cameraPos;

        //Don't need the rest if the game is over
        if (settings.gameState == GameSettings.GameState.Over)
        {
            return;
        }

        for (int i = 0; i < swappingObjects.Count; i++)
        {
            for (int j = 0; j < backgrounds.Count; j++)
            {
                if (swappingObjects[i] == backgrounds[j].gameObject)
                {
                    LoadBackGroundWithKnownBG(swappingObjects[i], backgrounds[j]);
                }
            }
        }

        if (cameraPos.y / settings.scoreMultiplier >= threshold1 && !threshHold1Passed)
        {
            for (int i = 0; i < backgrounds.Count; i++)
            {
                StartCoroutine(FadeInBGsCO(backgrounds[i], backgrounds[i].altSprite1, fadeInSpeed, fadeInSmoothness));
            }
            threshHold1Passed = true;
        }
        if (cameraPos.y >= threshold2 && !threshHold2Passed && threshHold1Passed && threshold2 != 0)
        {
            for (int i = 0; i < backgrounds.Count; i++)
            {
                StartCoroutine(FadeInBGsCO(backgrounds[i], backgrounds[i].altSprite2, fadeInSpeed, fadeInSmoothness));
            }
            threshHold2Passed = true;
        }
    }

    private void LoadBackGroundWithKnownBG(GameObject swappingObject, Background background)
    {
        SpriteRenderer parentRenderer = swappingObject.GetComponent<SpriteRenderer>();
        SpriteRenderer childRenderer = swappingObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        parentRenderer.sprite = background.activeSprite;
        childRenderer.sprite = background.childSprite;
        Color c = childRenderer.color;
        c.a = background.alpha;
        childRenderer.color = c;


    }

    private void LoadBackground(GameObject swappingObject)
    {
        SpriteRenderer parentRenderer = swappingObject.GetComponent<SpriteRenderer>();
        SpriteRenderer childRenderer = swappingObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        Background background = backgrounds[Random.Range(0, backgrounds.Count)];

        parentRenderer.sprite = background.activeSprite;
        childRenderer.sprite = background.childSprite;
        //Color c = childRenderer.color;
        //c.a = 0;
        //childRenderer.color = c;
        background.gameObject = swappingObject;
    }

    private IEnumerator FadeInBGsCO(Background background, Sprite newSprite, float fadeInSpeed, float steps)
    {
        background.childSprite = newSprite;
        float start = background.alpha;
        for (float alpha = start; alpha <= 1; alpha += steps)
        {
            background.alpha = alpha;
            yield return new WaitForSeconds(fadeInSpeed);
        }
        background.activeSprite = newSprite;
    }

    [System.Serializable]
    public class Background
    {
        public Sprite defaultSprite;
        public Sprite altSprite1;
        public Sprite altSprite2;
        [HideInInspector] public GameObject gameObject;
        [HideInInspector] public Sprite activeSprite;
        [HideInInspector] public Sprite childSprite;
        [HideInInspector] public bool changing;
        [HideInInspector] public float alpha;
    }
}