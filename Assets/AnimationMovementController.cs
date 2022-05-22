using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationMovementController : MonoBehaviour
{
	PlayerInput playerInput;
    CharacterController characterController;
    Vector2 currentMovInp;
    Vector3 currentMov;
    Animator animator;
    bool isMove;
    float rotperframe = 1.0f;
	
	
	/*
     void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

     void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }*/
	
	void Awake()
	{
		playerInput = new PlayerInput();
		characterController = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		/*
		playerInput.CharacterControls.Move.started += MovementInput;
		playerInput.CharacterControls.Move.canceled += MovementInput;
		playerInput.CharacterControls.Move.performed += MovementInput;*/
	}
	
    void Update()
    {
       handleRotation();
        conAnimation();
        characterController.Move(currentMov * Time.deltaTime);
    }

    void MovementInput(InputAction.CallbackContext context)
    {
        currentMovInp = context.ReadValue<Vector2>();
        currentMov.x = currentMovInp.x;
        currentMov.z = currentMovInp.y;

        isMove = currentMovInp.x != 0 || currentMovInp.y != 0; 
        Debug.Log(context.ReadValue<Vector2>()); 
    }

    void conAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if(isMove && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if(!isMove && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
    }

    void handleRotation()
    {
        Vector3 posLook;
        posLook.x = currentMov.x;
        posLook.y = 0.0f;
        posLook.z = currentMov.z;

        Quaternion curRotation= transform.rotation;
        if (isMove)
        {
            Quaternion targetRotation = Quaternion.LookRotation(posLook);
            transform.rotation=Quaternion.Slerp(curRotation, targetRotation, rotperframe*Time.deltaTime);
        }
    }
}
