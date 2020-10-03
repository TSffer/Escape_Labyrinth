using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class MGMazeGenerator : MonoBehaviour {

    public MGMazeRoom startRoom;
    public MGMazeRoom finishRoom;
    public int roomsCountToGenerate;
    public int minDistanceToFinish;
    public int doorsCount;
    

    public int roomWallLength;
    public List<MGMazeRoom> rooms;
    public List<MGDoor> doors;

    public bool newSeedOnGenerate;
    public int randomSeed;

    public bool generateOnRuntime = true;
	
    // Private

    private List<MGMazeRoom> map = new List<MGMazeRoom>();
    private List<MGDoor> doormap = new List<MGDoor>();
    private bool finishPut = false;
    
	void Start () {
        if (generateOnRuntime)
            Generate();
    }

    public void Generate() {

        if (newSeedOnGenerate) randomSeed = (int) (System.DateTime.UtcNow.ToBinary() % int.MaxValue);

        Random.seed = randomSeed;

        finishPut = false;
        map = new List<MGMazeRoom>();
        doormap = new List<MGDoor>();

        GameObject room = Instantiate(startRoom.gameObject, new Vector3(0,0,0), Quaternion.identity) as GameObject;
        var mazeRoom = room.GetComponent<MGMazeRoom>();
        if (mazeRoom == null) throw new UnityException("GameObject " + room.name + " has no MGMazeRoom component.");
        map.Add(mazeRoom);
        room.name = "Start-0-0";
        
        bool[] hasXEndsRoom = new bool[4]{false,false,false,false};
        
        for (int i = 0; i < rooms.Count; i++) { 
            var r = rooms[i];
            
                if (r.exits > 0)
                {  
                    hasXEndsRoom[r.exits - 1] = true;
                }
            }

        

        for (int i = 0; i < 4; i++)
        {  
            if (!hasXEndsRoom[i]) {
                throw new UnityException("Ganerator's room list has no room with " + (i + 1) + " entrances.");
            }
        }
        
        for(int i = 0; i < roomsCountToGenerate; i++)
        {
            addNewRoom();
        }

        for (int i = 0; i < minDistanceToFinish * 5 && !finishPut; i++)
            addNewRoom();

            finalizeMaze();
	}

    private void finalizeMaze()
    {
        for (int index = 0; index < map.Count; index++)
            if (map[index].isOpen)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    if (map[index].HasDirection(dir))
                    {
                        

                        int posX = map[index].genPosX, posY = map[index].genPosY;
                        switch (dir)
                        {
                            case 0: posY++;
                                break;
                            case 1: posX++;
                                break;
                            case 2: posY--;
                                break;
                            case 3: posX--;
                                break;
                        }

                        if (emptySlotInMap(posX, posY))
                        {
                            putRoomAt(posX, posY, true);
                        }
                        else
                        {
                            var nr = getRoomAt(posX, posY);
                            if (!nr.HasDirection(dir ^ 2))
                            {
                                removeRoomAt(posX, posY);
                                putRoomAt(posX, posY, true);
                            }
                        }
                    }
                }

            }

        List<int> triedRooms = new List<int>();
        triedRooms.Add(0);
        for (var i = 1; i < map.Count; i ++) triedRooms.Insert(Random.Range(0, triedRooms.Count+1), i);
            while (doormap.Count < doorsCount && triedRooms.Count > 0)
            {
                int roomId = triedRooms[0]; triedRooms.RemoveAt(0);
                int dir = Random.Range(0, 4);
                while (!map[roomId].HasDirection(dir)) dir = (dir+1) % 4;
                putDoor(map[roomId], dir);
            }

    }

    private bool putDoor(MGMazeRoom room, int dir)
    {
        int dPosX = room.genPosX * 2, dPosY = room.genPosY * 2;
        switch (dir)
        {
            case 0: dPosY++;
                break;
            case 1: dPosX++;
                break;
            case 2: dPosY--;
                break;
            case 3: dPosX--;
                break;
        }

        foreach (MGDoor door in doormap)
            if (door.posX == dPosX && door.posY == dPosY) return false;

        GameObject obj = GameObject.Instantiate(doors[Random.Range(0, doors.Count)].gameObject, new Vector3(dPosX * roomWallLength / 2f, 0, dPosY * roomWallLength / 2f), Quaternion.identity) as GameObject;
        MGDoor dObj = obj.GetComponent<MGDoor>();
        dObj.posY = dPosY;
        dObj.posX = dPosX;
        doormap.Add(dObj);
        obj.name = "Door @ " + dPosX + ":" + dPosY;
        if ((dir % 2 == 0) ^ dObj.north_SouthOrentation)
        {
            dObj.rotated = true;
            obj.transform.Rotate(Vector3.up * -90f);
        }

        return true;
    }

    private void addNewRoom()
    {       
        for (int index = 0; index < map.Count; index++)
            if (map[index].isOpen)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    if(map[index].HasDirection(dir))
                    {

                        int posX = map[index].genPosX, posY = map[index].genPosY;
                        switch (dir)
                        {
                            case 0: posY++;
                                break;
                            case 1: posX++;
                                break;
                            case 2: posY--;
                                break;
                            case 3: posX--;
                                break;
                        }

                        if(emptySlotInMap(posX, posY))
                        {
                            if (putRoomAt(posX, posY)) return;
                        }
                    }
                }
            
            }
    }

    private int entranceAt(MGMazeRoom r, int dir)
    {
        if (r == null) return 0; 
        return r.HasDirection(dir) ? 1 : 2;
    }

    private void removeRoomAt(int posX, int posY)
    {
        int l = map.Count;
        for (int i = 0; i < l; i++ )
        {
            if(map[i].genPosX == posX && map[i].genPosY == posY)
            {
                GameObject.Destroy(map[i].gameObject);
                MGMazeRoom[] neighbors = new MGMazeRoom[4] { getRoomAt(posX, posY + 1), getRoomAt(posX + 1, posY), getRoomAt(posX, posY - 1), getRoomAt(posX - 1, posY) };
                foreach (MGMazeRoom neib in neighbors)
                {
                    if (neib != null)
                    {
                        neib.attached--;
                    }
                }
                map.RemoveAt(i); i--;
            }
        }
    }

    private bool putRoomAt(int posX, int posY, bool deadEnd = false)
    {

        MGMazeRoom[] neighbors = new MGMazeRoom[4] { getRoomAt(posX, posY + 1), getRoomAt(posX + 1, posY), getRoomAt(posX, posY - 1), getRoomAt(posX - 1, posY) };
        int[] entrances = new int[4]{ entranceAt(neighbors[0], 2),
            entranceAt(neighbors[1], 3),
            entranceAt(neighbors[2], 0),
            entranceAt(neighbors[3], 1)
        };

        if (deadEnd)
        {
            for (int i = 0; i < 4; i++) if (entrances[i] == 0) entrances[i] = 2;
        }
        int openEnds = 0;
        for(int i = 0; i < map.Count; i++)
        {
            openEnds += map[i].openEnds;
        }
        
        
        int minDistance = -1;
        foreach (MGMazeRoom r in neighbors)
        { if (r != null && (r.distance < minDistance || minDistance == -1)) minDistance = r.distance; }

        int roomAdd = Random.Range(0, rooms.Count);
        bool putFinish = (minDistance >= minDistanceToFinish || roomsCountToGenerate <= map.Count) && !finishPut;

        for (int i = 0; i <= rooms.Count; i++ )
        {
            roomAdd %= rooms.Count;
            MGMazeRoom roomToAdd = putFinish ? finishRoom : rooms[roomAdd];
            int closedEnds = 0;
            foreach(int e in entrances)
            {
                if (e == 1) closedEnds++;
            }

            if (map.Count < 3 && roomToAdd.exits <= 2) { roomAdd++; putFinish = false; continue; }
            if (!deadEnd && closedEnds >= openEnds && roomsCountToGenerate > map.Count && roomToAdd.exits <= closedEnds) { roomAdd++; putFinish = false; continue; }

            if (putFinish 
                || (putFinish && ((openEnds < 3 && map.Count < roomsCountToGenerate) || minDistance < minDistanceToFinish || finishPut))
                || (!putFinish && rooms[roomAdd].exits == 1 && !deadEnd)
                )
            { roomAdd++; putFinish = false; continue; }



            // try to fit
            int rotation = 0;
            if (rooms[roomAdd].fitsMask(entrances, out rotation))
            {

                if (putFinish) finishPut = true;
                GameObject go = GameObject.Instantiate(roomToAdd.gameObject, new Vector3(posX * roomWallLength, 0, posY * roomWallLength), Quaternion.identity) as GameObject;
                go.transform.Rotate(Vector3.up * -90f * rotation);
                
                MGMazeRoom mr = go.GetComponent<MGMazeRoom>();
                
                mr.genPosX = posX;
                mr.genPosY = posY;
                mr.rotated = rotation;
                mr.distance = -1;
                for(int ni = 0; ni < 4; ni++)
                {
                    MGMazeRoom r = neighbors[ni];
                    if (r != null && (r.distance < mr.distance || mr.distance == -1)) mr.distance = r.distance + 1;
                    if (r != null && entrances[ni] == 1) { r.attached++; mr.attached++; }
                }

                go.name = (putFinish ? "finish" : "room") + "dist:" + mr.distance + "@" + posX + ":" + posY + " rot:" + rotation + " ends:" + openEnds ;

                map.Add(mr);
                return true;
            }
            putFinish = false;
            roomAdd++;
        }

            return false;
    }

    

    private bool emptySlotInMap(int posX, int posY)
    {
        return getRoomAt(posX, posY) == null;
    }

    private MGMazeRoom getRoomAt(int posX, int posY)
    {
        foreach(MGMazeRoom room in map)
        {
            if (room.genPosX == posX && room.genPosY == posY) return room;
        }
        return null;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(MGMazeGenerator))]
public class MazeGeneratorEditor : Editor
{
    private void OnDisable()
    {
        var mg = (MGMazeGenerator)target;
        foreach(MGMazeRoom r in mg.rooms)
        {   
            if (r != null)
            {
                r.width = mg.roomWallLength;
                r.depth = mg.roomWallLength;
            }
        }

        if (mg.generateOnRuntime)
        {
            var mrooms = GameObject.FindObjectsOfType<MGMazeRoom>();
            foreach (MGMazeRoom o in mrooms)
                GameObject.DestroyImmediate(o.gameObject);
            var mdoors = GameObject.FindObjectsOfType<MGDoor>();
            foreach (MGDoor o in mdoors)
                GameObject.DestroyImmediate(o.gameObject);
        }
    }

    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mg = (MGMazeGenerator)target;
        if (!mg.generateOnRuntime)
        {
            if(EditorGUILayout.ToggleLeft("Generate now", false))
            {
                var mrooms = GameObject.FindObjectsOfType<MGMazeRoom>();
                foreach (MGMazeRoom o in mrooms)
                    GameObject.DestroyImmediate(o.gameObject);
                var mdoors = GameObject.FindObjectsOfType<MGDoor>();
                foreach (MGDoor o in mdoors)
                    GameObject.DestroyImmediate(o.gameObject);

                mg.Generate();
            }
        }
        if (GUI.changed)
        {
            
            EditorUtility.SetDirty(target);
        }
    }
}
#endif