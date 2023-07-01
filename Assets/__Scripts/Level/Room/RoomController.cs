using System.Collections.Generic;
using UnityEngine;


public class RoomController : MonoBehaviour
{


    [SerializeField]
    public RoomConfig roomConfig = new RoomConfig();
    private RoomGenerator generator;

    public void UpdateConfig(RoomConfig roomConfig)
    {
        this.roomConfig = roomConfig;
        generator.UpdateConfig(roomConfig);
        generator.GenerateRoom();
    }

    public void Awake(){
        if(gameObject.GetComponent<RoomGenerator>() == null){
            generator = gameObject.AddComponent<RoomGenerator>();
        }else{
            generator =gameObject.GetComponent<RoomGenerator>();
        }
    }

}
