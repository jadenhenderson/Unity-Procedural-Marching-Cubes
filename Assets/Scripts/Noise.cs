using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
	private static float[,] _noiseMap;
	//AnimationCurve heightCurve;

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int octaves, float persistence, float lacunarity, Vector2 offset, Vector2 noiseOffset, float otherScale = 1f, float scale = 123.4f, float lerpMin = 0, float lerpMax = 0){
    	float[,] noiseMap = new float[mapWidth,mapHeight];
    	//heightCurve = ChunkHandler.heightCurve;
    	for (int x = 0; x < mapWidth; x++){
    		for (int z=0; z<mapHeight; z++){
    			float amplitude = 1;
    			float frequency = 1;
    			float output = 0;
    			for (int i=0; i<octaves; i++){
    				output += Mathf.PerlinNoise((x*otherScale+offset.x)/scale*frequency + noiseOffset.x,(z*otherScale+offset.y)/scale*frequency + noiseOffset.y) * amplitude;
    				amplitude *= persistence;
    				frequency *= lacunarity;
    			}
    			noiseMap[x,z] = output;
    		}
    	}
    	// normalize output
    	if (lerpMax == 0){
    		lerpMax = 1.1f;
    		//for (int i=1; i<octaves; i++){
    		//	lerpMax += Mathf.Pow(persistence,i);
    		//}
    	}

    	for (int i=0; i<mapWidth; i++){
    		for (int k=0; k<mapWidth; k++){
    			noiseMap[i,k] = Mathf.InverseLerp(lerpMin, lerpMax, noiseMap[i,k]);
    		}
    	}
    	_noiseMap = noiseMap;
    	return noiseMap;
    	
    }

    public static float NoiseValue(int x,int z){
    	return _noiseMap[x,z];
    }
}
