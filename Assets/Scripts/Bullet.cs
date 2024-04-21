using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

// This script will lerp a bullet along a vector until it reaches its target.
// purely for game aesthetic, no behavior is being calculated here.
public class Bullet : MonoBehaviour {
    [SerializeField]
    private float SPEED = 20.0f; // measured in Unity Units per second

    public Vector3 startPoint;
    public Vector3 endPoint;
    public float TTL; // "time to live", total time the bullet animation takes

    private float timeAlive; // current amount of time the bullet has taken thus far

    // given a start and end point, the bullet is set so that it will
    // go from start to end using SPEED constant
    public void configureBulletAnimation(Vector3 start, Vector3 end) {
        this.TTL = (end - start).magnitude / SPEED;
        this.startPoint = start;
        this.endPoint = end;

        transform.position = startPoint;
    }

    // called before the first frame update
    private void Start() {
        timeAlive = 0;
        StartCoroutine(Fire());
    }

    // coroutine that actually does the lerping based on startPoint, endPoint,
    // timeAlive, and TTL.
    IEnumerator Fire() {
        while(timeAlive < TTL) {
            yield return null;
            transform.position = Vector3.Lerp(startPoint, endPoint, timeAlive / TTL);
            timeAlive += Time.deltaTime;
        }

        // delete this object once the animation is completed
        Destroy(gameObject);
    }
}
