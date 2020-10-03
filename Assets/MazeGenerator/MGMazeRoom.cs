using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class MGMazeRoom : MonoBehaviour {
	
	internal float width;
    internal float depth;


	public bool north;
	public bool east;
	public bool south;
	public bool west;

    public bool gizmosAllwaysVisible;
    // internal for generator

    internal int genPosX = 0, genPosY = 0, rotated = 0;
    
    internal int attached = 0;

    internal int distance;
    
    internal int exits { get { return (north ? 1 : 0) + (east ? 1 : 0) + (south ? 1 : 0) + (west ? 1 : 0); } }
    internal bool isOpen { get { return attached < exits; } }
    internal int openEnds { get { return exits - attached; } }

    internal bool HasDirection(int dir)
    {
        dir = (dir + rotated) % 4;
        return dir == 0 ? north : (dir == 1 ? east : (dir == 2 ? south : west));
    }

    void InitMaze()
    {
       
    }

    internal bool fitsMask(int[] entrances, out int rotated)
    {
        rotated = 0;
        int startrot = Random.Range(0,4);
        for (int rot = 0; rot < 4; rot++)
        {
            bool fits = true;
            for (int ent = 0; ent < 4; ent++)
            {
                bool hasDir = HasDirection(rot + ent + startrot);
                if((hasDir && entrances[ent] == 2) || (!hasDir && entrances[ent] == 1))
                {
                    fits = false;
                }
            }
            if(fits)
            {
                rotated = (rot + startrot) % 4;
                return true;
            }
        }
        return false;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!gizmosAllwaysVisible) return;
        var mr = this;
        // Color dark = new Color(.2f, 0, 0);
        Color light = new Color(.6f, .7f, .6f);
        Handles.color = light;

        if (mr.HasDirection(0))
        {
            Handles.SphereCap(1, mr.transform.position + new Vector3(0, 0, mr.depth / 2.5f), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position, mr.transform.position + new Vector3(0, 0, mr.depth / 2.5f));
        }

        if (mr.HasDirection(1))
        {
            Handles.SphereCap(2, mr.transform.position + new Vector3(mr.width / 2.5f, 0, 0), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position, mr.transform.position + new Vector3(mr.depth / 2.5f, 0, 0));
        }

        if (mr.HasDirection(2))
        {
            Handles.SphereCap(3, mr.transform.position - new Vector3(0, 0, mr.depth / 2.5f), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position, mr.transform.position - new Vector3(0, 0, mr.depth / 2.5f));
        }

        if (mr.HasDirection(3))
        {
            Handles.SphereCap(4, mr.transform.position - new Vector3(mr.width / 2.5f, 0, 0), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position, mr.transform.position - new Vector3(mr.depth / 2.5f, 0, 0));
        }
    }
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof (MGMazeRoom))]
public class MazeRoomEditor : Editor {

    

	private void OnSceneGUI()
	{
		


        var mr = (MGMazeRoom)target;
       // Color dark = new Color(.2f, 0, 0);
        Color light = new Color(.8f, 1, .8f);
        Handles.color = light;
        
        if (mr.HasDirection(0))
        {
            Handles.SphereCap(1, mr.transform.position + new Vector3(0, 0, mr.depth / 2), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position, mr.transform.position + new Vector3(0, 0, mr.depth / 2));
        }

        if (mr.HasDirection(1))
        {
            Handles.SphereCap(2, mr.transform.position + new Vector3(mr.width / 2, 0, 0), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position, mr.transform.position + new Vector3(mr.depth / 2, 0, 0));
        }

        if (mr.HasDirection(2))
        {
            Handles.SphereCap(3, mr.transform.position - new Vector3(0, 0, mr.depth / 2), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position, mr.transform.position - new Vector3(0, 0, mr.depth / 2));
        }

        if (mr.HasDirection(3))
        {
            Handles.SphereCap(4, mr.transform.position - new Vector3(mr.width / 2, 0, 0), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position, mr.transform.position - new Vector3(mr.depth / 2, 0, 0));
        }

        
        
	}

	private void OnEnable()
	{
        

        GameObject generatorObj = GameObject.Find("MazeGenerator") as GameObject;
        MGMazeGenerator generator = generatorObj.GetComponent<MGMazeGenerator>();

        MGMazeRoom mzr = (MGMazeRoom)target;


        mzr.width = generator.roomWallLength;
        mzr.depth = generator.roomWallLength;
       
	}
	

}
#endif