using UnityEngine;
using System.Collections.Generic;

using System.Threading;
using com.youvisio;

public class NoiseForm : MonoBehaviour {

    public Vector2 cellsPerTile = new Vector2(50, 50);
    public Vector2 cellSize = new Vector2(1, 1);
    private Vector2 tileSize = new Vector2(50, 50);

    public int sideLength = 3;
    public int lodFactor = 2;//how much the detail is reduced
    public int lodDistance = 3;//how many cells away from the centre it switches to low detail

    [HideInInspector]
    public float maxY;

    TextureGenerator textureGenerator;

    GameObject[] tiles;
    GameObject player;
    Texture2D texture;

    Sampler[] samplers;

    public int centre
    {
        get { return (int)((tiles.Length / 2) / sideLength); }//coordinates are equal at the centre, so this works as x and y
    }

    public bool withinLodDist(int cell)//converts the tile's number in the tiles array to a 2d co-ordinate
    {
        return withinLodDist(cell % sideLength, cell / sideLength);//the :this(){} syntax didn't seem to be working
    }

    public bool withinLodDist(int x, int y)//determines if the specified cell is within the LOD distance from the centre
    {
        if ((x >= centre - lodDistance && x <= centre + lodDistance) && (y >= centre - lodDistance && y <= centre + lodDistance))
        {
            //Debug.Log(x + " " + y + " is within lod distance");
            return true;
        }

        return false;
    }

    public float GetHeight(Vector3 pos)
    {
        pos.y = float.MaxValue;
        RaycastHit hitInfo;
        bool collided = Physics.Raycast(pos, Vector3.down, out hitInfo);
        if (collided)
        {
            return hitInfo.point.y;
        }
        else
        {
            return hitInfo.point.y; //  SampleCell(pos);
        }
    }
    
    private void CreateTiles()
    {
        // The position of the bottom left tile
        Vector3 bottomLeft = new Vector3();
        bottomLeft.x = transform.position.x - (tileSize.x);
        bottomLeft.z = transform.position.z - (tileSize.y);

        int tileIndex = 0;

        for (int z = 0; z < sideLength; z ++)
        {
            for (int x = 0; x < sideLength; x ++)
            {
                if (withinLodDist(x, z))
                {
                    bottomLeft.x = transform.position.x - (tileSize.x);
                    bottomLeft.z = transform.position.z - (tileSize.y);
                }
                else
                {
                    bottomLeft.x = transform.position.x - (tileSize.x) + ((tileSize.x / 2) * (lodFactor - 1));
                    bottomLeft.z = transform.position.z - (tileSize.y) + ((tileSize.y / 2) * (lodFactor - 1));
                }

                GameObject tile = new GameObject();
                tile.transform.parent = this.transform;
                MeshRenderer renderer = tile.AddComponent<MeshRenderer>();
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = true;
                Mesh mesh = tile.AddComponent<MeshFilter>().mesh;
                mesh.Clear();
                tile.AddComponent<MeshCollider>();
                Vector3 tilePos = new Vector3();
                tilePos.x = bottomLeft.x + (x * tileSize.x);
                tilePos.z = bottomLeft.z + (z * tileSize.y);
                tilePos.y = transform.position.y;
                tile.transform.position = tilePos;

                if (withinLodDist(x, z))
                {
                    GenerateTile(tile, false);//should be false
                }
                else
                {
                    GenerateTile(tile, true);//should be true
                }

                tiles[tileIndex ++] = tile;

                renderer.material.color = RandomTextureGenerator.RandomColor();
            }
        }
    }

    private int FindTile(Vector3 pos)
    {       
        for (int i = 0; i < tiles.Length; i ++)
        {
            GameObject tile = tiles[i];
            Vector3 tileBottomLeft = new Vector3(
                tile.transform.position.x - (tileSize.x / 2)
                , 0
                , tile.transform.position.z - (tileSize.y / 2)
                );
            Vector3 tileTopRight = new Vector3(
                tile.transform.position.x + (tileSize.x / 2)
                , 0
                , tile.transform.position.z + (tileSize.y / 2)
                );
            if (pos.x > tileBottomLeft.x && pos.x <= tileTopRight.x && pos.z > tileBottomLeft.z && pos.z <= tileTopRight.z)
            {
                return i;
            }
        }
        return -1;
    }

    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, size);
    }

    void MaxY(float y)
    {
        if (y > maxY)
        {
            maxY = y;
        }
    }

    class Arg
    {
        public GameObject t;
        public Vector3 position;
    }

    class GeneratedMesh
    {
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uvs;
        public Color[] colours;
        public int[] triangles;
    }
    
    List<BackgroundWorker> workers = new List<BackgroundWorker>();

    void GenerateTile(GameObject tileGameObject, bool lod)
    {
        BackgroundWorker backgroundWorker = new BackgroundWorker();

        MeshRenderer renderer = tileGameObject.GetComponent<MeshRenderer>();
        Mesh mesh = tileGameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        backgroundWorker.DoWork += (o, a) =>
        {
            Arg aa = (Arg)a.Argument;            
            a.Result = GenerateTileAsync(aa.t, aa.position, lod);
        };

        backgroundWorker.RunWorkerCompleted += (o, a) =>
        {
            GeneratedMesh gm = (GeneratedMesh)a.Result;
            
            mesh.vertices = gm.vertices;
            mesh.uv = gm.uvs;
            mesh.triangles = gm.triangles;
            mesh.RecalculateNormals();

            //renderer.material.color = RandomTextureGenerator.RandomColor();

            tileGameObject.GetComponent<MeshCollider>().sharedMesh = null;
            tileGameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        };

        Arg args = new Arg();
        args.t = tileGameObject;
        args.position = tileGameObject.transform.position;
        workers.Add(backgroundWorker);
        backgroundWorker.RunWorkerAsync(args);
    }

    GeneratedMesh GenerateTileAsync(GameObject tileGameObject, Vector3 position, bool lod)
    {        
        int verticesPerSegment = 6;

        //adjusting these values for LOD if necessary
        Vector2 cellsPerTile;
        Vector2 tileSize;
        Vector2 cellSize;

        if (lod)
        {
            cellsPerTile = new Vector2(this.cellsPerTile.x / lodFactor, this.cellsPerTile.y / lodFactor);
            tileSize = new Vector2(this.tileSize.x * lodFactor, this.tileSize.y * lodFactor);
            cellSize = new Vector2(this.cellSize.x * lodFactor, this.cellSize.y * lodFactor);
        }
        else
        {
            cellsPerTile = this.cellsPerTile;
            tileSize = this.tileSize;
            cellSize = this.cellSize;
        }

        int vertexCount = verticesPerSegment * ((int)cellsPerTile.x) * ((int)cellsPerTile.y);

        GeneratedMesh gm = new GeneratedMesh();

        gm.vertices = new Vector3[vertexCount];
        gm.normals = new Vector3[vertexCount];
        gm.uvs = new Vector2[vertexCount];
        gm.triangles = new int[vertexCount];

        gm.colours = new Color[vertexCount];

        int vertex = 0;

        // What cell is x and z for the bottom left of this tile in world space
        Vector3 tileBottomLeft = new Vector3();
        tileBottomLeft.x = - (tileSize.x) / 2;
        tileBottomLeft.z = - (tileSize.y) / 2;

        for (int z = 0; z < cellsPerTile.y; z++)
        {
            for (int x = 0; x < cellsPerTile.x; x++)
            {
                int startVertex = vertex;
                // Calculate some stuff
                Vector3 cellBottomLeft = tileBottomLeft + new Vector3(x * cellSize.x, 0, z * cellSize.y);
                Vector3 cellTopLeft = tileBottomLeft + new Vector3(x * cellSize.x, 0, (z + 1) * cellSize.y);
                Vector3 cellTopRight = tileBottomLeft + new Vector3((x + 1) * cellSize.x, 0 , (z + 1) * cellSize.y);
                Vector3 celBottomRight = tileBottomLeft + new Vector3((x + 1) * cellSize.x, 0, z * cellSize.y);

                // Add all the samplers together to make the height
                Vector3 cellWorldCoords = position + tileBottomLeft  + new Vector3(x * cellSize.x, 0, z * cellSize.y);
                foreach(Sampler sampler in samplers)
                {
                    cellBottomLeft.y += sampler.Sample(cellWorldCoords.x, cellWorldCoords.z);
                    cellTopLeft.y += sampler.Sample(cellWorldCoords.x, cellWorldCoords.z + cellSize.y);
                    cellTopRight.y += sampler.Sample(cellWorldCoords.x + cellSize.x, cellWorldCoords.z + cellSize.y);
                    celBottomRight.y += sampler.Sample(cellWorldCoords.x + cellSize.x, cellWorldCoords.z);
                }
                                   
                // Make the vertices
                gm.vertices[vertex++] = cellBottomLeft;
                gm.vertices[vertex++] = cellTopLeft;
                gm.vertices[vertex++] = cellTopRight;
                gm.vertices[vertex++] = cellTopRight;
                gm.vertices[vertex++] = celBottomRight;
                gm.vertices[vertex++] = cellBottomLeft;

                // Make the normals, UV's and triangles                
                for (int i = 0; i < 6; i++)
                {
                    int vertexIndex = startVertex + i;
                    gm.triangles[vertexIndex] = vertexIndex;
                    gm.uvs[vertexIndex] = new Vector2(x / cellsPerTile.x, z / cellsPerTile.y);
                }
            }            
        }
        return gm;
    }

    void Awake()
    {
        textureGenerator = GetComponent<TextureGenerator>();
        if (textureGenerator != null)
        {
            texture = textureGenerator.GenerateTexture();
        }
    }

    void detectLODTransition(int cur, int prev, GameObject[] newTiles)
    {
        if (withinLodDist(cur) && !withinLodDist(prev))//tile has entered high detail zone
        {
            newTiles[cur].transform.Translate(new Vector3(-((tileSize.x / 2) * (lodFactor - 1)), 0.0f, -((tileSize.y / 2) * (lodFactor - 1))));
            GenerateTile(newTiles[cur], false);
        }
        else if (!withinLodDist(cur) && withinLodDist(prev))//tile has exited high detail zone
        {
            newTiles[cur].transform.Translate(new Vector3(((tileSize.x / 2) * (lodFactor - 1)), 0.0f, ((tileSize.y / 2) * (lodFactor - 1))));
            GenerateTile(newTiles[cur], true);
        }
    }

    void tilesForward()
    {
        GameObject[] newTiles = new GameObject[tiles.Length];
        
        for (int i = 0; i < tiles.Length; i++)
        {
            if (i < (tiles.Length) - sideLength)//if it's not on the last row
            {
                newTiles[i] = tiles[i + sideLength];
                detectLODTransition(i, i + sideLength, newTiles);
            }
            else
            {
                newTiles[i] = tiles[i - (sideLength * (sideLength - 1))];
            }
        }

        tiles = newTiles;

        for (int i = (tiles.Length) - sideLength; i < tiles.Length; i++)
        {
            tiles[i].transform.Translate(new Vector3(0.0f, 0.0f, tileSize.y * sideLength));
            GenerateTile(tiles[i], true);
        }

        //Debug.Log("forward");
    }

    void tilesBack()
    {
        GameObject[] newTiles = new GameObject[tiles.Length];

        for (int i = 0; i < tiles.Length; i++)
        {
            if (i < sideLength)//if it's on the first row
            {
                newTiles[i] = tiles[i + (sideLength * (sideLength - 1))];
            }
            else
            {
                newTiles[i] = tiles[i - sideLength];
                detectLODTransition(i, i - sideLength, newTiles);
            }
        }

        tiles = newTiles;

        for (int i = 0; i < sideLength; i++)
        {
            tiles[i].transform.Translate(new Vector3(0.0f, 0.0f, -tileSize.y * sideLength));
            GenerateTile(tiles[i], true);
        }
    }

    void tilesLeft()
    {
        GameObject[] newTiles = new GameObject[tiles.Length];

        for (int i = 0; i < tiles.Length; i++)
        {
            if (i % sideLength == 0)
            {
                newTiles[i] = tiles[i + (sideLength - 1)];
            }
            else
            {
                newTiles[i] = tiles[i - 1];
                detectLODTransition(i, i - 1, newTiles);
            }
        }

        tiles = newTiles;

        for (int i = 0; i < tiles.Length; i += sideLength)
        {
            tiles[i].transform.Translate(-tileSize.x * sideLength, 0.0f, 0.0f);
            GenerateTile(tiles[i], true);
        }
    }

    void tilesRight()
    {
        GameObject[] newTiles = new GameObject[tiles.Length];

        for (int i = 0; i < tiles.Length; i++)
        {
            if (i % sideLength == sideLength - 1)
            {
                newTiles[i] = tiles[i - (sideLength - 1)];
            }
            else
            {
                newTiles[i] = tiles[i + 1];
                detectLODTransition(i, i + 1, newTiles);
            }
        }

        tiles = newTiles;

        for (int i = sideLength - 1; i < tiles.Length; i += sideLength)
        {
            tiles[i].transform.Translate(tileSize.x * sideLength, 0.0f, 0.0f);
            GenerateTile(tiles[i], true);
        }
    }

    void Start()
    {
        if (sideLength % 2 == 0)
        {
            Debug.Log("Sidelength must be an uneven number, changing value to " + ++sideLength);
        }

        tiles = new GameObject[sideLength * sideLength];

        tileSize = new Vector2(cellSize.x * cellsPerTile.x, cellSize.y * cellsPerTile.y);

        samplers = GetComponents<Sampler>();
        if (samplers == null)
        {
            Debug.Log("Sampler is null! Add a sampler to the NoiseForm");
        }

        CreateTiles();
        Random.seed = 42;
        player = GameObject.FindGameObjectWithTag("Player");

        
       foreach(GameObject tile in tiles)
       {
           tile.GetComponent<Renderer>().material.SetTexture(0, texture);
       }

        player.transform.Translate(((sideLength / 2) * tileSize.x) - tileSize.x / 2, 0, ((sideLength / 2) * tileSize.y - tileSize.y / 2));
    }
   
    void Update () {
        int tileIndex = FindTile(player.transform.position);
        //GameManager.PrintFloat("Tile: ", tileIndex);

        int centre = (sideLength * sideLength) / 2;

        if (tileIndex == centre + sideLength)
        {
            // Player has moved forward one tile
            tilesForward();
            //Debug.Log("forward");
        }
        else if (tileIndex == centre - sideLength)
        {
            // Player has moved backward one tile, so regenerate the 2nd row
            tilesBack();
            //Debug.Log("back");
        }
        else if (tileIndex == centre - 1)
        {
            // Player has moved left one tile, so regenerate the 0th col
            tilesLeft();
            //Debug.Log("left");
        }
        else if (tileIndex == centre + 1)
        {
            // Player has moved right one tile, so regenerate the 0th col
            tilesRight();
            //Debug.Log("right");
        }
        else if (tileIndex == centre + sideLength - 1)
        {
            //forward left diagonal
            tilesForward();
            tilesLeft();
            //Debug.Log("forward left");
        }
        else if (tileIndex == centre + sideLength + 1)
        {
            //forward right diagonal
            tilesForward();
            tilesRight();
            //Debug.Log("forward right");
        }
        else if (tileIndex == centre - sideLength - 1)
        {
            //back left diagonal
            tilesBack();
            tilesLeft();
            //Debug.Log("back left");
        }
        else if (tileIndex == centre - sideLength + 1)
        {
            //back right diagonal
            tilesBack();
            tilesRight();
            //Debug.Log("back right");
        }

        for (int i = workers.Count - 1 ; i >=  0 ; i --)
        {
            if (workers[i].IsBusy) 
            {
                workers[i].Update();
            }
            else
            {
                workers.Remove(workers[i]);
            }
        }
    }
}
