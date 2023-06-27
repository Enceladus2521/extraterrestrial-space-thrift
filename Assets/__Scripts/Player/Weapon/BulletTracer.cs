using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BulletTracer : MonoBehaviour
{
    private Vector3 _startPos;
    private Vector3 _endPos;   


    private float _speed = 80f;


    public void SetStartAndEndPos(Vector3 startPos, Vector3 endPos, Material TracerColor )
    {
        _startPos = startPos;
        _endPos = endPos;
        GetComponent<TrailRenderer>().material = TracerColor;
              
    }

    private void Update()
    {
        //move tracer
        transform.position = Vector3.MoveTowards(transform.position, _endPos, _speed * Time.deltaTime);

        //destroy tracer
        if (Vector3.Distance(transform.position, _endPos) < 0.1f)
        {
            Destroy(gameObject);
        }
        
    }

}
