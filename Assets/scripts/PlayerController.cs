using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : MonoBehaviour
{
    // Player Handling
    public float gravity = 20;
    public float speed = 8f;
    public float jumpHeight = 12f;
    public float acceleration = 12f;

    private float currentSpeed;
    private float targetSpeed;
    private Vector2 amountToMove;

    private PlayerPhysics playerPhysics;

    private void Start() {
        playerPhysics = GetComponent<PlayerPhysics>();
    }
    private void Update() {

        if(playerPhysics.movementStopped){
            targetSpeed = 0;            //make sure these come before
            currentSpeed = 0;
        }
        //Input
        targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
        currentSpeed = IncrementTowards(currentSpeed,targetSpeed,acceleration);
        

        if(playerPhysics.grounded){
            amountToMove.y = 0;
            //Jump
            if(Input.GetButtonDown("Jump")){
                amountToMove.y = jumpHeight;
            }
        }
        amountToMove.x = currentSpeed;  
        amountToMove.y -= gravity * Time.deltaTime;
        playerPhysics.MoveAmount(amountToMove * Time.deltaTime);
    }
    private float IncrementTowards(float n ,float target,float a){
            if(n==target){
                return n;
            }
            else{
                float dir = Mathf.Sign(target-n);   // must n be incresed or decresed to get closer to target..
                n+=a*Time.deltaTime * dir;
                return (dir == Mathf.Sign(target-n))? n: target;    //if n now passses target then return target ,otherwise return n
            }
    }
}
