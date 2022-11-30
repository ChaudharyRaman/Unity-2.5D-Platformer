using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowBoy : MonoBehaviour
{
    public float walkSpeed = 2.5f;
    public float jumpHeight = 4f;
    public float gravity = -10f;
    
    public Transform groundCheckTransform;
    public float groundCheckRadius = 0.2f;

    public Transform targetTransform;
    public LayerMask mouseAimMask;
    public LayerMask groundMask;
    public GameObject bulletPrefab;
    public Transform muzzletransform;
    
    
    //Recoil INput
    public AnimationCurve recoilCurve;
    public float recoilDuration = 0.25f;
    public float recoilMaxRotation = 45f;
    public Transform rightLowerArm;
    public Transform rightHand;
    private float recoilTimer;


    private Animator animator;
    private Rigidbody rbody;
    private bool isGrounded;
    private float inputMovement;
    Camera mainCamera;
   
    
    private int FacingSign{
        get {
            Vector3 perp = Vector3.Cross(transform.forward,Vector3.forward);
            float dir = Vector3.Dot(perp,transform.up);
            return dir> 0f ? -1 : (dir)<0f   ? 1:0;
        }
    }
    private void Start() {
        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

    }

    private void Update() {
        inputMovement = Input.GetAxis("Horizontal");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit ;
        if(Physics.Raycast(ray,out hit,Mathf.Infinity,mouseAimMask)){
            targetTransform.position = hit.point;
        }
        
        if(Input.GetButtonDown("Jump") && isGrounded){
            rbody.velocity = new Vector3(rbody.velocity.x,0,0);
            //rbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight *gravity) , ForceMode.VelocityChange);
            rbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * gravity),ForceMode.VelocityChange );
        }

        if(Input.GetMouseButtonDown(0)){
            Fire();
        }
    }
    void Fire(){
        recoilTimer = Time.time;

        var go = Instantiate(bulletPrefab);
        go.transform.position = muzzletransform.position;
        var bullet = go.GetComponent<Bullet>();
        bullet.Fire(go.transform.position,muzzletransform.eulerAngles,gameObject.layer);
    }
    private void FixedUpdate() {
        //Movement
        rbody.velocity = new Vector3(inputMovement * walkSpeed ,rbody.velocity.y,0);
        animator.SetFloat("speed",(FacingSign * rbody.velocity.x)/walkSpeed);
        //FACING ROTATION
        rbody.MoveRotation(Quaternion.Euler(new Vector3(0,90 * Mathf.Sign(targetTransform.position.x - transform.position.x),0)));

        //GroundCheck
        isGrounded = Physics.CheckSphere(groundCheckTransform.position,groundCheckRadius,groundMask,QueryTriggerInteraction.Ignore);
        animator.SetBool("isGrounded",isGrounded);
    }

    private void LateUpdate() {
        //Recoil Animation
        if(recoilTimer<0){
            return;
        }
        float curveTime = (Time.time - recoilTimer)/recoilDuration;
        if(curveTime>1){
            recoilTimer = -1;
        } else{
            rightLowerArm.Rotate(Vector3.forward, recoilCurve.Evaluate(curveTime) * recoilMaxRotation,Space.Self);
        }
    }

    

    private void OnAnimatorIK(int layerIndex) {
        //Weapon aim at target IK
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
        animator.SetIKPosition(AvatarIKGoal.RightHand,targetTransform.position);

        //Look at target IK
        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(targetTransform.position);
    }

}

