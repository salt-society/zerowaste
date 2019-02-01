using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoundary : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Edge!");
        if (collision.gameObject.tag == "Egde")
        {
           
        }
    }
}
