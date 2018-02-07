using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenerateMapIcon : MonoBehaviour {

    GameObject player;
    [SerializeField]
     Sprite playerSprite;

    [SerializeField]
     Sprite circleSprite;
     Sprite boxSprite;

    Texture2D circleTex;
    Texture2D playerTex;
    Material mapTerrainMaterial;
    Texture2D voidBoundryLineTexture;

    public enum IconType
    {
        DarkStar,
        Player,
        Planet,
        Terrain,
        Creature, 
        VoidRing,
        Hook
    }

    public IconType ourIconType;

    private void OnEnable()
    {
        if(ourIconType == IconType.DarkStar)
        DarkStar.AdjustLuminosity += this.GrowIcon;
    }

    private void OnDisable()
    {
        if(ourIconType == IconType.DarkStar)
        DarkStar.AdjustLuminosity -= this.GrowIcon;
    }

    void GrowIcon(float adjustAmount)
    {

    }

    void DestroyIcon()
    {
        //add shatter effect to planet
    }

    // List<Collider2D> ourCombinedColliders = new List<Collider2D>();


    void VisualizeColliders()
    {

        if (ourIconType == IconType.Player)
        {
            GameObject mapSprite = new GameObject();
            Collider2D col = GetComponentInParent<Collider2D>();
            mapSprite.transform.parent = this.gameObject.transform;
            SpriteRenderer ourSpriteRenderer = mapSprite.AddComponent<SpriteRenderer>();
            ourSpriteRenderer.sprite = Resources.Load<Sprite>("MapIconSprites/OpaqueTriangle");
            mapSprite.transform.localScale = new Vector2(col.bounds.size.x * 2.0f, col.bounds.size.y * 2.0f);
            ourSpriteRenderer.color = Color.green;
            mapSprite.transform.parent = this.gameObject.transform;
            mapSprite.transform.position = player.transform.position;


        }
        else if (ourIconType == IconType.Planet)
        {
            GameObject mapSprite = new GameObject();
            Collider2D col = GetComponentInParent<Collider2D>();
            SpriteRenderer ourSpriteRenderer = mapSprite.AddComponent<SpriteRenderer>();
            ourSpriteRenderer.sprite = Resources.Load<Sprite>("MapIconSprites/OpaqueCircle");
            ourSpriteRenderer.color = Color.cyan;

            float divisible = ourSpriteRenderer.bounds.size.x;


            mapSprite.transform.localScale = new Vector2(col.bounds.size.x/divisible, col.bounds.size.y/divisible);
            mapSprite.transform.parent = this.gameObject.transform;
            mapSprite.transform.localPosition = col.offset;




        }
        else if(ourIconType == IconType.DarkStar)
        {
            GameObject mapSprite = new GameObject();
            Collider2D col = GetComponentInParent<Collider2D>();
            SpriteRenderer ourSpriteRenderer = mapSprite.AddComponent<SpriteRenderer>();
            ourSpriteRenderer.sprite = Resources.Load<Sprite>("MapIconSprites/OpaqueCircle");
            ourSpriteRenderer.color = new Color(138f/255, 43f/255,226f/255);

            float divisible = ourSpriteRenderer.bounds.size.x;


            mapSprite.transform.localScale = new Vector2(col.bounds.size.x/divisible, col.bounds.size.y/divisible);
            mapSprite.transform.parent = this.gameObject.transform;
            mapSprite.transform.localPosition = col.offset;


        }
        else if(ourIconType == IconType.Terrain)
        {
            CreateMeshFromFerr2DTerrain();
        }
        else { 

            List<Collider2D> ourCombinedColliders = GetComponentsInParent<Collider2D>().ToList();

            foreach (Collider2D col in ourCombinedColliders)
            {
                GameObject mapSprite = new GameObject();

                if (col.GetType() == typeof(CircleCollider2D))
                {
                    SpriteRenderer ourSpriteRenderer = mapSprite.AddComponent<SpriteRenderer>();
                    ourSpriteRenderer.sprite = Resources.Load<Sprite>("MapIconSprites/OpaqueCircle");
                    if (ourIconType == IconType.Creature)
                    {
                        ourSpriteRenderer.color = Color.red;
                    }
                    else if(ourIconType == IconType.Hook)
                    {
                        ourSpriteRenderer.color = Color.yellow;
                  
                    }
                    float divisible = ourSpriteRenderer.bounds.size.x;

                    mapSprite.transform.localScale = new Vector2(col.bounds.size.x/divisible, col.bounds.size.y/divisible);
                    mapSprite.transform.parent = this.gameObject.transform;
                    mapSprite.transform.localPosition = col.offset;

                }
                else if (col.GetType() == typeof(BoxCollider2D))
                {

                    SpriteRenderer ourSpriteRenderer = mapSprite.AddComponent<SpriteRenderer>();
                    ourSpriteRenderer.sprite = circleSprite;
                    mapSprite.transform.localPosition = col.offset;
                    mapSprite.transform.localScale = new Vector2(col.bounds.extents.x * 2, col.bounds.extents.y * 2);

                }

            }
        }
        foreach(Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("MapLayer");
        }
        
    }

    void CreateMeshFromFerr2DTerrain()
    {
        Ferr2D_Path ourPath = GetComponentInParent<Ferr2D_Path>();
        Mesh terrainMesh = GetComponentInParent<MeshFilter>().mesh;
        GameObject mapSprite = new GameObject();
        mapSprite.transform.parent = this.gameObject.transform;
        mapSprite.transform.localPosition = new Vector2(0, 0);
        mapSprite.transform.rotation = mapSprite.transform.parent.rotation;
        MeshFilter mf = mapSprite.AddComponent<MeshFilter>();
        MeshRenderer mr = mapSprite.AddComponent<MeshRenderer>();
        mf.mesh = terrainMesh;
        mr.material = mapTerrainMaterial;
        mapSprite.transform.localScale = new Vector3(1, 1, 1);


    }


    void CreateMeshFromPolygonCollider2D(PolygonCollider2D col)
    {

        Debug.Log("This was triggered");
        GameObject mapSprite = new GameObject();
        mapSprite.transform.parent = this.gameObject.transform;
        MeshFilter mf = mapSprite.AddComponent<MeshFilter>();
        Mesh mesh = mf.mesh;
        List<Vector2> points = col.points.ToList();
        List<Vector3> iconVerticies = new List<Vector3>();
        foreach (Vector2 point in col.points)
        {
            iconVerticies.Add((Vector3)point);

        }

        mesh.vertices = iconVerticies.ToArray();



    }
        

    void VoidBorderRingGenerator()
    {
        CircleCollider2D col = GetComponentInParent<CircleCollider2D>();
        GameObject borderRing = new GameObject();

        SpriteRenderer ourSpriteRenderer = borderRing.AddComponent<SpriteRenderer>();
        ourSpriteRenderer.sprite = Resources.Load<Sprite>("MapIconSprites/DottedCircle");
        ourSpriteRenderer.color = Color.magenta;
        borderRing.transform.parent = this.gameObject.transform;
        Bounds bounds = ourSpriteRenderer.bounds;

        float divisible = ourSpriteRenderer.bounds.size.x;

   

        borderRing.transform.localScale = new Vector2(col.bounds.size.x/divisible, col.bounds.size.y/divisible);

        borderRing.transform.localPosition = col.offset;

        borderRing.layer = this.gameObject.layer;

    }

    // Use this for initialization
    void Start () {

        player = GameObject.Find("Player");

        //List<Sprite> testList = Resources.FindObjectsOfTypeAll<Sprite>().ToList();
        //foreach(Sprite s in testList)
        //{
        //    Debug.Log(s);
        //}
        this.transform.localPosition = new Vector2(0, 0);
        Texture2D Circletex = Resources.Load<Texture2D>("MapIconSprites/OpaqueCircle");
        circleTex = Circletex;

        Texture2D playerTexture = Resources.Load<Texture2D>("MapIconSprites/OpaqueTriangle");
        playerTex = playerTexture;

        Material terrainMat = Resources.Load<Material>("MapIconSprites/TerrainMaterialForMap");
        mapTerrainMaterial = terrainMat;

        if (ourIconType == IconType.VoidRing)
        {
            VoidBorderRingGenerator();
        }
        else
        {
            //turn this back on if you ever find a use for it
           // VisualizeColliders();
        }
        
        
	}

    // Update is called once per frame
    void Update() {

        


        //if (ourIconType == IconType.Planet) {


        //}
        //else if (ourIconType == IconType.Creature)
        //{

        //}
		
	}
}
