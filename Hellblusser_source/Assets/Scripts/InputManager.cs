using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // instance
    public static InputManager instance;

    // state
    public enum InputType { Keyboard, Gamepad };
    public InputType lastInputType;

    // movement
    [HideInInspector] public bool jumpPressed, jumpHold, jumpReleased;
    [HideInInspector] public Vector2 moveDirection;
    [HideInInspector] public Vector2 lookDirection;

    // game
    [HideInInspector] public bool quitPressed, quitHold, quitReleased;
    [HideInInspector] public bool restartPressed, restartHold, restartReleased;
    [HideInInspector] public bool interactPressed, interactHold, interactReleased;
    [HideInInspector] public bool cancelPressed, cancelHold, cancelReleased;

    // combat
    [HideInInspector] public bool meleeAttackPressed, meleeAttackHold, meleeAttackReleased;
    [HideInInspector] public bool magicAttackPressed, magicAttackHold, magicAttackReleased;
    [HideInInspector] public bool blockPressed, blockHold, blockReleased;
    [HideInInspector] public bool kickPressed, kickHold, kickReleased;

    // blocking
    [HideInInspector] public bool keyboardBlock, gamepadBlock;
    int keyboardBlockHoldFrameCount;
    int gamepadBlockHoldFrameCount;

    // mouse
    [HideInInspector] public Vector2 mousePosition;
    [HideInInspector] public bool leftMouseHold, leftMousePressed, leftMouseReleased;
    [HideInInspector] public bool rightMouseHold, rightMousePressed, rightMouseReleased;

    // input strings
    string jumpInputKeyboardString, jumpInputGamepadString;
    string moveInputKeyboardString, moveInputGamepadString;
    string lookInputKeyboardString, lookInputGamepadString;
    string quitInputKeyboardString, quitInputGamepadString;
    string interactInputKeyboardString, interactInputGamepadString;
    string cancelInputKeyboardString, cancelInputGamepadString;
    string meleeAttackInputKeyboardString, meleeAttackInputGamepadString;
    string magicAttackInputKeyboardString, magicAttackInputGamepadString;
    string blockInputKeyboardString, blockInputGamepadString;
    string kickInputKeyboardString, kickInputGamepadString;
    [HideInInspector] public string jumpInputStringUse;
    [HideInInspector] public string moveInputStringUse;
    [HideInInspector] public string lookInputStringUse;
    [HideInInspector] public string quitInputStringUse;
    [HideInInspector] public string interactInputStringUse;
    [HideInInspector] public string cancelInputStringUse;
    [HideInInspector] public string meleeAttackInputStringUse;
    [HideInInspector] public string magicAttackInputStringUse;
    [HideInInspector] public string blockInputStringUse;
    [HideInInspector] public string kickInputStringUse;

    // hacks
    [HideInInspector] public bool keyHackPressed, keyHackHold, keyHackReleased;
    [HideInInspector] public bool timeHackPressed, timeHackHold, timeHackReleased;
    [HideInInspector] public bool fullscreenHackPressed, fullscreenHackHold, fullscreenHackReleased;
    [HideInInspector] public bool resolutionUpHackPressed, resolutionUpHackHold, resolutionUpHackReleased;
    [HideInInspector] public bool resolutionDownHackPressed, resolutionDownHackHold, resolutionDownHackReleased;
    [HideInInspector] public bool outlineToggleHackPressed, outlineToggleHackHold, outlineToggleHackReleased;
    [HideInInspector] public bool renderScaleToggleHackPressed, renderScaleToggleHackHold, renderScaleToggleHackReleased;
    [HideInInspector] public bool coinHackPressed, coinHackHold, coinHackReleased;
    [HideInInspector] public bool tearHackPressed, tearHackHold, tearHackReleased;
    [HideInInspector] public bool playerDeadHackPressed, playerDeadHackHold, playerDeadHackReleased;
    [HideInInspector] public bool musicToggleHackPressed, musicToggleHackHold, musicToggleHackReleased;
    [HideInInspector] public bool playerUIHackPressed, playerUIHackHold, playerUIHackReleased;

    void Awake ()
    {
        instance = this;

        // input strings
        CreateInputStrings();
        
        // starting input type?
        if (InputSystem.devices.Count > 0)
        {
            bool hasGamepadConnected = (Gamepad.current != null);
            SetLastInputType((hasGamepadConnected) ? InputType.Gamepad : InputType.Keyboard);
        }
        else
        {
            SetLastInputType(InputType.Keyboard);
        }
    }

    void CreateInputStrings ()
    {
        string inputFront = "[";
        string inputBack = "]";

        jumpInputKeyboardString = inputFront + "SPACEBAR" + inputBack;
        jumpInputGamepadString = inputFront + "A" + inputBack;

        moveInputKeyboardString = inputFront + "WASD" + inputBack;
        moveInputGamepadString = inputFront + "LEFT STICK" + inputBack;

        lookInputKeyboardString = inputFront + "MOUSE" + inputBack;
        lookInputGamepadString = inputFront + "RIGHT STICK" + inputBack;

        quitInputKeyboardString = inputFront + "ESC" + inputBack;
        quitInputGamepadString = inputFront + "B" + inputBack;

        interactInputKeyboardString = inputFront + "SPACEBAR" + inputBack;
        interactInputGamepadString = inputFront + "A" + inputBack;

        cancelInputKeyboardString = inputFront + "ESC" + inputBack;
        cancelInputGamepadString = inputFront + "B" + inputBack;

        meleeAttackInputKeyboardString = inputFront + "LEFT MOUSE BUTTON" + inputBack;
        meleeAttackInputGamepadString = inputFront + "RIGHT TRIGGER" + inputBack;

        magicAttackInputKeyboardString = inputFront + "RIGHT MOUSE BUTTON" + inputBack;
        magicAttackInputGamepadString = inputFront + "LEFT TRIGGER" + inputBack;

        blockInputKeyboardString = inputFront + "LEFT MOUSE BUTTON + S" + inputBack;
        blockInputGamepadString = inputFront + "RIGHT BUMPER" + inputBack;

        kickInputKeyboardString = inputFront + "LEFT SHIFT" + inputBack;
        kickInputGamepadString = inputFront + "LEFT BUMPER" + inputBack;
    }

    void UpdateInputStrings ()
    {
        jumpInputStringUse = (lastInputType == InputType.Keyboard) ? jumpInputKeyboardString : jumpInputGamepadString;
        moveInputStringUse = (lastInputType == InputType.Keyboard) ? moveInputKeyboardString : moveInputGamepadString;
        lookInputStringUse = (lastInputType == InputType.Keyboard) ? lookInputKeyboardString : lookInputGamepadString;
        quitInputStringUse = (lastInputType == InputType.Keyboard) ? quitInputKeyboardString : quitInputGamepadString;
        interactInputStringUse = (lastInputType == InputType.Keyboard) ? interactInputKeyboardString : interactInputGamepadString;
        cancelInputStringUse = (lastInputType == InputType.Keyboard) ? cancelInputKeyboardString : cancelInputGamepadString;
        meleeAttackInputStringUse = (lastInputType == InputType.Keyboard) ? meleeAttackInputKeyboardString : meleeAttackInputGamepadString;
        magicAttackInputStringUse = (lastInputType == InputType.Keyboard) ? magicAttackInputKeyboardString : magicAttackInputGamepadString;
        blockInputStringUse = (lastInputType == InputType.Keyboard) ? blockInputKeyboardString : blockInputGamepadString;
        kickInputStringUse = (lastInputType == InputType.Keyboard) ? kickInputKeyboardString : kickInputGamepadString;
    }

    void Update ()
    {
        // check for input type
        if ( Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame )
        {
            SetLastInputType(InputType.Keyboard);
        }
        if ( InputSystem.devices.Count > 0 && Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame )
        {
            SetLastInputType(InputType.Gamepad);
        }
        if ( lastInputType == InputType.Gamepad && Gamepad.current == null )
        {
            SetLastInputType(InputType.Keyboard);
        }
        UpdateInputStrings();

        // check for blocking (different for keyboard & gamepad)
        float blockThreshold = .25f;
        if ( !keyboardBlock )
        {
            if ( lastInputType == InputType.Keyboard && (moveDirection.y < -blockThreshold && meleeAttackHold) )
            {
                if (keyboardBlockHoldFrameCount < 0)
                {
                    keyboardBlockHoldFrameCount++;
                }
                else
                {
                    keyboardBlock = true;
                }
            }
        }
        else
        {
            if (!meleeAttackHold)
            {
                keyboardBlock = false;
                keyboardBlockHoldFrameCount = 0;
            }
        }

        if ( blockHold )
        {
            if ( gamepadBlockHoldFrameCount < 0 )
            {
                gamepadBlockHoldFrameCount++;
            }
            else
            {
                gamepadBlock = true;
            }
        }
        else
        {
            gamepadBlock = false;
            gamepadBlockHoldFrameCount = 0;
        }

        // log
        //Debug.Log("keyboard block: " + keyboardBlock.ToString() + " || gamepad block: " + gamepadBlock.ToString() + " || " + Time.time.ToString());
    }

    void LateUpdate ()
    {
        ResetAllInput();
    }

    public void ResetAllInput ()
    {
        jumpPressed = false;
        jumpReleased = false;

        meleeAttackPressed = false;
        meleeAttackReleased = false;

        magicAttackPressed = false;
        magicAttackReleased = false;

        quitPressed = false;
        quitReleased = false;

        restartPressed = false;
        restartReleased = false;

        keyHackPressed = false;
        keyHackReleased = false;

        timeHackPressed = false;
        timeHackReleased = false;

        fullscreenHackPressed = false;
        fullscreenHackReleased = false;

        resolutionUpHackPressed = false;
        resolutionUpHackReleased = false;

        resolutionDownHackPressed = false;
        resolutionDownHackReleased = false;

        outlineToggleHackPressed = false;
        outlineToggleHackReleased = false;

        renderScaleToggleHackPressed = false;
        renderScaleToggleHackReleased = false;

        interactPressed = false;
        interactReleased = false;

        coinHackPressed = false;
        coinHackReleased = false;

        tearHackPressed = false;
        tearHackReleased = false;

        playerDeadHackPressed = false;
        playerDeadHackReleased = false;

        cancelPressed = false;
        cancelReleased = false;

        blockPressed = false;
        blockReleased = false;

        kickPressed = false;
        kickReleased = false;

        musicToggleHackHold = false;
        musicToggleHackReleased = false;

        playerUIHackPressed = false;
        playerUIHackReleased = false;
    }

    public void DisableAllInput ()
    {
        jumpHold = false;
        jumpPressed = false;
        jumpReleased = false;

        meleeAttackHold = false; 
        meleeAttackPressed = false;
        meleeAttackReleased = false;

        magicAttackHold = false;
        magicAttackPressed = false;
        magicAttackReleased = false;

        quitHold = false;
        quitPressed = false;
        quitReleased = false;

        restartHold = false;
        restartPressed = false;
        restartReleased = false;

        keyHackHold = false;
        keyHackPressed = false;
        keyHackReleased = false;

        timeHackHold = false;
        timeHackPressed = false;
        timeHackReleased = false;

        fullscreenHackHold = false;
        fullscreenHackPressed = false;
        fullscreenHackReleased = false;

        resolutionUpHackHold = false;
        resolutionUpHackPressed = false;
        resolutionUpHackReleased = false;

        resolutionDownHackHold = false;
        resolutionDownHackPressed = false;
        resolutionDownHackReleased = false;

        outlineToggleHackHold = false;
        outlineToggleHackPressed = false;
        outlineToggleHackReleased = false;

        renderScaleToggleHackHold = false;
        renderScaleToggleHackPressed = false;
        renderScaleToggleHackReleased = false;

        interactHold = false;
        interactPressed = false;
        interactReleased = false;

        coinHackHold = false;
        coinHackPressed = false;
        coinHackReleased = false;

        tearHackHold = false;
        tearHackPressed = false;
        tearHackReleased = false;

        playerDeadHackHold = false;
        playerDeadHackPressed = false;
        playerDeadHackReleased = false;

        cancelHold = false;
        cancelPressed = false;
        cancelReleased = false;

        blockHold = false;
        blockPressed = false;
        blockReleased = false;

        kickHold = false;
        kickPressed = false;
        kickReleased = false;

        musicToggleHackHold = false;
        musicToggleHackPressed = false;
        musicToggleHackReleased = false;

        playerUIHackHold = false;
        playerUIHackPressed = false;
        playerUIHackReleased = false;
    }

    void SetLastInputType(InputType _to)
    {
        lastInputType = _to;
    }

    public void OnMousePosition(InputAction.CallbackContext _context)
    {
        mousePosition = _context.ReadValue<Vector2>();
    }

    public void OnMove ( InputAction.CallbackContext _context )
    {
        moveDirection = _context.ReadValue<Vector2>();
    }

    public void OnLook (InputAction.CallbackContext _context)
    {
        lookDirection = _context.ReadValue<Vector2>();
    }

    public void OnJump ( InputAction.CallbackContext _context )
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            jumpPressed = !jumpPressed;
        }
        jumpHold = buttonHold;
        if (!buttonHold)
        {
            jumpReleased = true;
        }
    }

    public void OnInteract(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            interactPressed = !interactPressed;
        }
        interactHold = buttonHold;
        if (!buttonHold)
        {
            interactReleased = true;
        }
    }

    public void OnCancel(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            cancelPressed = !cancelPressed;
        }
        cancelHold = buttonHold;
        if (!buttonHold)
        {
            cancelReleased = true;
        }
    }

    public void OnBlock(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            blockPressed = !blockPressed;
        }
        blockHold = buttonHold;
        if (!buttonHold)
        {
            blockReleased = true;
        }
    }

    public void OnKick(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            kickPressed = !kickPressed;
        }
        kickHold = buttonHold;
        if (!buttonHold)
        {
            kickReleased = true;
        }
    }

    public void OnMeleeAttack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            meleeAttackPressed = !meleeAttackPressed;
        }
        meleeAttackHold = buttonHold;
        if ( !buttonHold )
        {
            meleeAttackReleased = true;
        }
    }

    public void OnMagicAttack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            magicAttackPressed = !magicAttackPressed;
        }
        magicAttackHold = buttonHold;
        if (!buttonHold)
        {
            magicAttackReleased = true;
        }

        // log
        //Debug.Log("magic hold: " + magicAttackHold + " || magic press: " + magicAttackPressed + " || magic released: " + magicAttackReleased + " || " + Time.time.ToString());
    }

    public void OnQuit(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            quitPressed = !quitPressed;
        }
        quitHold = buttonHold;
        if (!buttonHold)
        {
            quitReleased = true;
        }
    }

    public void OnRestart(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            restartPressed = !restartPressed;
        }
        restartHold = buttonHold;
        if (!buttonHold)
        {
            restartReleased = true;
        }
    }

    // hacks
    public void OnKeyHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            keyHackPressed = !keyHackPressed;
        }
        keyHackHold = buttonHold;
        if (!buttonHold)
        {
            keyHackReleased = true;
        }
    }

    public void OnTimeHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            timeHackPressed = !timeHackPressed;
        }
        timeHackHold = buttonHold;
        if (!buttonHold)
        {
            timeHackReleased = true;
        }
    }

    public void OnFullscreenHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            fullscreenHackPressed = !fullscreenHackPressed;
        }
        fullscreenHackHold = buttonHold;
        if (!buttonHold)
        {
            fullscreenHackReleased = true;
        }
    }

    public void OnResolutionUpHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            resolutionUpHackPressed = !resolutionUpHackPressed;
        }
        resolutionUpHackHold = buttonHold;
        if (!buttonHold)
        {
            resolutionUpHackReleased = true;
        }
    }

    public void OnResolutionDownHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            resolutionDownHackPressed = !resolutionDownHackPressed;
        }
        resolutionDownHackHold = buttonHold;
        if (!buttonHold)
        {
            resolutionDownHackReleased = true;
        }
    }

    public void OnOutlineToggleHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            outlineToggleHackPressed = !outlineToggleHackPressed;
        }
        outlineToggleHackHold = buttonHold;
        if (!buttonHold)
        {
            outlineToggleHackReleased = true;
        }
    }

    public void OnRenderScaleToggleHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            renderScaleToggleHackPressed = !renderScaleToggleHackPressed;
        }
        renderScaleToggleHackHold = buttonHold;
        if (!buttonHold)
        {
            renderScaleToggleHackReleased = true;
        }
    }

    public void OnCoinHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            coinHackPressed = !coinHackPressed;
        }
        coinHackHold = buttonHold;
        if (!buttonHold)
        {
            coinHackReleased = true;
        }
    }

    public void OnTearHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            tearHackPressed = !tearHackPressed;
        }
        tearHackHold = buttonHold;
        if (!buttonHold)
        {
            tearHackReleased = true;
        }
    }

    public void OnPlayerDeadHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            playerDeadHackPressed = !playerDeadHackPressed;
        }
        playerDeadHackHold = buttonHold;
        if (!buttonHold)
        {
            playerDeadHackReleased = true;
        }
    }

    public void OnMusicToggleHack (InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            musicToggleHackPressed = !musicToggleHackPressed;
        }
        musicToggleHackHold = buttonHold;
        if (!buttonHold)
        {
            musicToggleHackReleased = true;
        }
    }

    public void OnPlayerUIHack(InputAction.CallbackContext _context)
    {
        bool buttonHold = _context.ReadValueAsButton();
        if (_context.started)
        {
            playerUIHackPressed = !playerUIHackPressed;
        }
        playerUIHackHold = buttonHold;
        if (!buttonHold)
        {
            playerUIHackReleased = true;
        }
    }
}
