using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MGDoor : MonoBehaviour {

    internal float width;
    internal float depth;
    internal int posX;
    internal int posY;
    internal bool rotated;

    public bool north_SouthOrentation;
    public bool gizmosAllwaysVisible;
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        var mr = this;
        // Color dark = new Color(.2f, 0, 0);
        Color light = new Color(.8f, 1, .8f);
        Handles.color = light;

        if (north_SouthOrentation ^ rotated)
        {
            Handles.SphereCap(1, mr.transform.position + new Vector3(0, 0, mr.depth / 2.5f), Quaternion.identity, mr.depth / 10);
            Handles.SphereCap(2, mr.transform.position - new Vector3(0, 0, mr.depth / 2.5f), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position - new Vector3(0, 0, mr.depth / 2.5f), mr.transform.position + new Vector3(0, 0, mr.depth / 2.5f));
        }
        else
        {
            Handles.SphereCap(3, mr.transform.position + new Vector3(mr.width / 2.5f, 0, 0), Quaternion.identity, mr.depth / 10);
            Handles.SphereCap(4, mr.transform.position - new Vector3(mr.width / 2.5f, 0, 0), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position - new Vector3(mr.depth / 2.5f, 0, 0), mr.transform.position + new Vector3(mr.depth / 2.5f, 0, 0));
        }
    }

    private void OnDrawGizmos()
    {
        if (!gizmosAllwaysVisible) return;
        var mr = this;
        // Color dark = new Color(.2f, 0, 0);
        Color light = new Color(.6f, .7f, .6f);
        Handles.color = light;

        if (north_SouthOrentation)
        {
            Handles.SphereCap(1, mr.transform.position + new Vector3(0, 0, mr.depth / 2.5f), Quaternion.identity, mr.depth / 10);
            Handles.SphereCap(2, mr.transform.position - new Vector3(0, 0, mr.depth / 2.5f), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position - new Vector3(0, 0, mr.depth / 2.5f), mr.transform.position + new Vector3(0, 0, mr.depth / 2.5f));
        }
        else
        {
            Handles.SphereCap(3, mr.transform.position + new Vector3(mr.width / 2.5f, 0, 0), Quaternion.identity, mr.depth / 10);
            Handles.SphereCap(4, mr.transform.position - new Vector3(mr.width / 2.5f, 0, 0), Quaternion.identity, mr.depth / 10);
            Handles.DrawLine(mr.transform.position - new Vector3(mr.depth / 2.5f, 0, 0), mr.transform.position + new Vector3(mr.depth / 2.5f, 0, 0));
        }
    }
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof(MGDoor))]
public class MazeDoorEditor : Editor
{



    

    private void OnEnable()
    {

        GameObject generatorObj = GameObject.Find("MazeGenerator") as GameObject;
        MGMazeGenerator generator = generatorObj.GetComponent<MGMazeGenerator>();

        MGDoor mzr = (MGDoor)target;
        mzr.width = generator.roomWallLength;
        mzr.depth = generator.roomWallLength;

    }


}
#endif