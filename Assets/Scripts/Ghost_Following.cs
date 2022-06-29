using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost_Following : MonoBehaviour
{
    public Transform humanTrans;

    public float radius = 1f;
    public float frequency = 10;
    public float smoothness = 30f;
    public float angularAcceleration = 100f;
    private float radiusNoise;

    private void FixedUpdate()
    {

        radiusNoise = radius + frequency * Mathf.PerlinNoise((transform.position.x) / smoothness, 0) - (frequency / 2);

        transform.RotateAround(humanTrans.position, Vector3.forward, angularAcceleration * Time.deltaTime);
        var desiredPosition = (transform.position - humanTrans.position).normalized * radiusNoise + humanTrans.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.fixedDeltaTime * angularAcceleration);
        transform.rotation = new Quaternion(0, 0, 0, 0);

    }

}
