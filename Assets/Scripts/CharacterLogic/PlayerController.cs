﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Collider2D col;

    private bool leftButton;
    private bool rightButton;
    private bool jumpButton;
    private bool intertactButton;
    private int movementDirection;
    private bool collidingWithWallLeft;
    private bool collidingWithWallRight;
    private bool grounded;
    private AHoldableObject heldObject;

    public bool keyboardControlsOn;

    [SerializeField]
    private float groundedVDelta = 0.01f;

    [SerializeField]
    private float playerSpeed = 0.1f;

    [SerializeField]
    private float jumpForce = 350f;

    private bool isInSphere;

    public HoldableObject HeldObject { get; } = HoldableObject.none;

    public int MovementDirection { get; }


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        GameObject.Find("TestingObjectHoldable").GetComponent<TestHoldableObject>().PickUp(this);
        heldObject = GameObject.Find("TestingObjectHoldable").GetComponent<TestHoldableObject>();
    }

    void Update()
    {

    }

    public void ButtonInput(string input)
    {
        switch (input)
        {
            case "right":
                rightButton = true;
                break;
            case "left":
                leftButton = true;
                break;
            case "right-up":
                rightButton = false;
                break;
            case "left-up":
                leftButton = false;
                break;
            case "jump":
                jumpButton = true;
                break;
            case "interact":
                intertactButton = true;
                break;
        }
    }

    private void FixedUpdate()
    {
        GetKeyboardInput();
        Movement();
    }

    private void GetKeyboardInput()
    {
        if (keyboardControlsOn)
        {
            if (Input.GetAxis("Horizontal") > 0)
                ButtonInput("right");

            if (Input.GetAxis("Horizontal") < 0)
                ButtonInput("left");

            if (Input.GetKey(KeyCode.Space))
                ButtonInput("jump");

            if (Input.GetAxis("Horizontal") == 0)
            {
                rightButton = false;
                leftButton = false;
            }
            if (Input.GetKey(KeyCode.F))
            {
                intertactButton = true;
            }
        }
    }
    private void Movement()
    {
        movementDirection = Convert.ToInt32(rightButton) - Convert.ToInt32(leftButton);
        transform.Translate(new Vector2(playerSpeed * movementDirection * Time.fixedDeltaTime, 0));

        if (jumpButton)
        {
            if (grounded)
            {
                grounded = false;
                rigidBody.AddForce(transform.up * jumpForce);
            }
            jumpButton = false;
        }
    }

    private void InteractActions()
    {
        if (intertactButton)
        {
            if (heldObject)
            {
                heldObject.Use(this);
                heldObject = null;
            }
        }
    }

    //Track if the player capsule is currently inside the transparent sphere or not
    void OnTriggerEnter(Collider trigger)
    {
        if (trigger.tag == "PlatformSphere")
        {
            isInSphere = true;
        }
        if (trigger.tag == "HoldableObject")
        {
            trigger.gameObject.GetComponent<AHoldableObject>().PickUp(this);
            heldObject = trigger.GetComponent<AHoldableObject>();
        }
    }

    void OnTriggerExit(Collider trigger)
    {
        if (trigger.tag == "PlatformSphere")
        {
            isInSphere = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            grounded = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (Math.Abs(rigidBody.velocity.y) <= groundedVDelta)
            {
                grounded = true;
            }
        }
    }
}
