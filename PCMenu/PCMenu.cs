using UnityEngine;
using UnityEngine.UI;

namespace ChillzMenu
{
    public class PCMenu : MonoBehaviour
    {
        public static PCMenu Instance;

        private Canvas canvas;
        private Image background;

        private bool microphoneMuted;
        private bool fpsEnabled;

        private Color currentColor =
            new Color(
                0.05f,
                1f,
                0.15f,
                0.45f
            );

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            CreateCanvas();

            if (canvas == null)
                return;

            CreateBackground();
            CreateTitle();
            CreateButtons();
        }

        private void CreateCanvas()
        {
            Camera camera =
                Camera.main;

            if (camera == null)
            {
                Debug.LogError(
                    "[Chillz] Camera not found."
                );

                return;
            }

            GameObject canvasObject =
                new GameObject(
                    "ChillzCanvas"
                );

            canvas =
                canvasObject.AddComponent<Canvas>();

            canvas.renderMode =
                RenderMode.ScreenSpaceCamera;

            canvas.worldCamera =
                camera;

            canvas.planeDistance =
                0.5f;

            CanvasScaler scaler =
                canvasObject.AddComponent<CanvasScaler>();

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(
                    1920f,
                    1080f
                );

            canvasObject.AddComponent<
                GraphicRaycaster
            >();

            Debug.Log(
                "[Chillz] Canvas created."
            );
        }

        private void CreateBackground()
        {
            GameObject objectObject =
                new GameObject(
                    "ChillzBackground"
                );

            objectObject.transform.SetParent(
                canvas.transform,
                false
            );

            background =
                objectObject.AddComponent<Image>();

            background.color =
                currentColor;

            RectTransform rect =
                objectObject.GetComponent<
                    RectTransform
                >();

            rect.anchorMin =
                new Vector2(1f, 1f);

            rect.anchorMax =
                new Vector2(1f, 1f);

            rect.pivot =
                new Vector2(1f, 1f);

            rect.anchoredPosition =
                new Vector2(
                    -20f,
                    -20f
                );

            rect.sizeDelta =
                new Vector2(
                    320f,
                    230f
                );
        }

        private void CreateTitle()
        {
            GameObject objectObject =
                new GameObject(
                    "ChillzTitle"
                );

            objectObject.transform.SetParent(
                background.transform,
                false
            );

            Text text =
                objectObject.AddComponent<Text>();

            text.text =
                "CHILLZ BASIC MENU";

            text.fontSize = 22;

            text.fontStyle =
                FontStyle.Bold;

            text.alignment =
                TextAnchor.MiddleCenter;

            text.color =
                Color.white;

            RectTransform rect =
                objectObject.GetComponent<
                    RectTransform
                >();

            rect.anchorMin =
                new Vector2(0f, 1f);

            rect.anchorMax =
                new Vector2(1f, 1f);

            rect.pivot =
                new Vector2(
                    0.5f,
                    1f
                );

            rect.anchoredPosition =
                new Vector2(
                    0f,
                    -10f
                );

            rect.sizeDelta =
                new Vector2(
                    -20f,
                    40f
                );
        }

        private void CreateButtons()
        {
            CreateButton(
                "MIC",
                0
            );

            CreateButton(
                "SETTINGS",
                1
            );

            CreateButton(
                "FPS",
                2
            );

            CreateButton(
                "INFO",
                3
            );
        }

        private void CreateButton(
            string buttonName,
            int index)
        {
            GameObject objectObject =
                new GameObject(
                    "ChillzButton_" +
                    buttonName
                );

            objectObject.transform.SetParent(
                background.transform,
                false
            );

            Image image =
                objectObject.AddComponent<Image>();

            image.color =
                new Color(
                    currentColor.r,
                    currentColor.g,
                    currentColor.b,
                    0.8f
                );

            Button button =
                objectObject.AddComponent<Button>();

            string capturedName =
                buttonName;

            button.onClick.AddListener(
                delegate
                {
                    ButtonPressed(
                        capturedName
                    );
                }
            );

            GameObject textObject =
                new GameObject(
                    "ButtonText"
                );

            textObject.transform.SetParent(
                objectObject.transform,
                false
            );

            Text text =
                textObject.AddComponent<Text>();

            text.text =
                buttonName;

            text.fontSize = 16;

            text.fontStyle =
                FontStyle.Bold;

            text.alignment =
                TextAnchor.MiddleCenter;

            text.color =
                Color.white;

            RectTransform textRect =
                textObject.GetComponent<
                    RectTransform
                >();

            textRect.anchorMin =
                Vector2.zero;

            textRect.anchorMax =
                Vector2.one;

            textRect.offsetMin =
                Vector2.zero;

            textRect.offsetMax =
                Vector2.zero;

            RectTransform rect =
                objectObject.GetComponent<
                    RectTransform
                >();

            rect.anchorMin =
                new Vector2(
                    0f,
                    1f
                );

            rect.anchorMax =
                new Vector2(
                    0f,
                    1f
                );

            rect.pivot =
                new Vector2(
                    0f,
                    1f
                );

            float x =
                index % 2 == 0
                    ? 15f
                    : 165f;

            float y =
                index < 2
                    ? -65f
                    : -125f;

            rect.anchoredPosition =
                new Vector2(
                    x,
                    y
                );

            rect.sizeDelta =
                new Vector2(
                    140f,
                    45f
                );
        }

        private void ButtonPressed(
            string buttonName)
        {
            if (buttonName == "MIC")
            {
                microphoneMuted =
                    !microphoneMuted;

                Debug.Log(
                    "[Chillz] Mic: " +
                    (
                        microphoneMuted
                            ? "MUTED"
                            : "UNMUTED"
                    )
                );

                return;
            }

            if (buttonName == "SETTINGS")
            {
                currentColor =
                    new Color(
                        0.1f,
                        0.6f,
                        1f,
                        0.45f
                    );

                if (background != null)
                    background.color =
                        currentColor;

                return;
            }

            if (buttonName == "FPS")
            {
                fpsEnabled =
                    !fpsEnabled;

                Debug.Log(
                    "[Chillz] FPS: " +
                    (
                        fpsEnabled
                            ? "ON"
                            : "OFF"
                    )
                );

                return;
            }

            if (buttonName == "INFO")
            {
                Debug.Log(
                    "[Chillz] Chillz Basic Menu"
                );
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            if (canvas != null)
                Destroy(
                    canvas.gameObject
                );
        }
    }
}