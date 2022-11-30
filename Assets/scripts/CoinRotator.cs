using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotateVector;
    private void Update() {
        transform.Rotate(rotateVector * Time.deltaTime,Space.World);
    }

}
