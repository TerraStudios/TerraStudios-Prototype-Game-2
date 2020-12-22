// GENERATED AUTOMATICALLY FROM 'Assets/Settings/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Camera"",
            ""id"": ""a83ba1e6-d102-447b-98a6-cb504d571d1d"",
            ""actions"": [
                {
                    ""name"": ""ForwardMovement"",
                    ""type"": ""Button"",
                    ""id"": ""2ccb3096-ff7d-4ab2-9e9e-e65892c1aa84"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Left Movement"",
                    ""type"": ""Button"",
                    ""id"": ""52e9e3ab-bfdb-4435-bf75-a698db7bbe75"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BackwardMovement"",
                    ""type"": ""Button"",
                    ""id"": ""c68babb4-657a-4cde-b02e-0afa8cc8f733"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightMovement"",
                    ""type"": ""Button"",
                    ""id"": ""5aa7c6dd-0896-402a-b524-647aa14d71a8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RotateLeft"",
                    ""type"": ""Button"",
                    ""id"": ""7f6550d2-775a-4327-aabf-9553d60307af"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RotateRight"",
                    ""type"": ""Button"",
                    ""id"": ""064b6b98-8680-42b1-a901-80e65d647638"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f98ac97b-30a7-4b75-8e4b-c63ef2c16890"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ForwardMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""08679edb-f053-43d6-90e2-74246ce686da"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ForwardMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54a085be-ba83-4ba9-b034-df879f308fef"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Left Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5c0999b3-dfe6-4af0-8479-588be29f1580"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Left Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bef41aee-2b13-4893-8085-54b26839c26b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""BackwardMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de742b37-3dd7-4621-8f1d-2f353d70c7e6"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""BackwardMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9e02dbcb-935c-4905-9b83-e4d766fb75b2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""RightMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb598bff-2381-4a86-a886-71be530114da"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""RightMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a6728c21-4821-425a-b4c3-a0a7ad2c66ca"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""RotateLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""94f0be4d-9ead-4168-8c9e-d67cbc624260"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""RotateRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Camera
        m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
        m_Camera_ForwardMovement = m_Camera.FindAction("ForwardMovement", throwIfNotFound: true);
        m_Camera_LeftMovement = m_Camera.FindAction("Left Movement", throwIfNotFound: true);
        m_Camera_BackwardMovement = m_Camera.FindAction("BackwardMovement", throwIfNotFound: true);
        m_Camera_RightMovement = m_Camera.FindAction("RightMovement", throwIfNotFound: true);
        m_Camera_RotateLeft = m_Camera.FindAction("RotateLeft", throwIfNotFound: true);
        m_Camera_RotateRight = m_Camera.FindAction("RotateRight", throwIfNotFound: true);
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

    // Camera
    private readonly InputActionMap m_Camera;
    private ICameraActions m_CameraActionsCallbackInterface;
    private readonly InputAction m_Camera_ForwardMovement;
    private readonly InputAction m_Camera_LeftMovement;
    private readonly InputAction m_Camera_BackwardMovement;
    private readonly InputAction m_Camera_RightMovement;
    private readonly InputAction m_Camera_RotateLeft;
    private readonly InputAction m_Camera_RotateRight;
    public struct CameraActions
    {
        private @InputMaster m_Wrapper;
        public CameraActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @ForwardMovement => m_Wrapper.m_Camera_ForwardMovement;
        public InputAction @LeftMovement => m_Wrapper.m_Camera_LeftMovement;
        public InputAction @BackwardMovement => m_Wrapper.m_Camera_BackwardMovement;
        public InputAction @RightMovement => m_Wrapper.m_Camera_RightMovement;
        public InputAction @RotateLeft => m_Wrapper.m_Camera_RotateLeft;
        public InputAction @RotateRight => m_Wrapper.m_Camera_RotateRight;
        public InputActionMap Get() { return m_Wrapper.m_Camera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
        public void SetCallbacks(ICameraActions instance)
        {
            if (m_Wrapper.m_CameraActionsCallbackInterface != null)
            {
                @ForwardMovement.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnForwardMovement;
                @ForwardMovement.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnForwardMovement;
                @ForwardMovement.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnForwardMovement;
                @LeftMovement.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnLeftMovement;
                @LeftMovement.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnLeftMovement;
                @LeftMovement.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnLeftMovement;
                @BackwardMovement.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnBackwardMovement;
                @BackwardMovement.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnBackwardMovement;
                @BackwardMovement.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnBackwardMovement;
                @RightMovement.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnRightMovement;
                @RightMovement.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnRightMovement;
                @RightMovement.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnRightMovement;
                @RotateLeft.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotateLeft;
                @RotateLeft.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotateLeft;
                @RotateLeft.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotateLeft;
                @RotateRight.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotateRight;
                @RotateRight.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotateRight;
                @RotateRight.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotateRight;
            }
            m_Wrapper.m_CameraActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ForwardMovement.started += instance.OnForwardMovement;
                @ForwardMovement.performed += instance.OnForwardMovement;
                @ForwardMovement.canceled += instance.OnForwardMovement;
                @LeftMovement.started += instance.OnLeftMovement;
                @LeftMovement.performed += instance.OnLeftMovement;
                @LeftMovement.canceled += instance.OnLeftMovement;
                @BackwardMovement.started += instance.OnBackwardMovement;
                @BackwardMovement.performed += instance.OnBackwardMovement;
                @BackwardMovement.canceled += instance.OnBackwardMovement;
                @RightMovement.started += instance.OnRightMovement;
                @RightMovement.performed += instance.OnRightMovement;
                @RightMovement.canceled += instance.OnRightMovement;
                @RotateLeft.started += instance.OnRotateLeft;
                @RotateLeft.performed += instance.OnRotateLeft;
                @RotateLeft.canceled += instance.OnRotateLeft;
                @RotateRight.started += instance.OnRotateRight;
                @RotateRight.performed += instance.OnRotateRight;
                @RotateRight.canceled += instance.OnRotateRight;
            }
        }
    }
    public CameraActions @Camera => new CameraActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    public interface ICameraActions
    {
        void OnForwardMovement(InputAction.CallbackContext context);
        void OnLeftMovement(InputAction.CallbackContext context);
        void OnBackwardMovement(InputAction.CallbackContext context);
        void OnRightMovement(InputAction.CallbackContext context);
        void OnRotateLeft(InputAction.CallbackContext context);
        void OnRotateRight(InputAction.CallbackContext context);
    }
}
