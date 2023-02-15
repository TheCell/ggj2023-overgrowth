using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class LevelGeneratorGrid : MonoBehaviour
{

    public GameObject[] buildingObjects;
    public int buildingDensity; // between 1 and 100 (maxDensity)
    // public GameObject treeObject;
    public GameObject crossroadObject;
    public GameObject monumentObject;
    public int fieldSize;
    public GameObject enemySpawner;
    
    private int maxDensity = 100;
    private int[] roationOptions = { 0, 90, 180, 270 };
    private int pointDistance = 5;
    private GameObject[,] crossRoadGrid;
    private List<GameObject> buildings;
    private int monumentPosition = 1;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject start = GameObject.Find("NavMeshSceneGeometry");
        //GameObject ground = GameObject.Find("Ground");
        //ground.gameObject.transform.localScale += new Vector3((fieldSize * pointDistance), 0, (fieldSize * pointDistance));
        BuildCrossroadGrid();
        AddBuildingsAndMonument();
        PlantTreesAtCenter();

        Instantiate(enemySpawner, new Vector3(-4, 0,-4), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject b in buildings)
        {
            BuildingGrowth script = (BuildingGrowth) b.GetComponent(typeof(BuildingGrowth));
            script.RedrawBuilding();
            if (script.IsOvergrown() && script.isMonument)
            {
                winGame();
            }
        }
        CheckIfGameLost();
    }

    void connectAdjacentCrossroadsToBuilding(GameObject b, int i, int j)
    {
        connectAdjacentCrossroad(b, i, j);
        connectAdjacentCrossroad(b, i + 1, j);
        connectAdjacentCrossroad(b, i, j + 1);
        connectAdjacentCrossroad(b, i + 1, j + 1);
    }

    void BuildCrossroadGrid()
    {
        crossRoadGrid = new GameObject[fieldSize, fieldSize];
        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                Vector3 position = new Vector3(i * pointDistance, 0.079f, j * pointDistance);

                crossRoadGrid[i, j] = Instantiate(crossroadObject, position, Quaternion.identity);

                List<GameObject> bros = new List<GameObject>();
                if (i > 0)
                {
                    bros.Add(crossRoadGrid[i - 1, j]);
                }
                if (j > 0)
                {
                    bros.Add(crossRoadGrid[i, j - 1]);
                }
                crossRoadGrid[i, j].GetComponent<CrossroadGrowth>().connectedCrossroads = bros;
            }
        }
    }
    private void PlantTreesAtCenter()
    {
        int center = (fieldSize / 2) - 1;
        crossRoadGrid[center, center].GetComponent<CrossroadGrowth>().startsWithTree = true;
        crossRoadGrid[center + 1, center].GetComponent<CrossroadGrowth>().startsWithTree = true;
        crossRoadGrid[center, center + 1].GetComponent<CrossroadGrowth>().startsWithTree = true;
        crossRoadGrid[center + 1, center + 1].GetComponent<CrossroadGrowth>().startsWithTree = true;
    }
    private void AddBuildingsAndMonument()
    {
        buildings = new List<GameObject>();
        for (int i = 0; i < fieldSize - 1; i++)
        {
            for (int j = 0; j < fieldSize - 1; j++)
            {
                int rand = UnityEngine.Random.Range(0, buildingObjects.Length);
                bool build = UnityEngine.Random.Range(0, maxDensity) < buildingDensity;

                if (i == monumentPosition && j == monumentPosition)
                {
                    GameObject b = addBuilding(monumentObject, i, j);
                    BuildingGrowth script = (BuildingGrowth)b.GetComponent(typeof(BuildingGrowth));
                    script.isMonument = true;
                }
                else if (build)
                {
                    GameObject b = addBuilding(buildingObjects[rand], i, j);
                }
            }
        }
    }
    private void connectAdjacentCrossroad(GameObject b, int i, int j)
    {
        if (crossRoadGrid[i, j].GetComponent<CrossroadGrowth>().adjacentBuildings == null) { 
            crossRoadGrid[1, 1].GetComponent<CrossroadGrowth>().adjacentBuildings = new List<GameObject>();
        }
        crossRoadGrid[i, j].GetComponent<CrossroadGrowth>().adjacentBuildings.Add(b);
    }
    private void winGame()
    {
        Debug.Log("You won!");
        GameSceneSwitcher sceneSwitcher = gameObject.AddComponent<GameSceneSwitcher>();
        sceneSwitcher.SwitchToWinScene();
    }
    private IEnumerator CheckIfGameLost()
    {
        yield return new WaitForEndOfFrame();
        if (GameObject.FindGameObjectsWithTag("Tree").Length == 0)
        {
            LoseGame();
        }
    }
    private void LoseGame()
    {
        Debug.Log("You lost!");
        GameSceneSwitcher sceneSwitcher = gameObject.AddComponent<GameSceneSwitcher>();
        sceneSwitcher.SwitchToLooseScene();
    }
    private GameObject addBuilding(GameObject building, int i, int j)
    {
        Vector3 positionBuilding = new Vector3(i * pointDistance + ((pointDistance / 2) + 0.5f), 0, j * pointDistance + ((pointDistance / 2) + 0.5f));
        int rotation = UnityEngine.Random.Range(0, roationOptions.Length);
        GameObject b = Instantiate(building, positionBuilding, Quaternion.Euler(new Vector3(0, roationOptions[rotation], 0)));
        // b.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        connectAdjacentCrossroadsToBuilding(b, i, j);
        buildings.Add(b);
        return b;
    }
}
