using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class PlayerPhysics : MonoBehaviour
{
    public LayerMask collisionMask;
    [HideInInspector]public bool grounded;
    [HideInInspector]public bool movementStopped;
    private BoxCollider collider ;
    private Vector3 s;
    private Vector3 c;

    private float skin = 0.005f;
    Ray ray;
    RaycastHit hit;

    private void Start() {
        collider = GetComponent<BoxCollider>();
        s = collider.size;
        c = collider.center;

    }   
    public void MoveAmount(Vector2 moveAmount){
        float deltaY = moveAmount.y;
        float deltaX = moveAmount.x;

        
        Vector2 p =transform.position;

        grounded = false;
        for(int i=0;i<3;i++){
            float dir = Mathf.Sign(deltaY);
            float x = (p.x + c.x - s.x/2)  + s.x/2* i;  //left, center and then rightmost point of collider
            float y = p.y +c.y + s.y/2*dir; //Bottom of collider and also upper side since multiplied by dir...

            ray = new Ray(new Vector2(x,y),new Vector2(0,dir));
            Debug.DrawRay(ray.origin,ray.direction);
            if(Physics.Raycast(ray ,out hit, Mathf.Abs(deltaY) + skin , collisionMask)){
                float dst = Vector3.Distance(ray.origin,hit.point);

                //Stop player downward movment after coing within the skin width of the collider
                if(dst>skin){
                    deltaY = dir*dst - skin* dir;
                }else {
                    deltaY = 0;
                }
                grounded = true;
                break;
            }
        }

        //Check collision from left and right
        movementStopped = false;
        for(int i=0;i<3;i++){
            float dir = Mathf.Sign(deltaX);
            float x = p.x + c.x +s.x/2*dir;
            float y = p.y + c.y -s.y/2 +s.y/2*i; 

            ray = new Ray(new Vector2(x,y),new Vector2(dir,0));
            Debug.DrawRay(ray.origin,ray.direction);
            if(Physics.Raycast(ray ,out hit, Mathf.Abs(deltaX) + skin , collisionMask)){
                float dst = Vector3.Distance(ray.origin,hit.point);

                //Stop player downward movment after coing within the skin width of the collider
                if(dst>skin){
                    deltaX = dir*dst - skin* dir;
                }else {
                    deltaX = 0;
                }
                movementStopped = true;
                break;
            }
        }

        if(!grounded && !movementStopped){      // to check the digonal collision 
            Vector3 playerDir = new Vector3(deltaX,deltaY);
            Vector3 o = new Vector3(p.x + c.x +s.x/2*Mathf.Sign(deltaX),p.y +c.y + s.y/2*Mathf.Sign(deltaY));//x value from horizonal collision check
                                                                                //y value from vertical collision check..
            ray = new Ray(o,playerDir.normalized);

            if(Physics.Raycast(ray,Mathf.Sqrt(deltaX*deltaX + deltaY*deltaY),collisionMask)){
                grounded = true;
                deltaY = 0;

                
            }
        }

        Vector2 newTransform = new Vector2(deltaX,deltaY);

        transform.Translate(newTransform);
    }
}
