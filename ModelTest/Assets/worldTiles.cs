using UnityEngine;
using System.Collections;

public class worldTiles : MonoBehaviour {

    public GameObject groundObject;
    public GameObject buildingObject;//feasible to have more than one building type?
    public GameObject player;//camera? maybe not, depending on AI

    public int tileGrid = 5;
    public int buildingGrid = 4;

    public float threshold = 0.5f;

    public float heightOffset = 10.0f;//Unity doesn't seem to support different noise seeds, so an offset will have to do.

    bool once = false;

    Tile[] tiles;
    public Vector2 pos;

    // Use this for initialization
    void Start () {
        tiles = new Tile[tileGrid * tileGrid];

        placeTiles((int)pos.x, (int)pos.y);
    }
	
	// Update is called once per frame
	void Update () {
	    /*if (Input.anyKey && !once)
        {
            //PlaceTile(new Vector2(2, 0));
            //PlaceTile(new Vector2(3, 0));

            //Tile[] tiles = new Tile[tileGrid * tileGrid];

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = getTile(new Vector2(i % tileGrid, i / tileGrid));
            }

            once = true;
        }*/

        /*if (Vector3.Distance(player.transform.position, centre) > (tileScale * 2))
        {
            //new tiles
            Vector2 shift = (Vector2)player.transform.position - centre;
            //shiftTiles(shift.normalized);

        }*/
	}

    private Vector2 centre
    {
        get { return new Vector2(
            ((tileGrid / 2) * tileScale) + (pos.x * tileScale),
            ((tileGrid / 2) * tileScale) + (pos.y * tileScale)); }
    }

    private float tileScale
    {
        get { return groundObject.GetComponent<Renderer>().bounds.size.x; }
    }

    void placeTiles(int x, int y)
    {
        Tile[] newTiles = new Tile[tiles.Length];

        for (int i = 0; i < tiles.Length; i++)
        {
            newTiles[i] = getTile(new Vector2((i % tileGrid) + x, (i / tileGrid) + y));
        }

        tiles = newTiles;
    }

    /*void shiftTiles(int x, int y)
    {
        Vector2 oldPos = pos;
        pos = new Vector2(x, y);
        Vector2 diff = pos - oldPos;

        Tile[] newTiles = new Tile[tiles.Length];

        for (int i = 0; i < tiles.Length; i++)
        {
            Vector2 coords = index2D(i) + pos;
            for (int j = 0; j < tiles.Length; j++)
            {
                if (tiles[j].pos == coords)
                {
                    newTiles[i] = tiles[j];
                    tiles[j] = null;
                    break;
                }
            }

            if (newTiles[i] == null)
            {
                newTiles[i] = getTile(new Vector2(i % tileGrid, i / tileGrid));
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
            {
                Destroy(tiles[i].g);
            }
        }

        tiles = newTiles;
    }

    void shiftTiles(Vector2 coords)
    {
        shiftTiles((int)coords.x, (int)coords.y);
    }*/

    Vector2 index2D(int index)//converts the 1D index to 2D coordinates
    {
        return new Vector2(index % tileGrid, index / tileGrid);
    }

    Tile getTile(Vector2 pos)
    {
        return new Tile(pos, buildingGrid, threshold, groundObject, buildingObject);
    }

    class Tile
    {
        GameObject ground;
        GameObject[] buildings;
        public Vector2 pos;

        public Tile(Vector2 Pos, int buildingGrid, float threshold, GameObject groundObject, GameObject buildingObject)
        {
            pos = Pos;

            PlaceTile(buildingGrid, threshold, groundObject, buildingObject);
        }

        void PlaceTile(int buildingGrid, float threshold, GameObject groundObject, GameObject buildingObject)
        {
            ground = GameObject.Instantiate(groundObject);
            ground.SetActive(true);
            buildings = new GameObject[buildingGrid * buildingGrid];

            float tileScale = ground.GetComponent<Renderer>().bounds.size.x;//assuming tile is square

            ground.transform.position = new Vector3(tileScale * pos.x, 0, tileScale * pos.y);

            for (int i = 0; i < buildings.Length; i++)
            {
                float noise = Mathf.PerlinNoise(pos.x + ((i % buildingGrid) * 0.1f), pos.y + ((i / buildingGrid) * 0.1f));

                if (noise >= threshold)
                {
                    buildings[i] = GameObject.Instantiate(buildingObject);

                    buildings[i].transform.position = new Vector3(ground.transform.position.x - (tileScale / 2), 10, ground.transform.position.z - (tileScale / 2));
                    buildings[i].transform.Translate(((tileScale / buildingGrid) * (i % buildingGrid)), 0, ((tileScale / buildingGrid) * (i / buildingGrid)));

                    buildings[i].transform.localScale = buildings[i].transform.localScale + new Vector3(0.0f, ((Mathf.PerlinNoise(pos.x + 10 + (i * 0.1f), pos.y + 10 + (i * 0.1f))) * 2) * buildings[i].transform.localScale.y, 0.0f);
                    buildings[i].SetActive(true);

                    //attempt at varying colours - not working
                    //float shadeOffset = 1.0f;//can't change it in the editor, but this should suffice
                    //float shade = Mathf.PerlinNoise((pos.x * 5) + shadeOffset + (i % buildingGrid), (pos.y * 5) + shadeOffset + (i / buildingGrid)) + 0.1f;
                    //buildings[i].GetComponent<Renderer>().material.color = new Color(shade, shade, shade);

                    if (noise <= threshold + 0.01f)
                    {
                        buildings[i].GetComponent<Renderer>().material.color = new Color(0.8f, 0.2f, 0.2f);
                    }
                    else if (noise >= 0.91f)
                    {
                        buildings[i].GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.8f);
                    }
                }
                else
                {
                    buildings[i] = null;
                }
            }
        }

        
    }
}
