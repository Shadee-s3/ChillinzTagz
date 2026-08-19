using System.Collections.Generic;
using UnityEngine;

namespace Granalax.Mods
{
    public class HoloTag : MonoBehaviour
    {
        private class TagData
        {
            public VRRig Rig;
            public GameObject Root;

            public TextMesh HzText;
            public TextMesh PlatformText;
            public TextMesh NameText;
            public TextMesh DecorationText;
            public TextMesh StatusText;

            public GameObject ModBox;
            public TextMesh ModText;

            public string LastName = "";
            public string LastPlatform = "";
            public string LastMods = "";

            public int UpdateCount;
            public float HzTimer;

            public Vector3 LastHeadPosition;
            public Quaternion LastHeadRotation;
            public bool HasPreviousTransform;
        }

        private readonly Dictionary<VRRig, TagData> tags =
            new Dictionary<VRRig, TagData>();

        private float scanTimer;
        private float textTimer;

        private const float ScanInterval = 1f;
        private const float TextInterval = 0.25f;
        private const float HzInterval = 0.1f;

        private readonly Color Violet =
            new Color(0.72f, 0.25f, 1f, 1f);

        private readonly Color BrightViolet =
            new Color(0.9f, 0.55f, 1f, 1f);

        private void Start()
        {
            ScanPlayers();
        }

        private void Update()
        {
            scanTimer -= Time.deltaTime;
            textTimer -= Time.deltaTime;

            if (scanTimer <= 0f)
            {
                scanTimer = ScanInterval;
                ScanPlayers();
            }

            UpdatePositions();
            UpdateHz();

            if (textTimer <= 0f)
            {
                textTimer = TextInterval;
                UpdateText();
            }
        }

        // =========================================================
        // PLAYERS
        // =========================================================

        private void ScanPlayers()
        {
            VRRig[] rigs =
                Object.FindObjectsOfType<VRRig>();

            HashSet<VRRig> current =
                new HashSet<VRRig>();

            foreach (VRRig rig in rigs)
            {
                if (rig == null)
                    continue;

                if (GorillaTagger.Instance != null &&
                    rig ==
                    GorillaTagger.Instance.offlineVRRig)
                {
                    continue;
                }

                current.Add(rig);

                if (!tags.ContainsKey(rig))
                    CreateTag(rig);
            }

            List<VRRig> remove =
                new List<VRRig>();

            foreach (KeyValuePair<VRRig, TagData> pair
                in tags)
            {
                if (!current.Contains(pair.Key))
                {
                    if (pair.Value.Root != null)
                        Destroy(pair.Value.Root);

                    remove.Add(pair.Key);
                }
            }

            foreach (VRRig rig in remove)
                tags.Remove(rig);
        }

        // =========================================================
        // CREATE
        // =========================================================

        private void CreateTag(VRRig rig)
        {
            GameObject root =
                new GameObject("Granalax_HoloTag");

            TagData data =
                new TagData();

            data.Rig = rig;
            data.Root = root;

            data.HzText = CreateText(
                root,
                "0 Hz",
                0.011f,
                BrightViolet,
                48);

            data.HzText.transform.localPosition =
                new Vector3(0f, 0.24f, 0f);

            data.PlatformText = CreateText(
                root,
                "[ UNKNOWN ]",
                0.014f,
                BrightViolet,
                58);

            data.PlatformText.transform.localPosition =
                new Vector3(0f, 0.15f, 0f);

            data.NameText = CreateText(
                root,
                "< Player >",
                0.025f,
                Color.white,
                72);

            data.NameText.transform.localPosition =
                new Vector3(0f, 0.055f, 0f);

            data.DecorationText = CreateText(
                root,
                "--- • ---",
                0.012f,
                Violet,
                52);

            data.DecorationText.transform.localPosition =
                new Vector3(0f, -0.035f, 0f);

            data.StatusText = CreateText(
                root,
                "● UNKNOWN",
                0.012f,
                ModListDetection.GetStatusColor(
                    ModListDetection.ModStatus.Unknown),
                52);

            data.StatusText.transform.localPosition =
                new Vector3(0f, -0.095f, 0f);

            CreateModBox(data);

            Transform head =
                GetHead(rig);

            if (head != null)
            {
                data.LastHeadPosition =
                    head.position;

                data.LastHeadRotation =
                    head.rotation;

                data.HasPreviousTransform = true;
            }

            tags.Add(rig, data);
        }

        // =========================================================
        // TEXT
        // =========================================================

        private TextMesh CreateText(
            GameObject parent,
            string text,
            float characterSize,
            Color color,
            int fontSize)
        {
            GameObject obj =
                new GameObject("TagText");

            obj.transform.SetParent(
                parent.transform,
                false);

            TextMesh mesh =
                obj.AddComponent<TextMesh>();

            mesh.text = text;
            mesh.fontSize = fontSize;
            mesh.characterSize = characterSize;

            mesh.anchor =
                TextAnchor.MiddleCenter;

            mesh.alignment =
                TextAlignment.Center;

            mesh.fontStyle =
                FontStyle.Bold;

            mesh.color = color;

            return mesh;
        }

        // =========================================================
        // MOD BOX
        // =========================================================

        private void CreateModBox(TagData data)
        {
            data.ModBox =
                new GameObject("DetectedModsBox");

            data.ModBox.transform.SetParent(
                data.Root.transform,
                false);

            data.ModBox.transform.localPosition =
                new Vector3(0.55f, 0f, 0f);

            data.ModBox.transform.localScale =
                new Vector3(0.35f, 0.25f, 0.01f);

            SpriteRenderer renderer =
                data.ModBox.AddComponent<SpriteRenderer>();

            renderer.sprite =
                CreateSquareSprite();

            renderer.color =
                new Color(
                    0.32f,
                    0.18f,
                    0.08f,
                    0.95f);

            data.ModText = CreateText(
                data.ModBox,
                "",
                0.012f,
                new Color(
                    1f,
                    0.82f,
                    0.55f,
                    1f),
                45);

            data.ModText.transform.localPosition =
                new Vector3(0f, 0f, -0.01f);

            data.ModBox.SetActive(false);
        }

        private Sprite CreateSquareSprite()
        {
            Texture2D texture =
                new Texture2D(2, 2);

            texture.SetPixel(0, 0, Color.white);
            texture.SetPixel(1, 0, Color.white);
            texture.SetPixel(0, 1, Color.white);
            texture.SetPixel(1, 1, Color.white);

            texture.Apply();

            return Sprite.Create(
                texture,
                new Rect(0, 0, 2, 2),
                new Vector2(0.5f, 0.5f));
        }

        // =========================================================
        // TEXT UPDATE
        // =========================================================

        private void UpdateText()
        {
            foreach (TagData data in tags.Values)
            {
                if (data.Rig == null)
                    continue;

                string name =
                    GetPlayerName(data.Rig);

                string platform =
                    PlatformDetector.GetPlatformName(
                        data.Rig);

                if (name != data.LastName)
                {
                    data.LastName = name;

                    data.NameText.text =
                        "< " + name + " >";
                }

                if (platform != data.LastPlatform)
                {
                    data.LastPlatform = platform;

                    data.PlatformText.text =
                        "[ " + platform + " ]";
                }

                UpdateStatus(data);
            }
        }

        // =========================================================
        // STATUS
        // =========================================================

        private void UpdateStatus(TagData data)
        {
            List<string> mods =
                GetDetectedMods(data.Rig);

            /*
             * Nothing detected does NOT mean LEGIT.
             */
            if (mods == null ||
                mods.Count == 0)
            {
                SetStatus(
                    data,
                    ModListDetection.ModStatus.Unknown);

                HideModBox(data);
                return;
            }

            ModListDetection.ModStatus status =
                ModListDetection.CheckMods(mods);

            SetStatus(data, status);
            UpdateModBox(data, mods);

            /*
             * Send the same information to CheaterAlert
             * if one exists in the scene.
             */
            CheaterAlert alert =
                FindObjectOfType<CheaterAlert>();

            if (alert != null)
            {
                alert.CheckPlayer(
                    data.Rig,
                    mods);
            }
        }

        private void SetStatus(
            TagData data,
            ModListDetection.ModStatus status)
        {
            if (data.StatusText == null)
                return;

            data.StatusText.text =
                "● " +
                ModListDetection.GetStatusText(
                    status);

            data.StatusText.color =
                ModListDetection.GetStatusColor(
                    status);
        }

        // =========================================================
        // MOD BOX
        // =========================================================

        private void UpdateModBox(
            TagData data,
            List<string> mods)
        {
            if (data.ModBox == null)
                return;

            string combined = "";

            foreach (string mod in mods)
            {
                if (string.IsNullOrWhiteSpace(mod))
                    continue;

                ModListDetection.ModStatus status =
                    ModListDetection.CheckMod(mod);

                if (combined.Length > 0)
                    combined += "\n";

                combined +=
                    ModListDetection.GetStatusText(status) +
                    ": " +
                    mod;
            }

            if (string.IsNullOrEmpty(combined))
            {
                HideModBox(data);
                return;
            }

            if (combined != data.LastMods)
            {
                data.LastMods = combined;

                if (data.ModText != null)
                    data.ModText.text = combined;
            }

            data.ModBox.SetActive(true);
        }

        private void HideModBox(TagData data)
        {
            data.LastMods = "";

            if (data.ModBox != null)
                data.ModBox.SetActive(false);
        }

        // =========================================================
        // MOD DATA
        // =========================================================

        private List<string> GetDetectedMods(VRRig rig)
        {
            /*
             * This is the only part that still needs a real
             * network/shared mod-data source.
             *
             * Do not return LEGIT here just because nothing
             * was found.
             */

            return null;
        }

        // =========================================================
        // NAME
        // =========================================================

        private string GetPlayerName(VRRig rig)
        {
            try
            {
                if (rig.OwningNetPlayer != null)
                {
                    string name =
                        rig.OwningNetPlayer.NickName;

                    if (!string.IsNullOrWhiteSpace(name))
                        return name;
                }
            }
            catch
            {
            }

            return "UNKNOWN";
        }

        // =========================================================
        // HEAD
        // =========================================================

        private Transform GetHead(VRRig rig)
        {
            if (rig == null)
                return null;

            if (rig.headMesh != null)
                return rig.headMesh.transform;

            return rig.transform;
        }

        // =========================================================
        // HZ
        // =========================================================

        private void UpdateHz()
        {
            foreach (TagData data in tags.Values)
            {
                if (data.Rig == null)
                    continue;

                Transform head =
                    GetHead(data.Rig);

                if (head == null)
                    continue;

                data.HzTimer += Time.deltaTime;

                bool changed = false;

                if (!data.HasPreviousTransform)
                {
                    changed = true;
                    data.HasPreviousTransform = true;
                }
                else
                {
                    float positionDifference =
                        Vector3.Distance(
                            head.position,
                            data.LastHeadPosition);

                    float rotationDifference =
                        Quaternion.Angle(
                            head.rotation,
                            data.LastHeadRotation);

                    if (positionDifference > 0.0005f ||
                        rotationDifference > 0.1f)
                    {
                        changed = true;
                    }
                }

                if (changed)
                {
                    data.UpdateCount++;

                    data.LastHeadPosition =
                        head.position;

                    data.LastHeadRotation =
                        head.rotation;
                }

                if (data.HzTimer >= HzInterval)
                {
                    float hz =
                        data.UpdateCount /
                        data.HzTimer;

                    data.UpdateCount = 0;
                    data.HzTimer = 0f;

                    if (data.HzText != null)
                    {
                        data.HzText.text =
                            Mathf.RoundToInt(hz) +
                            " Hz";
                    }
                }
            }
        }

        // =========================================================
        // POSITION
        // =========================================================

        private void UpdatePositions()
        {
            Camera camera = Camera.main;

            if (camera == null)
                return;

            foreach (TagData data in tags.Values)
            {
                if (data.Rig == null ||
                    data.Root == null)
                    continue;

                Transform head =
                    GetHead(data.Rig);

                if (head != null)
                {
                    data.Root.transform.position =
                        head.position +
                        Vector3.up * 0.30f;
                }
                else
                {
                    data.Root.transform.position =
                        data.Rig.transform.position +
                        Vector3.up;
                }

                data.Root.transform.LookAt(
                    camera.transform);

                data.Root.transform.Rotate(
                    0f,
                    180f,
                    0f);
            }
        }

        // =========================================================
        // CLEANUP
        // =========================================================

        private void OnDestroy()
        {
            foreach (TagData data in tags.Values)
            {
                if (data.Root != null)
                    Destroy(data.Root);
            }

            tags.Clear();
        }
    }
}