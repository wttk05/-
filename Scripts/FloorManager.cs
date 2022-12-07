using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class FloorManager : MonoBehaviour
{
    [SerializeField] GameObject floor;
    float floorPos_x;
    int floorCount;

    public static FloorManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        floorPos_x = 0;
        floorCount = 1;

        for (int i = 0; i<6;i++)
        {
            CreateFloor();
        }
    }

    void Update()
    {
        
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            //CreateFloor();
        }
    }

    public void CreateFloor()
    {
        //ƒ‰ƒ“ƒ_ƒ€‚Ì”Žš
        int Rand = Random.Range(0, 3);

        switch (Rand)
        {
            case 0:
                floorPos_x += 0;// senter                  
                break;
            case 1:
                floorPos_x += 1.5f;// Right
                break;
            case 2:
                floorPos_x -= 1.5f;// Left
                break;
        }

        // ƒtƒƒA‚ð¶¬‚·‚é
        if (floorCount <= GameData.Entity.maxScore)
        {
            var x = Instantiate(floor, new Vector3(floorPos_x, 0, floorCount * 1.5f), Quaternion.identity);
            x.name = "Floor" + floorCount;

            floorCount++;
        }
    }
}
