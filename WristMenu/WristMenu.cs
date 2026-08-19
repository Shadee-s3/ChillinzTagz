using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Granalax.Mods
{
    public class WristMenu : MonoBehaviour
    {
        private GameObject board;
        private GameObject pointer;

        private Transform vrHand;

        private InputDevice leftController;

        private bool foundController;

        private void Start()
        {
            CreateBoard();
            CreatePointer();

            FindVRController();
        }

        private void Update()
        {
            if (!foundController ||
                !leftController.isValid)
            {
                FindVRController();
            }

            UpdateVRHand();
        }

        // =====================================================
        // FIND VR CONTROLLER
        // =====================================================

        private void FindVRController()
        {
            List<InputDevice> devices =
                new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.HeldInHand |
                InputDeviceCharacteristics.Left |
                InputDeviceCharacteristics.Controller,
                devices
            );

            if (devices.Count > 0)
            {
                leftController =
                    devices[0];

                foundController = true;
            }
        }

        // =====================================================
        // VR HAND
        // =====================================================

        private void UpdateVRHand()
        {
            if (!foundController)
                return;

            Vector3 position;

            Quaternion rotation;

            if (!leftController.TryGetFeatureValue(
                CommonUsages.devicePosition,
                out position))
            {
                return;
            }

            if (!leftController.TryGetFeatureValue(
                CommonUsages.deviceRotation,
                out rotation))
            {
                return;
            }

            /*
             * Convert controller tracking into
             * world space using the XR origin.
             */

            Transform camera =
                Camera.main != null
                    ? Camera.main.transform
                    : null;

            if (camera == null)
                return;

            Vector3 worldPosition =
                camera.TransformPoint(position);

            Quaternion worldRotation =
                camera.rotation *
                rotation;

            if (board != null)
            {
                /*
                 * Move the board toward the
                 * top/back of the controller.
                 */

                board.transform.position =
                    worldPosition
                    + worldRotation *
                    new Vector3(
                        0f,
                        0.055f,
                        -0.035f
                    );

                board.transform.rotation =
                    worldRotation *
                    Quaternion.Euler(
                        90f,
                        0f,
                        0f
                    );
            }

            if (pointer != null)
            {
                pointer.transform.position =
                    board.transform.position
                    + board.transform.forward
                    * 0.055f;
            }
        }

        // =====================================================
        // BOARD
        // =====================================================

        private void CreateBoard()
        {
            board =
                GameObject.CreatePrimitive(
                    PrimitiveType.Cube
                );

            board.name =
                "ChillzWristMenu";

            board.transform.localScale =
                new Vector3(
                    0.30f,
                    0.14f,
                    0.008f
                );

            Renderer renderer =
                board.GetComponent<Renderer>();

            if (renderer != null)
            {
                Material material =
                    CreateTransparentMaterial();

                material.color =
                    new Color(
                        0.05f,
                        1f,
                        0.15f,
                        0.5f
                    );

                renderer.material =
                    material;
            }

            RemoveCollider(board);

            CreateButtons();
        }

        // =====================================================
        // BUTTONS
        // =====================================================

        private void CreateButtons()
        {
            CreateButton(
                "MIC",
                new Vector3(
                    -0.09f,
                    0f,
                    -0.008f
                )
            );

            CreateButton(
                "SETTINGS",
                new Vector3(
                    0f,
                    0f,
                    -0.008f
                )
            );

            CreateButton(
                "INFO",
                new Vector3(
                    0.09f,
                    0f,
                    -0.008f
                )
            );
        }

        private GameObject CreateButton(
            string buttonName,
            Vector3 position)
        {
            GameObject button =
                GameObject.CreatePrimitive(
                    PrimitiveType.Cube
                );

            button.name =
                "ChillzButton_" +
                buttonName;

            button.transform.SetParent(
                board.transform,
                false
            );

            button.transform.localPosition =
                position;

            button.transform.localScale =
                new Vector3(
                    0.075f,
                    0.08f,
                    0.5f
                );

            Renderer renderer =
                button.GetComponent<Renderer>();

            if (renderer != null)
            {
                Material material =
                    CreateTransparentMaterial();

                material.color =
                    new Color(
                        0.1f,
                        0.9f,
                        0.2f,
                        0.9f
                    );

                renderer.material =
                    material;
            }

            RemoveCollider(button);

            return button;
        }

        // =====================================================
        // POINTER
        // =====================================================

        private void CreatePointer()
        {
            pointer =
                GameObject.CreatePrimitive(
                    PrimitiveType.Sphere
                );

            pointer.name =
                "ChillzWhitePointer";

            pointer.transform.localScale =
                Vector3.one * 0.025f;

            Renderer renderer =
                pointer.GetComponent<Renderer>();

            if (renderer != null)
            {
                Material material =
                    CreateTransparentMaterial();

                material.color =
                    Color.white;

                renderer.material =
                    material;
            }

            RemoveCollider(pointer);
        }

        // =====================================================
        // MATERIAL
        // =====================================================

        private Material CreateTransparentMaterial()
        {
            Shader shader =
                Shader.Find(
                    "Transparent/Diffuse"
                );

            if (shader == null)
            {
                shader =
                    Shader.Find(
                        "Unlit/Transparent"
                    );
            }

            if (shader == null)
            {
                shader =
                    Shader.Find(
                        "Sprites/Default"
                    );
            }

            return new Material(shader);
        }

        // =====================================================
        // REMOVE COLLIDER
        // =====================================================

        private void RemoveCollider(
            GameObject obj)
        {
            if (obj == null)
                return;

            Collider collider =
                obj.GetComponent<Collider>();

            if (collider != null)
                Destroy(collider);
        }

        // =====================================================
        // CLEANUP
        // =====================================================

        private void OnDestroy()
        {
            if (board != null)
                Destroy(board);

            if (pointer != null)
                Destroy(pointer);
        }
    }
}