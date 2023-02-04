using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossroad : MonoBehaviour
{
    [SerializeField] private List<GameObject> connectedCrossroads = new List<GameObject>();
    [SerializeField] private List<GameObject> adjacentBuildings = new List<GameObject>();

    private GameObject treeGameObject;
    private int treePrepStatus = 0;
    private int newBuildTreshhold;


    // Start is called before the first frame update
    void Start()
    {
        newBuildTreshhold = transform.parent.gameObject.GetComponent<CrossroadConstants>().crossroadNewBuildTreshhold;

        foreach (GameObject crossroad in connectedCrossroads)
        {
            crossroad.GetComponent<Crossroad>().EnsureStreetConnectionBothWays(this.gameObject);
        }
        foreach (GameObject building in adjacentBuildings)
        {
            building.GetComponent<Building>().EnsureBuildingConnectionBothWays(this.gameObject);
        }

        // Debug
        PlantTree();
    }

    public void EnsureStreetConnectionBothWays(GameObject otherGameobject)
    {
        if (!connectedCrossroads.Contains(otherGameobject))
        {
            connectedCrossroads.Add(otherGameobject);
        }
    }
    public void EnsureBuildingConnectionBothWays(GameObject otherGameobject)
    {
        if (!adjacentBuildings.Contains(otherGameobject))
        {
            adjacentBuildings.Add(otherGameobject);
        }
    }

    private void PrepareTree()
    {
        if (!HasTree() && CheckIfTreePreparable())
        {
            treePrepStatus++;
            if(treePrepStatus >= newBuildTreshhold) 
            {
                PlantTree();
                treePrepStatus = 0;
            }
        }
    }

    private bool CheckIfTreePreparable()
    {
        foreach (GameObject crossroad in connectedCrossroads)
        {
            if (crossroad.GetComponent<Crossroad>().HasTree())
            {
                return true;
            }
        }

        return false;
    }

    private void PlantTree()
    {
        GameObject treePrefab = transform.parent.gameObject.GetComponent<CrossroadConstants>().treePrefab;
        treeGameObject = Instantiate(treePrefab, transform.GetChild(0).position, transform.GetChild(0).rotation);
        treeGameObject.transform.parent = this.transform;

        RedrawAllAdjacentBuildings();
    }

    public void DestroyTree()
    {
        Destroy(treeGameObject);
        treeGameObject = null;
        RedrawAllAdjacentBuildings();
    }

    private void RedrawAllAdjacentBuildings()
    {
        foreach (GameObject building in adjacentBuildings)
        {
            building.GetComponent<Building>().RedrawBuilding();
        }
    }

    public bool HasTree() 
    {
        return treeGameObject != null;
    }

    private void RedrawRoots()
    {
        foreach (GameObject crossroad in connectedCrossroads)
        {
            if (crossroad.GetComponent<Crossroad>().HasTree())
            {
                //return true;
            }
        }
    }
}
