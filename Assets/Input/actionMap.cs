// GENERATED AUTOMATICALLY FROM 'Assets/Input/actionMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @ActionMap : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @ActionMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""actionMap"",
    ""maps"": [
        {
            ""name"": ""Overworld"",
            ""id"": ""cdf163c4-4ca9-4af5-a4ef-d2c91f73ffe7"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""b1cdf558-6c04-40fa-9d02-4bbbc8b126bc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""090a6820-9987-43f7-a6fa-d5f500115d80"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""67126046-b6c5-4c44-8788-4722e7dbcf34"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""ab4fb819-d720-4e02-9066-834d974da4c1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""arrowkeys"",
                    ""id"": ""e5bae47b-2d8a-4b3c-a2a0-c4ec39740d2b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""704e5942-0673-4e4c-9d95-cf7f93a63986"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e7880c3d-58e2-4cc5-b31e-c9a575f3aafd"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""cfe38cd6-4038-4a5b-9c21-c21301c63bce"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d3758d69-87fd-450b-a96d-3c3e545002a4"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f785b42a-6333-4643-94c2-fa9d6d02c178"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7629e543-f842-4473-bc1b-3930fc587c0b"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47afc3a8-f2ad-4d0f-b908-7930bc64bf21"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""49ac8f62-4826-4aa6-b45b-e8135c5d2b4a"",
            ""actions"": [
                {
                    ""name"": ""UIMove"",
                    ""type"": ""Value"",
                    ""id"": ""f020dfb4-d2fa-4146-97ee-c83716e7c91b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UIBack"",
                    ""type"": ""Button"",
                    ""id"": ""bc82e436-6ada-49b2-86e3-61e0dc659f89"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UISelect"",
                    ""type"": ""Button"",
                    ""id"": ""9ef26083-75c7-4785-a5af-dede9982ec7a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""349fc08c-3d6c-4202-8c70-17ca07b78b90"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UIMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e8a01e35-0273-4a03-9d5a-f05118595694"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UIMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8856a732-b747-4e2a-959a-d58af053cec4"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UIMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bab53343-f7e6-4b1e-bdf1-20640e9886a3"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UIMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f855a4b5-972e-447e-8f76-e08182bbd023"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UIMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e270bf8e-60c9-4633-af9f-9338bf8957c1"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UIBack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f0f5f18-d0d7-46bf-a1fa-0002ea6a0452"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UISelect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""default"",
            ""bindingGroup"": ""default"",
            ""devices"": []
        }
    ]
}");
        // Overworld
        m_Overworld = asset.FindActionMap("Overworld", throwIfNotFound: true);
        m_Overworld_Move = m_Overworld.FindAction("Move", throwIfNotFound: true);
        m_Overworld_Select = m_Overworld.FindAction("Select", throwIfNotFound: true);
        m_Overworld_Back = m_Overworld.FindAction("Back", throwIfNotFound: true);
        m_Overworld_Pause = m_Overworld.FindAction("Pause", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_UIMove = m_UI.FindAction("UIMove", throwIfNotFound: true);
        m_UI_UIBack = m_UI.FindAction("UIBack", throwIfNotFound: true);
        m_UI_UISelect = m_UI.FindAction("UISelect", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Overworld
    private readonly InputActionMap m_Overworld;
    private IOverworldActions m_OverworldActionsCallbackInterface;
    private readonly InputAction m_Overworld_Move;
    private readonly InputAction m_Overworld_Select;
    private readonly InputAction m_Overworld_Back;
    private readonly InputAction m_Overworld_Pause;
    public struct OverworldActions
    {
        private @ActionMap m_Wrapper;
        public OverworldActions(@ActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Overworld_Move;
        public InputAction @Select => m_Wrapper.m_Overworld_Select;
        public InputAction @Back => m_Wrapper.m_Overworld_Back;
        public InputAction @Pause => m_Wrapper.m_Overworld_Pause;
        public InputActionMap Get() { return m_Wrapper.m_Overworld; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(OverworldActions set) { return set.Get(); }
        public void SetCallbacks(IOverworldActions instance)
        {
            if (m_Wrapper.m_OverworldActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_OverworldActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_OverworldActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_OverworldActionsCallbackInterface.OnMove;
                @Select.started -= m_Wrapper.m_OverworldActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_OverworldActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_OverworldActionsCallbackInterface.OnSelect;
                @Back.started -= m_Wrapper.m_OverworldActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_OverworldActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_OverworldActionsCallbackInterface.OnBack;
                @Pause.started -= m_Wrapper.m_OverworldActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_OverworldActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_OverworldActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_OverworldActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public OverworldActions @Overworld => new OverworldActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_UIMove;
    private readonly InputAction m_UI_UIBack;
    private readonly InputAction m_UI_UISelect;
    public struct UIActions
    {
        private @ActionMap m_Wrapper;
        public UIActions(@ActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @UIMove => m_Wrapper.m_UI_UIMove;
        public InputAction @UIBack => m_Wrapper.m_UI_UIBack;
        public InputAction @UISelect => m_Wrapper.m_UI_UISelect;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @UIMove.started -= m_Wrapper.m_UIActionsCallbackInterface.OnUIMove;
                @UIMove.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnUIMove;
                @UIMove.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnUIMove;
                @UIBack.started -= m_Wrapper.m_UIActionsCallbackInterface.OnUIBack;
                @UIBack.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnUIBack;
                @UIBack.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnUIBack;
                @UISelect.started -= m_Wrapper.m_UIActionsCallbackInterface.OnUISelect;
                @UISelect.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnUISelect;
                @UISelect.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnUISelect;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @UIMove.started += instance.OnUIMove;
                @UIMove.performed += instance.OnUIMove;
                @UIMove.canceled += instance.OnUIMove;
                @UIBack.started += instance.OnUIBack;
                @UIBack.performed += instance.OnUIBack;
                @UIBack.canceled += instance.OnUIBack;
                @UISelect.started += instance.OnUISelect;
                @UISelect.performed += instance.OnUISelect;
                @UISelect.canceled += instance.OnUISelect;
            }
        }
    }
    public UIActions @UI => new UIActions(this);
    private int m_defaultSchemeIndex = -1;
    public InputControlScheme defaultScheme
    {
        get
        {
            if (m_defaultSchemeIndex == -1) m_defaultSchemeIndex = asset.FindControlSchemeIndex("default");
            return asset.controlSchemes[m_defaultSchemeIndex];
        }
    }
    public interface IOverworldActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnSelect(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnUIMove(InputAction.CallbackContext context);
        void OnUIBack(InputAction.CallbackContext context);
        void OnUISelect(InputAction.CallbackContext context);
    }
}
