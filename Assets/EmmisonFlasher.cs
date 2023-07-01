using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmisonFlasher : MonoBehaviour
{
    [SerializeField] private float _flashSpeed = 0.2f;

    [SerializeField] Material _EmmisionMaterial, _NormalMaterial;





    private void Start()
    {
        StartCoroutine(FlashEmmision());
    }


    IEnumerator FlashEmmision()
    {
        while (true)
        {            
            GetComponent<Renderer>().material = _EmmisionMaterial;            
            yield return new WaitForSeconds(_flashSpeed);                    
            GetComponent<Renderer>().material = _NormalMaterial;
            yield return new WaitForSeconds(_flashSpeed);  
        }
    }
}
