using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    // Before game starts offically
    private bool isRunning = false;

    // Animation
    private Animator anim;

    // Movement
	private CharacterController controller;
    private float jumpForce = 4.0f;
	private float verticalVelocity;
	private float gravity = 12.0f; // gravity value

    // Speed Modifier
    private float originalSpeed = 10.0f; // Initial speed of player
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f; // every 2.5s
    private float speedIncreaseAmount = 0.1f; // increase speed by 0.1

    // 7 Lanes in total: 0 = FarFarLeft, 1 = FarLeft, 2 = Left, 3 = Middle, 4 = Right, 5 = FarRight, 6 = FarFarRight
    private int desiredLane = 3; // Set lane to middle where player will begin at
	private const float LANE_DISTANCE = 1.5f; // distance between each lane
	private const float TURN_SPEED = 0.05f; // when running while turning to side

	// Use this for initialization
	private void Start ()
	{
        speed = originalSpeed;
		anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	private void Update ()
	{
        if(!isRunning) // If game have not started or if player collides or fall; they stop moving
        {
            return;
        }

        if(Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }
		
		// --- For Adding Lane ---
		// Gather inputs on which lane player should be
        if(MobileInput.Instance.SwipeLeft) //if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
			MoveLane(false);
		}
        if(MobileInput.Instance.SwipeRight) //if(Input.GetKeyDown(KeyCode.RightArrow))
        {
			MoveLane(true);
		}

		// For 7 Lanes: Calculator where player should be next
		Vector3 targetPosition = transform.position.z * Vector3.forward;
		switch(desiredLane)
		{
		case 2: // move left
			targetPosition += Vector3.left * LANE_DISTANCE;
			break;
		case 1: // move far left
			targetPosition += (Vector3.left * LANE_DISTANCE) * 2;
			break;
		case 0: // move far far left
			targetPosition += (Vector3.left * LANE_DISTANCE) * 3;
			break;
		case 4: // move right
			targetPosition += Vector3.right * LANE_DISTANCE;
			break;
		case 5: // move far right
			targetPosition += (Vector3.right * LANE_DISTANCE) * 2;
			break;
		case 6: // move far far right
			targetPosition += (Vector3.right * LANE_DISTANCE) * 3;
			break;
		default: // otherwise desiredLane = 3
			// do nothing since it is in the middle
			break;
		}

		// Calculate player's move delta
		Vector3 moveVector = Vector3.zero;
		moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

		// Calculate Y
        if(isGrounded) // player on ground
		{
			verticalVelocity = -0.1f;

            if(MobileInput.Instance.SwipeUp) //if(Input.GetKeyDown(KeyCode.Space))
            {
                // Jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
                // Shrink character controller's height and shift center upwards when jumping
                controller.height /= 1.5f;
                controller.center = new Vector3(controller.center.x, controller.center.y * 2.0f, controller.center.z);
                Invoke("FinishJump", 0.65f); // To undo changes after finish jump
            }
            else if(MobileInput.Instance.SwipeDown)
            {
                // Roll
                StartRolling();
                Invoke("StopRolling", 1.0f);
            }
		}
		else // player not on ground
		{
			// Currently, the player is slightly off the ground so we need this statement
            verticalVelocity -= (gravity * Time.deltaTime); // make player grounded


            /*// Not working properly as player sink halfway through ground
            // Fast Falling mechanic
            if(MobileInput.Instance.SwipeDown) //if(Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = -jumpForce;
            }*/
		}

		moveVector.y = verticalVelocity; // make player grounded

		// if player's position fall below a certain height, display Death message
		if(moveVector.y < -20.0f)
		{
			FallToDeath();
		}

		moveVector.z = speed; // Constantly moving forward

		// Move the player
		controller.Move(moveVector * Time.deltaTime);

		// To Rotate the player's direction to where he is going
		Vector3 dir = controller.velocity; // Initialize dir variable to current velocity
		if(dir != Vector3.zero)
		{
			dir.y = 0; // kill the y axis, so that when jumping will snap down instead of gliding down
			transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED); // Turn the player's direction
		}

        // Not working properly; death animation also speed up...which is not what we want
        //anim.speed = speed;

        // To increase speed of running animation of player as the game speeds up
        // Not working properly...
        //anim.SetFloat("runMultiplier", 1.0f);
	}

	// Function for switching between lanes
	private void MoveLane(bool goingRight)
	{
		// For 7 lanes:
		if(!goingRight) // left
        {
			desiredLane--;
			if(desiredLane == -1)
			{
				desiredLane = 0;
			}
		}
        else // right
        {
			desiredLane++;
			if(desiredLane == 7)
			{
				desiredLane = 6;
			}
		}
	}

    // Use own grounded function
    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x, (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f, controller.bounds.center.z), Vector3.down);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    // Function for when game offically starts running
    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }

    private void StartRolling()
    {
        anim.SetBool("Rolling", true);
        // Shrink character controller size when rolling
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }

    private void StopRolling()
    {
        anim.SetBool("Rolling", false);
        // Unshrink character controller size after rolling
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    private void FinishJump()
    {
        // Unshrink and Shift character controller back to original after finish jump
        controller.height *= 1.5f;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2.0f, controller.center.z);
    }

	// Function is called everytime player collides with an object
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
        switch(hit.gameObject.tag)
        {
            case "Obstacle": // Colliding with inanimated objects
                Crashed();
                break;

            case "Enemy": // Colliding with animated objects/enemies
                Killed();
                break;
        }
	}

    // Creating separate functions for different deaths
    // so that, later we can show different "You fell/crashed/killed" messages or scenes

    // Function to call when player falls
    private void FallToDeath()
	{
		Debug.Log("You fell into the Abyss!"); // For testing when player falls
        anim.SetTrigger("byFallDeath");
		Death();
	}

	// Function to call when player collides into inanimated objects
	private void Crashed()
	{
		Debug.Log("You crashed!"); // For testing when player crashed
        anim.SetTrigger("byObstacleDeath");
        Death();
	}

	// Function to call when player get killed by animated objects
	private void Killed()
	{
		Debug.Log("You were killed!"); // For testing when player get killed
        anim.SetTrigger("byEnemyDeath");
        Death();
	}

	// General Function to call to lose when player dies
	private void Death()
	{
		Debug.Log("You have died!"); // For testing when player collides with obstacle
        isRunning = false;
        GameManager.Instance.IsDead = true;
	}
}
