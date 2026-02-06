using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ArrayUtility;

public class ChunkHandler : MonoBehaviour
{

	int chunkWidth = 16;
	int chunkHeight = 100;
	public int viewDistance;
	public int viewDistanceMax;
	public int LOD;
	public int LODMax;
	public Gradient gradient;
	public AnimationCurve heightCurve;

	GameObject player;
	Dictionary<Vector2Int, Chunk> chunkDic = new Dictionary<Vector2Int, Chunk>(); 
	Camera camera;

    Vector3 gizmoCenter;
    Vector3 gizmoSize;

    // Start is called before the first frame update
    void Start()
    {
    	player = GameObject.Find("Player");
    	camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //#### INFINITE CHUNK RENDERING
        Vector2Int pos = new Vector2Int((int)(player.transform.position.x/chunkWidth), (int)(player.transform.position.z/chunkWidth));
        
        for (int x=pos.x-viewDistanceMax; x<pos.x+viewDistanceMax; x++){
        	for (int y=pos.y-viewDistanceMax; y<pos.y+viewDistanceMax; y++){
        		Vector2Int iPos = new Vector2Int(x,y);
        		if (Vector2.Distance(pos, iPos) <= viewDistance){
        			if (!chunkDic.ContainsKey(iPos)){
        				Chunk newChunk = new Chunk(iPos * chunkWidth, chunkWidth,chunkHeight, LOD, gradient, heightCurve);
        				chunkDic.Add(iPos, newChunk);
        			} else if (chunkDic[iPos].scale > LOD){
        				Destroy(chunkDic[iPos].chunkObject);
        				chunkDic[iPos].DestroyChunk();
        				chunkDic[iPos] = new Chunk(iPos * chunkWidth, chunkWidth,chunkHeight, LOD, gradient, heightCurve);
        			}
        		}

        		else if (Vector2.Distance(pos, iPos) <= viewDistanceMax){
        			if (!chunkDic.ContainsKey(iPos)){
        				Chunk newChunk = new Chunk(iPos * chunkWidth, chunkWidth,chunkHeight, LODMax, gradient, heightCurve);
        				chunkDic.Add(iPos, newChunk);
        			}
        		}
        	}
        }
        
        
        if (Input.GetMouseButtonDown(0)){
    		
    		RaycastHit hit;
    		Ray ray = camera.ViewportPointToRay(new Vector3(0.5f,0.5f));
    		if (Physics.Raycast(ray, out hit)) {
    			BlockEdit(hit.point, 6, false);
    		}
    		
    	}

        if (Input.GetMouseButtonDown(1)){
            
            RaycastHit hit;
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f,0.5f));
            if (Physics.Raycast(ray, out hit)) {
                BlockEdit(hit.point, 6, true);
            }
            
        }
    }

    List<Chunk> editPosition(Vector3Int pos, float value, int offsetX = 0, int offsetY = 0){
        int negX = 0;
        int negZ = 0;
        if (pos.x < 0){negX=1;}
        if (pos.z < 0){negZ=1;}
    	int chunkX = (pos.x)/(chunkWidth) + offsetX - negX;
    	int chunkY = (pos.z)/(chunkWidth) + offsetY - negZ;
    	Chunk chunk = chunkDic[new Vector2Int(chunkX, chunkY)];
    	float scale = chunk.scale;
    	//int _scale = (int)scale;

    	List<Chunk> chunks = new List<Chunk>() {chunk};
    	pos = vectorStep(pos, scale);

        // gigabrain code that handles chunk borders
    	if (offsetX == 0 && offsetY == 0){ //  || (pos.x - chunk.chunkPos.x != 0 && pos.z - chunk.chunkPos.y != 0)


    		if ((pos.x)%(chunkWidth) == 0){ // x-axis chunk borders
    			if (pos.x - chunk.chunkPos.x != 0){ // right-side x axis
                    chunks.AddRange(editPosition(pos, value, 1,0));
    			     if ((pos.z)%(chunkWidth) == 0){
                        if (pos.z - chunk.chunkPos.y != 0){
    				        chunks.AddRange(editPosition(pos, value, 1,1));
                        } else {
                            chunks.AddRange(editPosition(pos, value, 1,-1));
                        }
    			     }
                } else { // left-side x axis
                    chunks.AddRange(editPosition(pos, value, -1,0)); 
                    if ((pos.z)%(chunkWidth) == 0){
                        if (pos.z - chunk.chunkPos.y != 0){
                            chunks.AddRange(editPosition(pos, value, -1,1));
                        } else {
                            chunks.AddRange(editPosition(pos, value, -1,-1));
                        }
                     }
                }
    		}



    		if ((pos.z)%(chunkWidth) == 0){ // z-axis chunk borders
                if (pos.z - chunk.chunkPos.y != 0){
                    chunks.AddRange(editPosition(pos, value, 0,1));
                } else {
                    chunks.AddRange(editPosition(pos, value, 0,-1));
                }
    		}
    		
    	}
        
    	//chunk.DestroyChunk();
    	Vector3Int posInChunk = (pos - new Vector3Int(chunk.chunkPos.x, 0, chunk.chunkPos.y));
    	posInChunk.x = (int)(posInChunk.x / scale);
    	posInChunk.z = (int)(posInChunk.z / scale);
    	//Debug.Log(chunk.chunkPos);
    	chunk.terrainMap[posInChunk.x,posInChunk.y,posInChunk.z] = value;
    	
    	
    	return chunks;
    }

    Vector3Int vectorStep(Vector3Int pos, float _scale){
    	if (_scale <= 1) return pos;
    	int scale = (int)_scale;
    	pos.x = pos.x - pos.x % scale;
    	pos.z = pos.z - pos.z % scale;
    	return pos;
    }

    //List<Vector3Int> points = new List<Vector3Int>();

    public void BlockEdit(Vector3 _pos, int width, bool remove){
    	if (width % 2 != 0){
    		Debug.Log("Width must be a multiple of 2");
    		return;
    	}
    	Vector3Int pos = Vector3Int.RoundToInt(_pos);

    	int value;
    	if (remove) value = 1;
    	else value = 0;

    	List<Chunk> chunkList = new List<Chunk>();
    	List<Chunk> curChunks;

        if (!remove){
        List<Collider> collisionList = new List<Collider>(Physics.OverlapBox(pos, new Vector3(width/2,width/2,width/2)));
        if (collisionList.Contains(player.transform.Find("Capsule").gameObject.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider)){
            return;
        }
        }

        //if (remove){
        //    gizmoCenter = pos;
        //    gizmoSize = new Vector3(width/2,width/2,width/2);
        //}

    	for (int x=pos.x-width/2; x<pos.x+width/2; x++){
    		for (int y=pos.y-width/2; y<pos.y+width/2; y++){
    			for (int z=pos.z-width/2; z<pos.z+width/2; z++){
                    if (x%LOD!=0 || z%LOD!=0) continue;
                    try {
    				    curChunks = editPosition(new Vector3Int(x,y,z), value);
                    } catch {
                        continue;
                    }
    				foreach (Chunk chunk in curChunks){
    					if (!chunkList.Contains(chunk)) chunkList.Add(chunk);
    				}

    			}
    		}
    	}
    	for (int i=0; i<chunkList.Count; i++){
    		chunkList[i].BuildMesh();
    	}
        
        //foreach (KeyValuePair<Vector2Int, Chunk> entry in chunkDic){ // rebuild every chunk mesh
        //    entry.Value.BuildMesh();
        //}

    }


    public void SphereEdit(Vector3 _pos, int diameter, bool remove){
        int value;
        if (remove) value = 1;
        else value = 0;

        int width = diameter;
        Vector3Int pos = Vector3Int.RoundToInt(_pos);

        if (width % 2 != 0){
            Debug.Log("Might not want to use non %2 width with sphere edit");
        }

        List<Chunk> chunkList = new List<Chunk>();
        List<Chunk> curChunks;

        if (!remove){
        List<Collider> collisionList = new List<Collider>(Physics.OverlapBox(pos, new Vector3(width/2,width/2,width/2)));
        if (collisionList.Contains(player.transform.Find("Capsule").gameObject.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider)){
            return;
        }
        }

        for (int x=pos.x-width/2; x<pos.x+width/2; x++){
            for (int y=pos.y-width/2; y<pos.y+width/2; y++){
                for (int z=pos.z-width/2; z<pos.z+width/2; z++){
                    if (x%LOD!=0 || z%LOD!=0) continue;
                    if (Vector3.Distance(new Vector3(x,y,z), pos) > diameter/2){ continue; Debug.Log("Skipped!");}
                    Debug.Log(Vector3.Distance(new Vector3(x,y,z), pos));
                    try {
                        curChunks = editPosition(new Vector3Int(x,y,z), value);
                    } catch {
                        continue;
                    }
                    foreach (Chunk chunk in curChunks){
                        if (!chunkList.Contains(chunk)) chunkList.Add(chunk);
                    }

                }
            }
        }
        for (int i=0; i<chunkList.Count; i++){
            chunkList[i].BuildMesh();
        }
        //foreach (KeyValuePair<Vector2Int, Chunk> entry in chunkDic){ // rebuild every chunk mesh
        //    entry.Value.BuildMesh();
        //}

    }

    void OnDrawGizmos(){
        if (gizmoCenter != null){
            Gizmos.DrawWireCube(gizmoCenter,gizmoSize);
        }
    }

/*
    void OnDrawGizmos(){
    	for (int i=0; i<points.Count; i++){
    		Gizmos.DrawSphere(points[i], 0.3f);
    	}

    	if (!chunkDic.ContainsKey(new Vector2Int(0,0))) return;
    	Chunk zeroChunk = chunkDic[new Vector2Int(0,0)];
    	for (int x=0; x<chunkWidth+1;x++){
    		for (int y=0; y<chunkHeight+1;y++){
    			for (int z=0; z<chunkWidth+1;z++){
    				if (zeroChunk.terrainMap[x,y,z] < 0.5f){
    					Gizmos.DrawSphere(terrainMap[x,y,z], 0.1f);
    				}
    			}
    		}
    	}

    } */
}
