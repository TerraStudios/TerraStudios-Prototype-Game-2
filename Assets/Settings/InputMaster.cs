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
            ""name"": ""Gameplay"",
            ""id"": ""a83ba1e6-d102-447b-98a6-cb504d571d1d"",
            ""actions"": [
                {
                    ""name"": ""Camera_Manager_SwitchTopdownCamera"",
                    ""type"": ""Button"",
                    ""id"": ""617d593f-acaf-426a-a451-ddfff894e1b1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Manager_SwitchFreecam"",
                    ""type"": ""Button"",
                    ""id"": ""c26e6300-e355-4acb-bd65-096559b64082"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Normal_Move"",
                    ""type"": ""Button"",
                    ""id"": ""382675d8-1eb0-4590-a13b-3eacc4fe9ed2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Normal_Rotate"",
                    ""type"": ""Button"",
                    ""id"": ""b7ead13b-af31-4543-ac92-90d18d67bd59"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Normal_UpAndDown"",
                    ""type"": ""Button"",
                    ""id"": ""524e7aa0-ae7f-4d34-ac16-74645528b457"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Normal_DragState"",
                    ""type"": ""PassThrough"",
                    ""id"": ""db7bac09-980a-4d93-a1a1-f521ce054f29"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Normal_PanRotateState"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8e43bdc9-795b-4431-8bd7-8347ca56d619"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Normal_PanAndDrag"",
                    ""type"": ""Value"",
                    ""id"": ""35523880-500f-4965-b3a7-f312a61d4083"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Normal_Zoom"",
                    ""type"": ""PassThrough"",
                    ""id"": ""02df1c6e-29d3-4237-b437-bdaf5219a660"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Freecam_Move"",
                    ""type"": ""Button"",
                    ""id"": ""fb1671e4-1b26-4048-8e7a-00be20f1cb13"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Freecam_Look"",
                    ""type"": ""Value"",
                    ""id"": ""e9dcaa5e-9d0b-441c-a752-cce57569a4c3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Freecam_UpAndDown"",
                    ""type"": ""Button"",
                    ""id"": ""c0cd1484-0313-4f69-a7ab-a9e395757c7d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera_Freecam_Faster"",
                    ""type"": ""PassThrough"",
                    ""id"": ""1bc334e3-cba9-4c74-bf58-aa10cbd42709"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RemoveSystem_RemoveTrigger"",
                    ""type"": ""Button"",
                    ""id"": ""74655bea-bb51-4e25-8d0a-3620f3fe9e78"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EconomyManager_TestBalanceChange"",
                    ""type"": ""Button"",
                    ""id"": ""8278b30d-d35f-4e33-ab6b-078526941f39"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ItemSelector_SelectItem"",
                    ""type"": ""Button"",
                    ""id"": ""c2fcf54f-9911-41dc-86da-c32933bf4928"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ItemSelector_Position"",
                    ""type"": ""Value"",
                    ""id"": ""fe726f2d-b752-41bb-85d3-4d343a3bc90f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GridManager_GridHitPosition"",
                    ""type"": ""Value"",
                    ""id"": ""6768d740-dc6c-4418-9106-e403e70676a9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GridManager_PlaceBuilding"",
                    ""type"": ""Button"",
                    ""id"": ""40879897-6dc4-49b9-8b2a-efbd03cc9b3f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GridManager_RotateBuildingLeft"",
                    ""type"": ""Button"",
                    ""id"": ""0db2edd5-043b-4b53-a1d2-eb263be1afca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GridManager_RotateBuildingRight"",
                    ""type"": ""Button"",
                    ""id"": ""6ee0803c-ac35-4f92-a8b5-164546b52a22"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GridManager_ContinueBuilding"",
                    ""type"": ""PassThrough"",
                    ""id"": ""61e0ef6e-cc51-427d-a316-11962b80e6f9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DebugUI_Show"",
                    ""type"": ""Button"",
                    ""id"": ""450f3aab-3aa4-4b63-9306-6b801591443a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PauseMenu_Show"",
                    ""type"": ""Button"",
                    ""id"": ""af29e5a0-72da-42f6-b381-888b686209a4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Direction"",
                    ""id"": ""404c04ca-9b92-4e29-80dd-f320cfebfbc0"",
                    ""path"": ""MovementVector3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera_Normal_Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""forward"",
                    ""id"": ""39b30153-546a-40fc-be71-3380a98920ae"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""forward"",
                    ""id"": ""6ff50420-3276-47fa-b3f8-a068ad62be4f"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""backward"",
                    ""id"": ""76705f79-57af-4425-a0c8-835f1d3a6326"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""backward"",
                    ""id"": ""2ba62076-5381-4886-b40f-d4941d3930e4"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5bcb43a4-265d-4f9d-9720-0a72f52dfafe"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b8bfbeb5-2f95-4be1-a904-46ea2f34e32f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d9511116-4530-4df2-ab16-d5feb1d6206e"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""15ffb44b-9c16-4385-9b38-0f6ac6b44055"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Direction"",
                    ""id"": ""1cb79b7c-eb9f-4829-8a87-305e7fb01a38"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera_Normal_Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""534291c2-ad6f-4275-86d8-3cc281a72af2"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""f75231ed-e6cf-41c1-8924-6d70283620e0"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""887c91f5-a6e9-4fe9-bd9c-889d2e31b7ee"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0f2199d7-4658-4c7d-b05e-3d7f5fd19ef1"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_DragState"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d2020a79-c090-4879-b739-8c716b2e3acf"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Direction"",
                    ""id"": ""c4cd847b-8ddd-45fa-8afa-61576d499032"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera_Freecam_UpAndDown"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""30496b90-39b3-49cd-8ef6-4fd69be8ee09"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_UpAndDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""402a01a9-c945-4047-93ab-15764df34ca5"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_UpAndDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e47c4e31-f06b-4b6d-8dbf-c88ea89ac61d"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Faster"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Direction"",
                    ""id"": ""e4a89455-df52-416e-b2af-2cb9cfc3fafb"",
                    ""path"": ""MovementVector3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera_Freecam_Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""forward"",
                    ""id"": ""073d25ac-3052-40c8-bfba-e274ff0701a6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""forward"",
                    ""id"": ""b100a7e6-7f28-4b9f-a97c-8aa70fcc2ea3"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""backward"",
                    ""id"": ""68191f66-efcb-42ee-8be2-ec1dfb7904b5"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""backward"",
                    ""id"": ""1ca77fd1-9320-4299-9cf7-ae9c0e53b983"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9f1e29f1-0226-40f7-972c-c92ca3fbc367"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""40e91db0-aa5b-427f-a232-a1280f8fb5ca"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""eb81e36b-5e8c-4ea5-9eca-cb3e2f581c10"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6dfc4c8e-1d01-4b69-8e10-02ff7477e5c2"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Freecam_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b3bddfa8-85e7-4262-b343-8b4a18a37800"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_PanAndDrag"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7cb3340b-f110-438b-845f-b6f7acd6192e"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_PanRotateState"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2acb9e93-6225-4c5a-828d-b928be8285d3"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Manager_SwitchTopdownCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8372dbf-2d4f-489b-977a-be77958ecc5a"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Manager_SwitchFreecam"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4f38bbfc-023f-4d47-81d1-1193c3e6105a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""RemoveSystem_RemoveTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f14c6fc0-7fef-4019-a82f-2e3e69515088"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""EconomyManager_TestBalanceChange"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48c43edd-ee78-4c75-844b-8685be02dda9"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ItemSelector_SelectItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cbb294ab-0ee1-4e50-8cde-fb15b60df48b"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ItemSelector_Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47bbcf15-377c-49b2-8885-5f4665d18a31"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""GridManager_PlaceBuilding"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ca846f42-0cfd-43d3-8054-02215b57037e"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""GridManager_GridHitPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f094a1a0-25c7-45e7-912e-914abe50bed4"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""GridManager_ContinueBuilding"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Show"",
                    ""id"": ""50a14a12-5e7f-447d-a3dd-5159948d099d"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DebugUI_Show"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""e02366db-74ac-4e1e-a6d7-7e7031329f40"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""DebugUI_Show"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""e5d5ab6f-cd29-452a-b23d-1411a3d9dd19"",
                    ""path"": ""<Keyboard>/rightCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""DebugUI_Show"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f1cba9ae-1289-43ea-a652-baf7ad918788"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PauseMenu_Show"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd006c71-412e-4d1f-9cf6-76ee1050345f"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""GridManager_RotateBuildingRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""064c756c-238e-4d03-9ccc-3cac0757445e"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""GridManager_RotateBuildingLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""UpAndDown"",
                    ""id"": ""7f15eb3c-c0ae-42db-b969-9aa8d587c4dd"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera_Normal_UpAndDown"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""19f9b7e0-ece2-44ca-a1c6-e5ca06507837"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_UpAndDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""c06db96a-2438-4ec1-a3d2-28606d227344"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Camera_Normal_UpAndDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Pause Menu"",
            ""id"": ""ec968856-9719-4c1c-858b-a9730ef0275a"",
            ""actions"": [
                {
                    ""name"": ""PauseMenu_Exit"",
                    ""type"": ""Button"",
                    ""id"": ""8f4e8b0f-4b55-4b92-a811-28b86a229a05"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""41a56f50-9fae-47b1-b0cf-801827c4769a"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PauseMenu_Exit"",
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
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Camera_Manager_SwitchTopdownCamera = m_Gameplay.FindAction("Camera_Manager_SwitchTopdownCamera", throwIfNotFound: true);
        m_Gameplay_Camera_Manager_SwitchFreecam = m_Gameplay.FindAction("Camera_Manager_SwitchFreecam", throwIfNotFound: true);
        m_Gameplay_Camera_Normal_Move = m_Gameplay.FindAction("Camera_Normal_Move", throwIfNotFound: true);
        m_Gameplay_Camera_Normal_Rotate = m_Gameplay.FindAction("Camera_Normal_Rotate", throwIfNotFound: true);
        m_Gameplay_Camera_Normal_UpAndDown = m_Gameplay.FindAction("Camera_Normal_UpAndDown", throwIfNotFound: true);
        m_Gameplay_Camera_Normal_DragState = m_Gameplay.FindAction("Camera_Normal_DragState", throwIfNotFound: true);
        m_Gameplay_Camera_Normal_PanRotateState = m_Gameplay.FindAction("Camera_Normal_PanRotateState", throwIfNotFound: true);
        m_Gameplay_Camera_Normal_PanAndDrag = m_Gameplay.FindAction("Camera_Normal_PanAndDrag", throwIfNotFound: true);
        m_Gameplay_Camera_Normal_Zoom = m_Gameplay.FindAction("Camera_Normal_Zoom", throwIfNotFound: true);
        m_Gameplay_Camera_Freecam_Move = m_Gameplay.FindAction("Camera_Freecam_Move", throwIfNotFound: true);
        m_Gameplay_Camera_Freecam_Look = m_Gameplay.FindAction("Camera_Freecam_Look", throwIfNotFound: true);
        m_Gameplay_Camera_Freecam_UpAndDown = m_Gameplay.FindAction("Camera_Freecam_UpAndDown", throwIfNotFound: true);
        m_Gameplay_Camera_Freecam_Faster = m_Gameplay.FindAction("Camera_Freecam_Faster", throwIfNotFound: true);
        m_Gameplay_RemoveSystem_RemoveTrigger = m_Gameplay.FindAction("RemoveSystem_RemoveTrigger", throwIfNotFound: true);
        m_Gameplay_EconomyManager_TestBalanceChange = m_Gameplay.FindAction("EconomyManager_TestBalanceChange", throwIfNotFound: true);
        m_Gameplay_ItemSelector_SelectItem = m_Gameplay.FindAction("ItemSelector_SelectItem", throwIfNotFound: true);
        m_Gameplay_ItemSelector_Position = m_Gameplay.FindAction("ItemSelector_Position", throwIfNotFound: true);
        m_Gameplay_GridManager_GridHitPosition = m_Gameplay.FindAction("GridManager_GridHitPosition", throwIfNotFound: true);
        m_Gameplay_GridManager_PlaceBuilding = m_Gameplay.FindAction("GridManager_PlaceBuilding", throwIfNotFound: true);
        m_Gameplay_GridManager_RotateBuildingLeft = m_Gameplay.FindAction("GridManager_RotateBuildingLeft", throwIfNotFound: true);
        m_Gameplay_GridManager_RotateBuildingRight = m_Gameplay.FindAction("GridManager_RotateBuildingRight", throwIfNotFound: true);
        m_Gameplay_GridManager_ContinueBuilding = m_Gameplay.FindAction("GridManager_ContinueBuilding", throwIfNotFound: true);
        m_Gameplay_DebugUI_Show = m_Gameplay.FindAction("DebugUI_Show", throwIfNotFound: true);
        m_Gameplay_PauseMenu_Show = m_Gameplay.FindAction("PauseMenu_Show", throwIfNotFound: true);
        // Pause Menu
        m_PauseMenu = asset.FindActionMap("Pause Menu", throwIfNotFound: true);
        m_PauseMenu_PauseMenu_Exit = m_PauseMenu.FindAction("PauseMenu_Exit", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Camera_Manager_SwitchTopdownCamera;
    private readonly InputAction m_Gameplay_Camera_Manager_SwitchFreecam;
    private readonly InputAction m_Gameplay_Camera_Normal_Move;
    private readonly InputAction m_Gameplay_Camera_Normal_Rotate;
    private readonly InputAction m_Gameplay_Camera_Normal_UpAndDown;
    private readonly InputAction m_Gameplay_Camera_Normal_DragState;
    private readonly InputAction m_Gameplay_Camera_Normal_PanRotateState;
    private readonly InputAction m_Gameplay_Camera_Normal_PanAndDrag;
    private readonly InputAction m_Gameplay_Camera_Normal_Zoom;
    private readonly InputAction m_Gameplay_Camera_Freecam_Move;
    private readonly InputAction m_Gameplay_Camera_Freecam_Look;
    private readonly InputAction m_Gameplay_Camera_Freecam_UpAndDown;
    private readonly InputAction m_Gameplay_Camera_Freecam_Faster;
    private readonly InputAction m_Gameplay_RemoveSystem_RemoveTrigger;
    private readonly InputAction m_Gameplay_EconomyManager_TestBalanceChange;
    private readonly InputAction m_Gameplay_ItemSelector_SelectItem;
    private readonly InputAction m_Gameplay_ItemSelector_Position;
    private readonly InputAction m_Gameplay_GridManager_GridHitPosition;
    private readonly InputAction m_Gameplay_GridManager_PlaceBuilding;
    private readonly InputAction m_Gameplay_GridManager_RotateBuildingLeft;
    private readonly InputAction m_Gameplay_GridManager_RotateBuildingRight;
    private readonly InputAction m_Gameplay_GridManager_ContinueBuilding;
    private readonly InputAction m_Gameplay_DebugUI_Show;
    private readonly InputAction m_Gameplay_PauseMenu_Show;
    public struct GameplayActions
    {
        private @InputMaster m_Wrapper;
        public GameplayActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Camera_Manager_SwitchTopdownCamera => m_Wrapper.m_Gameplay_Camera_Manager_SwitchTopdownCamera;
        public InputAction @Camera_Manager_SwitchFreecam => m_Wrapper.m_Gameplay_Camera_Manager_SwitchFreecam;
        public InputAction @Camera_Normal_Move => m_Wrapper.m_Gameplay_Camera_Normal_Move;
        public InputAction @Camera_Normal_Rotate => m_Wrapper.m_Gameplay_Camera_Normal_Rotate;
        public InputAction @Camera_Normal_UpAndDown => m_Wrapper.m_Gameplay_Camera_Normal_UpAndDown;
        public InputAction @Camera_Normal_DragState => m_Wrapper.m_Gameplay_Camera_Normal_DragState;
        public InputAction @Camera_Normal_PanRotateState => m_Wrapper.m_Gameplay_Camera_Normal_PanRotateState;
        public InputAction @Camera_Normal_PanAndDrag => m_Wrapper.m_Gameplay_Camera_Normal_PanAndDrag;
        public InputAction @Camera_Normal_Zoom => m_Wrapper.m_Gameplay_Camera_Normal_Zoom;
        public InputAction @Camera_Freecam_Move => m_Wrapper.m_Gameplay_Camera_Freecam_Move;
        public InputAction @Camera_Freecam_Look => m_Wrapper.m_Gameplay_Camera_Freecam_Look;
        public InputAction @Camera_Freecam_UpAndDown => m_Wrapper.m_Gameplay_Camera_Freecam_UpAndDown;
        public InputAction @Camera_Freecam_Faster => m_Wrapper.m_Gameplay_Camera_Freecam_Faster;
        public InputAction @RemoveSystem_RemoveTrigger => m_Wrapper.m_Gameplay_RemoveSystem_RemoveTrigger;
        public InputAction @EconomyManager_TestBalanceChange => m_Wrapper.m_Gameplay_EconomyManager_TestBalanceChange;
        public InputAction @ItemSelector_SelectItem => m_Wrapper.m_Gameplay_ItemSelector_SelectItem;
        public InputAction @ItemSelector_Position => m_Wrapper.m_Gameplay_ItemSelector_Position;
        public InputAction @GridManager_GridHitPosition => m_Wrapper.m_Gameplay_GridManager_GridHitPosition;
        public InputAction @GridManager_PlaceBuilding => m_Wrapper.m_Gameplay_GridManager_PlaceBuilding;
        public InputAction @GridManager_RotateBuildingLeft => m_Wrapper.m_Gameplay_GridManager_RotateBuildingLeft;
        public InputAction @GridManager_RotateBuildingRight => m_Wrapper.m_Gameplay_GridManager_RotateBuildingRight;
        public InputAction @GridManager_ContinueBuilding => m_Wrapper.m_Gameplay_GridManager_ContinueBuilding;
        public InputAction @DebugUI_Show => m_Wrapper.m_Gameplay_DebugUI_Show;
        public InputAction @PauseMenu_Show => m_Wrapper.m_Gameplay_PauseMenu_Show;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Camera_Manager_SwitchTopdownCamera.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Manager_SwitchTopdownCamera;
                @Camera_Manager_SwitchTopdownCamera.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Manager_SwitchTopdownCamera;
                @Camera_Manager_SwitchTopdownCamera.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Manager_SwitchTopdownCamera;
                @Camera_Manager_SwitchFreecam.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Manager_SwitchFreecam;
                @Camera_Manager_SwitchFreecam.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Manager_SwitchFreecam;
                @Camera_Manager_SwitchFreecam.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Manager_SwitchFreecam;
                @Camera_Normal_Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_Move;
                @Camera_Normal_Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_Move;
                @Camera_Normal_Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_Move;
                @Camera_Normal_Rotate.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_Rotate;
                @Camera_Normal_Rotate.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_Rotate;
                @Camera_Normal_Rotate.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_Rotate;
                @Camera_Normal_UpAndDown.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_UpAndDown;
                @Camera_Normal_UpAndDown.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_UpAndDown;
                @Camera_Normal_UpAndDown.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_UpAndDown;
                @Camera_Normal_DragState.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_DragState;
                @Camera_Normal_DragState.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_DragState;
                @Camera_Normal_DragState.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_DragState;
                @Camera_Normal_PanRotateState.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_PanRotateState;
                @Camera_Normal_PanRotateState.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_PanRotateState;
                @Camera_Normal_PanRotateState.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_PanRotateState;
                @Camera_Normal_PanAndDrag.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_PanAndDrag;
                @Camera_Normal_PanAndDrag.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_PanAndDrag;
                @Camera_Normal_PanAndDrag.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_PanAndDrag;
                @Camera_Normal_Zoom.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_Zoom;
                @Camera_Normal_Zoom.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_Zoom;
                @Camera_Normal_Zoom.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Normal_Zoom;
                @Camera_Freecam_Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_Move;
                @Camera_Freecam_Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_Move;
                @Camera_Freecam_Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_Move;
                @Camera_Freecam_Look.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_Look;
                @Camera_Freecam_Look.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_Look;
                @Camera_Freecam_Look.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_Look;
                @Camera_Freecam_UpAndDown.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_UpAndDown;
                @Camera_Freecam_UpAndDown.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_UpAndDown;
                @Camera_Freecam_UpAndDown.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_UpAndDown;
                @Camera_Freecam_Faster.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_Faster;
                @Camera_Freecam_Faster.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_Faster;
                @Camera_Freecam_Faster.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera_Freecam_Faster;
                @RemoveSystem_RemoveTrigger.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRemoveSystem_RemoveTrigger;
                @RemoveSystem_RemoveTrigger.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRemoveSystem_RemoveTrigger;
                @RemoveSystem_RemoveTrigger.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRemoveSystem_RemoveTrigger;
                @EconomyManager_TestBalanceChange.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEconomyManager_TestBalanceChange;
                @EconomyManager_TestBalanceChange.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEconomyManager_TestBalanceChange;
                @EconomyManager_TestBalanceChange.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEconomyManager_TestBalanceChange;
                @ItemSelector_SelectItem.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemSelector_SelectItem;
                @ItemSelector_SelectItem.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemSelector_SelectItem;
                @ItemSelector_SelectItem.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemSelector_SelectItem;
                @ItemSelector_Position.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemSelector_Position;
                @ItemSelector_Position.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemSelector_Position;
                @ItemSelector_Position.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemSelector_Position;
                @GridManager_GridHitPosition.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_GridHitPosition;
                @GridManager_GridHitPosition.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_GridHitPosition;
                @GridManager_GridHitPosition.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_GridHitPosition;
                @GridManager_PlaceBuilding.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_PlaceBuilding;
                @GridManager_PlaceBuilding.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_PlaceBuilding;
                @GridManager_PlaceBuilding.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_PlaceBuilding;
                @GridManager_RotateBuildingLeft.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_RotateBuildingLeft;
                @GridManager_RotateBuildingLeft.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_RotateBuildingLeft;
                @GridManager_RotateBuildingLeft.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_RotateBuildingLeft;
                @GridManager_RotateBuildingRight.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_RotateBuildingRight;
                @GridManager_RotateBuildingRight.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_RotateBuildingRight;
                @GridManager_RotateBuildingRight.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_RotateBuildingRight;
                @GridManager_ContinueBuilding.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_ContinueBuilding;
                @GridManager_ContinueBuilding.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_ContinueBuilding;
                @GridManager_ContinueBuilding.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGridManager_ContinueBuilding;
                @DebugUI_Show.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDebugUI_Show;
                @DebugUI_Show.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDebugUI_Show;
                @DebugUI_Show.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDebugUI_Show;
                @PauseMenu_Show.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPauseMenu_Show;
                @PauseMenu_Show.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPauseMenu_Show;
                @PauseMenu_Show.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPauseMenu_Show;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Camera_Manager_SwitchTopdownCamera.started += instance.OnCamera_Manager_SwitchTopdownCamera;
                @Camera_Manager_SwitchTopdownCamera.performed += instance.OnCamera_Manager_SwitchTopdownCamera;
                @Camera_Manager_SwitchTopdownCamera.canceled += instance.OnCamera_Manager_SwitchTopdownCamera;
                @Camera_Manager_SwitchFreecam.started += instance.OnCamera_Manager_SwitchFreecam;
                @Camera_Manager_SwitchFreecam.performed += instance.OnCamera_Manager_SwitchFreecam;
                @Camera_Manager_SwitchFreecam.canceled += instance.OnCamera_Manager_SwitchFreecam;
                @Camera_Normal_Move.started += instance.OnCamera_Normal_Move;
                @Camera_Normal_Move.performed += instance.OnCamera_Normal_Move;
                @Camera_Normal_Move.canceled += instance.OnCamera_Normal_Move;
                @Camera_Normal_Rotate.started += instance.OnCamera_Normal_Rotate;
                @Camera_Normal_Rotate.performed += instance.OnCamera_Normal_Rotate;
                @Camera_Normal_Rotate.canceled += instance.OnCamera_Normal_Rotate;
                @Camera_Normal_UpAndDown.started += instance.OnCamera_Normal_UpAndDown;
                @Camera_Normal_UpAndDown.performed += instance.OnCamera_Normal_UpAndDown;
                @Camera_Normal_UpAndDown.canceled += instance.OnCamera_Normal_UpAndDown;
                @Camera_Normal_DragState.started += instance.OnCamera_Normal_DragState;
                @Camera_Normal_DragState.performed += instance.OnCamera_Normal_DragState;
                @Camera_Normal_DragState.canceled += instance.OnCamera_Normal_DragState;
                @Camera_Normal_PanRotateState.started += instance.OnCamera_Normal_PanRotateState;
                @Camera_Normal_PanRotateState.performed += instance.OnCamera_Normal_PanRotateState;
                @Camera_Normal_PanRotateState.canceled += instance.OnCamera_Normal_PanRotateState;
                @Camera_Normal_PanAndDrag.started += instance.OnCamera_Normal_PanAndDrag;
                @Camera_Normal_PanAndDrag.performed += instance.OnCamera_Normal_PanAndDrag;
                @Camera_Normal_PanAndDrag.canceled += instance.OnCamera_Normal_PanAndDrag;
                @Camera_Normal_Zoom.started += instance.OnCamera_Normal_Zoom;
                @Camera_Normal_Zoom.performed += instance.OnCamera_Normal_Zoom;
                @Camera_Normal_Zoom.canceled += instance.OnCamera_Normal_Zoom;
                @Camera_Freecam_Move.started += instance.OnCamera_Freecam_Move;
                @Camera_Freecam_Move.performed += instance.OnCamera_Freecam_Move;
                @Camera_Freecam_Move.canceled += instance.OnCamera_Freecam_Move;
                @Camera_Freecam_Look.started += instance.OnCamera_Freecam_Look;
                @Camera_Freecam_Look.performed += instance.OnCamera_Freecam_Look;
                @Camera_Freecam_Look.canceled += instance.OnCamera_Freecam_Look;
                @Camera_Freecam_UpAndDown.started += instance.OnCamera_Freecam_UpAndDown;
                @Camera_Freecam_UpAndDown.performed += instance.OnCamera_Freecam_UpAndDown;
                @Camera_Freecam_UpAndDown.canceled += instance.OnCamera_Freecam_UpAndDown;
                @Camera_Freecam_Faster.started += instance.OnCamera_Freecam_Faster;
                @Camera_Freecam_Faster.performed += instance.OnCamera_Freecam_Faster;
                @Camera_Freecam_Faster.canceled += instance.OnCamera_Freecam_Faster;
                @RemoveSystem_RemoveTrigger.started += instance.OnRemoveSystem_RemoveTrigger;
                @RemoveSystem_RemoveTrigger.performed += instance.OnRemoveSystem_RemoveTrigger;
                @RemoveSystem_RemoveTrigger.canceled += instance.OnRemoveSystem_RemoveTrigger;
                @EconomyManager_TestBalanceChange.started += instance.OnEconomyManager_TestBalanceChange;
                @EconomyManager_TestBalanceChange.performed += instance.OnEconomyManager_TestBalanceChange;
                @EconomyManager_TestBalanceChange.canceled += instance.OnEconomyManager_TestBalanceChange;
                @ItemSelector_SelectItem.started += instance.OnItemSelector_SelectItem;
                @ItemSelector_SelectItem.performed += instance.OnItemSelector_SelectItem;
                @ItemSelector_SelectItem.canceled += instance.OnItemSelector_SelectItem;
                @ItemSelector_Position.started += instance.OnItemSelector_Position;
                @ItemSelector_Position.performed += instance.OnItemSelector_Position;
                @ItemSelector_Position.canceled += instance.OnItemSelector_Position;
                @GridManager_GridHitPosition.started += instance.OnGridManager_GridHitPosition;
                @GridManager_GridHitPosition.performed += instance.OnGridManager_GridHitPosition;
                @GridManager_GridHitPosition.canceled += instance.OnGridManager_GridHitPosition;
                @GridManager_PlaceBuilding.started += instance.OnGridManager_PlaceBuilding;
                @GridManager_PlaceBuilding.performed += instance.OnGridManager_PlaceBuilding;
                @GridManager_PlaceBuilding.canceled += instance.OnGridManager_PlaceBuilding;
                @GridManager_RotateBuildingLeft.started += instance.OnGridManager_RotateBuildingLeft;
                @GridManager_RotateBuildingLeft.performed += instance.OnGridManager_RotateBuildingLeft;
                @GridManager_RotateBuildingLeft.canceled += instance.OnGridManager_RotateBuildingLeft;
                @GridManager_RotateBuildingRight.started += instance.OnGridManager_RotateBuildingRight;
                @GridManager_RotateBuildingRight.performed += instance.OnGridManager_RotateBuildingRight;
                @GridManager_RotateBuildingRight.canceled += instance.OnGridManager_RotateBuildingRight;
                @GridManager_ContinueBuilding.started += instance.OnGridManager_ContinueBuilding;
                @GridManager_ContinueBuilding.performed += instance.OnGridManager_ContinueBuilding;
                @GridManager_ContinueBuilding.canceled += instance.OnGridManager_ContinueBuilding;
                @DebugUI_Show.started += instance.OnDebugUI_Show;
                @DebugUI_Show.performed += instance.OnDebugUI_Show;
                @DebugUI_Show.canceled += instance.OnDebugUI_Show;
                @PauseMenu_Show.started += instance.OnPauseMenu_Show;
                @PauseMenu_Show.performed += instance.OnPauseMenu_Show;
                @PauseMenu_Show.canceled += instance.OnPauseMenu_Show;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Pause Menu
    private readonly InputActionMap m_PauseMenu;
    private IPauseMenuActions m_PauseMenuActionsCallbackInterface;
    private readonly InputAction m_PauseMenu_PauseMenu_Exit;
    public struct PauseMenuActions
    {
        private @InputMaster m_Wrapper;
        public PauseMenuActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @PauseMenu_Exit => m_Wrapper.m_PauseMenu_PauseMenu_Exit;
        public InputActionMap Get() { return m_Wrapper.m_PauseMenu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PauseMenuActions set) { return set.Get(); }
        public void SetCallbacks(IPauseMenuActions instance)
        {
            if (m_Wrapper.m_PauseMenuActionsCallbackInterface != null)
            {
                @PauseMenu_Exit.started -= m_Wrapper.m_PauseMenuActionsCallbackInterface.OnPauseMenu_Exit;
                @PauseMenu_Exit.performed -= m_Wrapper.m_PauseMenuActionsCallbackInterface.OnPauseMenu_Exit;
                @PauseMenu_Exit.canceled -= m_Wrapper.m_PauseMenuActionsCallbackInterface.OnPauseMenu_Exit;
            }
            m_Wrapper.m_PauseMenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PauseMenu_Exit.started += instance.OnPauseMenu_Exit;
                @PauseMenu_Exit.performed += instance.OnPauseMenu_Exit;
                @PauseMenu_Exit.canceled += instance.OnPauseMenu_Exit;
            }
        }
    }
    public PauseMenuActions @PauseMenu => new PauseMenuActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnCamera_Manager_SwitchTopdownCamera(InputAction.CallbackContext context);
        void OnCamera_Manager_SwitchFreecam(InputAction.CallbackContext context);
        void OnCamera_Normal_Move(InputAction.CallbackContext context);
        void OnCamera_Normal_Rotate(InputAction.CallbackContext context);
        void OnCamera_Normal_UpAndDown(InputAction.CallbackContext context);
        void OnCamera_Normal_DragState(InputAction.CallbackContext context);
        void OnCamera_Normal_PanRotateState(InputAction.CallbackContext context);
        void OnCamera_Normal_PanAndDrag(InputAction.CallbackContext context);
        void OnCamera_Normal_Zoom(InputAction.CallbackContext context);
        void OnCamera_Freecam_Move(InputAction.CallbackContext context);
        void OnCamera_Freecam_Look(InputAction.CallbackContext context);
        void OnCamera_Freecam_UpAndDown(InputAction.CallbackContext context);
        void OnCamera_Freecam_Faster(InputAction.CallbackContext context);
        void OnRemoveSystem_RemoveTrigger(InputAction.CallbackContext context);
        void OnEconomyManager_TestBalanceChange(InputAction.CallbackContext context);
        void OnItemSelector_SelectItem(InputAction.CallbackContext context);
        void OnItemSelector_Position(InputAction.CallbackContext context);
        void OnGridManager_GridHitPosition(InputAction.CallbackContext context);
        void OnGridManager_PlaceBuilding(InputAction.CallbackContext context);
        void OnGridManager_RotateBuildingLeft(InputAction.CallbackContext context);
        void OnGridManager_RotateBuildingRight(InputAction.CallbackContext context);
        void OnGridManager_ContinueBuilding(InputAction.CallbackContext context);
        void OnDebugUI_Show(InputAction.CallbackContext context);
        void OnPauseMenu_Show(InputAction.CallbackContext context);
    }
    public interface IPauseMenuActions
    {
        void OnPauseMenu_Exit(InputAction.CallbackContext context);
    }
}
