using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
	ChunkHandler chunkHandler;

	void Start(){
		chunkHandler = GameObject.Find("Terrain").GetComponent(typeof(ChunkHandler)) as ChunkHandler;
	}

    void OnCollisionEnter(Collision collider){
    	if (collider.gameObject.CompareTag("Player")){
    		Debug.Log("We hit a player!");
    	}
    	if (collider.gameObject.CompareTag("Chunk")){
    		Debug.Log("We hit a chunk!");
    		chunkHandler.BlockEdit(transform.position + transform.TransformDirection(Vector3.forward), 2,true);
    	}
    	Destroy(gameObject);
    }
}
