using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuke : MonoBehaviour
{
    ChunkHandler chunkHandler;
    public GameObject NukeAnimation;

	void Start(){
		chunkHandler = GameObject.Find("Terrain").GetComponent(typeof(ChunkHandler)) as ChunkHandler;
	}

    void OnCollisionEnter(Collision collider){
    	Debug.Log("collision found");
    	chunkHandler.BlockEdit(transform.position + transform.TransformDirection(Vector3.forward), 200,true);
    	Instantiate(NukeAnimation, transform.position - new Vector3(0,40,0), Quaternion.identity);
    	Destroy(gameObject);
    }
}
