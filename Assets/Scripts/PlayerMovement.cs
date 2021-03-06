using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grapple)), RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    
    public Rigidbody rb { get; private set; }
    public Grapple grapple { get; private set; }
    public LayerMask terrainLayer;
    public float extendAmount;
    [HideInInspector]
    public GameManager manager;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

// Some stupid rigidbody based movement by Dani

    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    private Vector3 normalVector = Vector3.up;

    //Input
    float x, y;
    bool jumping, sprinting, extending;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        grapple = this.GetComponent<Grapple>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        manager = FindObjectOfType<GameManager>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }
    private void Update()
    {
        MyInput();
    }
    private void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        extending = Input.GetButton("Fire3");
    }

    private void Movement()
    {
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //If holding space, retract rope. If holding shift, extend rope
        if (jumping) { grapple.Extend(-extendAmount * 1.5f * Time.deltaTime); }
        if (extending) { grapple.Extend(extendAmount * Time.deltaTime); }

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 0.5f;
        }

        if (grapple.attached) {
            multiplier = 0.8f;
        }

        //Apply forces to move player
        rb.AddForce(transform.forward * y * moveSpeed * Time.deltaTime * multiplier);
        rb.AddForce(transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f, ForceMode.Impulse);
            rb.AddForce(normalVector * jumpForce * 0.5f, ForceMode.Impulse);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    //private float desiredX;
    //private void Look()
    //{
    //    float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
    //    float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

    //    //Find current look rotation
    //    Vector3 rot = playerCam.transform.localRotation.eulerAngles;
    //    desiredX = rot.y + mouseX;

    //    //Rotate, and also make sure we dont over- or under-rotate.
    //    xRotation -= mouseY;
    //    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

    //    //Perform the rotations
    //    playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
    //    transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    //}

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Counter movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (terrainLayer != (terrainLayer | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }
    private void StopGrounded()
    {
        grounded = false;
    }
    public void Reset()
    {
        CancelInvoke();
        grapple.Reset();
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.velocity = Vector3.zero;
    }
}
