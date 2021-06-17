using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[AlwaysUpdateSystem]
[UpdateBefore(typeof(SaveTriggerSystem))]
[UpdateBefore(typeof(SaveAndLoadSystem))]
[UpdateBefore(typeof(MovementSystem))]
[UpdateBefore(typeof(CreditsMenuSystem))]
[UpdateBefore(typeof(OptionMenuSystem))]
[UpdateBefore(typeof(PauseMenuSystem))]
[UpdateBefore(typeof(TitleScreenSystem))]
public class InputGatheringSystem : ComponentSystem,
    ActionMap.IOverworldActions,
    ActionMap.IUIActions
{
    static ActionMap m_InputActions;

    EntityQuery m_UIInputInputQuery;
    EntityQuery m_OverWorldInputQuery;

    Vector2 m_CharacterMove;
    Vector2 m_UIMove;
    bool m_CharacterSprint;

    bool m_UIMovedRight;
    bool m_UIMovedLeft;
    bool m_UIMovedUp;
    bool m_UIMovedDown;

    bool m_CharacterSelect;
    bool m_CharacterSelected;
    bool m_UISelect;
    bool m_UISelected;

    bool m_CharacterBack;
    bool m_UIBack;
    bool m_UIBacked;

    bool m_CharacterPause;

    public static CurrentInput currentInput;
    

    protected override void OnCreate()
    {
        m_InputActions = new ActionMap();
        m_InputActions.Overworld.SetCallbacks(this);
        m_InputActions.UI.SetCallbacks(this);

        m_UIInputInputQuery = GetEntityQuery(typeof(UIInputData));
        m_OverWorldInputQuery = GetEntityQuery(typeof(OverworldInputData));
    }

    protected override void OnStartRunning() => m_InputActions.Enable();
    protected override void OnStopRunning() => m_InputActions.Disable();

    protected override void OnUpdate()
    {
        if (m_OverWorldInputQuery.CalculateEntityCount() == 0)
        {
            EntityManager.CreateEntity(typeof(OverworldInputData));
        }
        if (m_UIInputInputQuery.CalculateEntityCount() == 0)
        {
            EntityManager.CreateEntity(typeof(UIInputData));
        }

        switch (currentInput)
        {
            case CurrentInput.overworld:
                m_UIInputInputQuery.SetSingleton(new UIInputData
                {
                    movedown = false,
                    moveup = false,
                    moveleft = false,
                    moveright = false,
                    goselected = false,
                    goback = false
                });
                m_UISelected = m_CharacterSelect;
                m_UIBacked = m_CharacterBack;

                m_OverWorldInputQuery.SetSingleton(new OverworldInputData
                {
                    moveVertical = Mathf.Abs(m_CharacterMove.y) > .1 ? m_CharacterMove.y : 0,
                    moveHorizontal = Mathf.Abs(m_CharacterMove.x) > .1 ? m_CharacterMove.x : 0,

                    escape = m_CharacterPause,

                    select = m_CharacterSelect && !m_CharacterSelected,
                    back = m_CharacterBack,
                    sprint = m_CharacterSprint
                });
                m_CharacterSelected = m_CharacterSelect;
                break;
            case CurrentInput.ui:
                m_UIInputInputQuery.SetSingleton(new UIInputData
                {
                    movedown = m_UIMove.y < -.1 && !m_UIMovedDown,
                    moveup = m_UIMove.y > .1 && !m_UIMovedUp,
                    moveleft = m_UIMove.x < -.1 && !m_UIMovedLeft,
                    moveright = m_UIMove.x > .1 && !m_UIMovedRight,
                    goselected = m_UISelect && !m_UISelected,
                    goback = m_UIBack && !m_UIBacked
                });
                m_UIMovedDown = m_UIMove.y < -.1;
                m_UIMovedUp = m_UIMove.y > .1;
                m_UIMovedLeft = m_UIMove.x < -.1;
                m_UIMovedRight = m_UIMove.x > .1;

                m_UISelected = m_UISelect;
                m_UIBacked = m_UIBack;
                m_OverWorldInputQuery.SetSingleton(new OverworldInputData
                {
                    moveVertical = 0,
                    moveHorizontal = 0,

                    escape = false,

                    select = false,
                    back = false
                });
                m_CharacterSelected = m_UISelect;

                break;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_CharacterMove = context.ReadValue<Vector2>();
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        m_CharacterSelect = context.ReadValue<float>() > 0;
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        m_CharacterBack = context.ReadValue<float>() > 0;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        m_CharacterPause = context.ReadValue<float>() > 0;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        m_CharacterSprint = context.ReadValue<float>() > 0;
    }

    public void OnUIMove(InputAction.CallbackContext context)
    {
        m_UIMove = context.ReadValue<Vector2>();
    }

    public void OnUIBack(InputAction.CallbackContext context)
    {
        m_UIBack = context.ReadValue<float>() > 0;
    }

    public void OnUISelect(InputAction.CallbackContext context)
    {
        m_UISelect = context.ReadValue<float>() > 0;
    }
}

public enum CurrentInput
{
    overworld,
    ui
}
