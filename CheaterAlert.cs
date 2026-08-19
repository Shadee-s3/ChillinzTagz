using System.Collections.Generic;
using UnityEngine;

namespace Granalax.Mods
{
    public class CheaterAlert : MonoBehaviour
    {
        private GameObject alertObject;
        private TextMesh alertText;

        private float alertTimer;

        private const float AlertDuration = 5f;

        private readonly HashSet<string> alertedPlayers =
            new HashSet<string>();

        private void Start()
        {
            CreateAlert();

            if (alertObject != null)
                alertObject.SetActive(false);
        }

        private void Update()
        {
            if (alertObject == null)
                return;

            if (!alertObject.activeSelf)
                return;

            alertTimer -= Time.deltaTime;

            if (alertTimer <= 0f)
            {
                alertObject.SetActive(false);
                alertTimer = 0f;
            }
        }

        public void CheckPlayer(
            VRRig rig,
            List<string> detectedMods)
        {
            if (rig == null ||
                detectedMods == null ||
                detectedMods.Count == 0)
                return;

            string playerName =
                GetPlayerName(rig);

            if (string.IsNullOrEmpty(playerName))
                playerName = "UNKNOWN";

            foreach (string mod in detectedMods)
            {
                if (string.IsNullOrEmpty(mod))
                    continue;

                if (!ModListDetection.IsIllegal(mod))
                    continue;

                // Don't repeatedly alert for the same player.
                if (alertedPlayers.Contains(playerName))
                    return;

                alertedPlayers.Add(playerName);

                ShowAlert(
                    playerName,
                    mod
                );

                return;
            }
        }

        private void ShowAlert(
            string playerName,
            string dllName)
        {
            if (alertObject == null ||
                alertText == null)
                return;

            alertText.text =
                "ALERT\n" +
                playerName +
                "\nIS CHEATING!\n\n" +
                dllName;

            alertObject.SetActive(true);

            alertTimer = AlertDuration;
        }

        private void CreateAlert()
        {
            alertObject =
                new GameObject(
                    "GranalaxCheaterAlert"
                );

            Camera camera =
                Camera.main;

            if (camera != null)
            {
                alertObject.transform.SetParent(
                    camera.transform,
                    false
                );

                alertObject.transform.localPosition =
                    new Vector3(
                        0f,
                        0.15f,
                        0.7f
                    );

                alertObject.transform.localRotation =
                    Quaternion.identity;
            }

            alertText =
                alertObject.AddComponent<TextMesh>();

            alertText.anchor =
                TextAnchor.MiddleCenter;

            alertText.alignment =
                TextAlignment.Center;

            alertText.fontSize = 70;
            alertText.characterSize = 0.025f;

            alertText.fontStyle =
                FontStyle.Bold;

            alertText.color =
                new Color(
                    1f,
                    0.15f,
                    0.05f,
                    1f
                );
        }

        private string GetPlayerName(VRRig rig)
        {
            try
            {
                if (rig.OwningNetPlayer != null)
                {
                    return rig.OwningNetPlayer.NickName;
                }
            }
            catch
            {
            }

            return "UNKNOWN";
        }

        public void ResetPlayer(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
                return;

            alertedPlayers.Remove(playerName);
        }

        private void OnDestroy()
        {
            if (alertObject != null)
                Destroy(alertObject);

            alertedPlayers.Clear();
        }
    }
}