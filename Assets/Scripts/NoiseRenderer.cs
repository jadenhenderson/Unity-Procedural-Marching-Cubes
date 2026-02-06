using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseRenderer : MonoBehaviour
{
	public Renderer textureRender;
	public int width;
	public int octaves;
	public float lacuranity;
	public float persistence;
	public float scale;
	public Vector2 offset;

	public float waitTime;
	private float timeElapsed = 0;
    void Start()
    {
        Debug.Log(Mathf.InverseLerp(1,100,103));
        transform.position = new Vector3(width/2,0,width/2);
    }

    void Update(){
    	if (timeElapsed > waitTime){
    		if (scale <= 0) scale = 0.01f;
    		DrawNoiseMap(Noise.GenerateNoiseMap(width,width,octaves,persistence,lacuranity,new Vector2(47.82f,0f), offset, 1f,scale));
    		timeElapsed = 0f;
    	}
    	timeElapsed += Time.deltaTime;
    }

    void DrawNoiseMap(float[,] noiseMap){
    	int width = noiseMap.GetLength(0);
    	int height = noiseMap.GetLength(1);

    	Texture2D texture = new Texture2D(width, height);

    	Color[] colors = new Color[width*height];

    	for (int x=0; x<width; x++){
    		for (int z=0; z<width; z++){
    			Color baseColor = Color.Lerp(Color.black, Color.white, noiseMap[x,z]);
    			colors[x*width+z] = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f);
    		}
    	}
    	texture.SetPixels(colors);
    	texture.Apply();

    	textureRender.sharedMaterial.mainTexture = texture;
    	textureRender.transform.localScale = new Vector3(width/10, 1, height/10);
    }
}
