using LIV.SDK.Unity;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using Valve.VR;

namespace OwLiv
{
    public class OwLivMod : ModBehaviour
    {
        public static LIV.SDK.Unity.LIV liv;
        private AssetBundle shaderBundle;
        private Camera previousCurrentCamera;
        
        private void Start()
        {
            shaderBundle = LoadBundle("liv-shaders");
            SDKShaders.LoadFromAssetBundle(shaderBundle);
            
            GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);
            LoadManager.OnCompleteSceneLoad += OnSceneLoaded;
            
            ModHelper.Events.Subscribe<PlayerCameraController>(Events.AfterStart);
            ModHelper.Events.Event += HandleEvent;
        }

        private void HandleEvent(MonoBehaviour behaviour, Events events)
        {
            if (behaviour.GetType() == typeof(PlayerCameraController) && events == Events.AfterStart)
            {
                SetUpLiv(Locator.GetPlayerCamera()._mainCamera);
            }
        }

        private void HandlePlayerAwake(PlayerBody playerBody)
        {
            SetUpLiv(Locator.GetPlayerCamera()._mainCamera);
        }

        private void OnSceneLoaded(OWScene originalscene, OWScene loadscene)
        {
            SetUpLiv(Camera.main);

            var flashback = FindObjectOfType<Flashback>();
            if (flashback)
            {
                flashback._maskEndDist = -2;
            }
        }

        private void OnSwitchActiveCamera(OWCamera activeCamera)
        {
            ModHelper.Console.WriteLine($"Switch active camera to ${activeCamera}");

            if (activeCamera.GetComponent<NomaiRemoteCamera>())
            {
                SetUpLivRemotNomaiCamera(activeCamera.mainCamera);
            }
            else if (activeCamera.GetComponent<Flashback>())
            {
                SetUpLivFlashback(activeCamera.mainCamera);
            }
            else
            {
                SetUpLiv(activeCamera.mainCamera);
            }
        }

        private void Update()
        {
            var currentCamera = Camera.main;

            if (!currentCamera) return;

            if (previousCurrentCamera != currentCamera)
            {
                previousCurrentCamera = currentCamera;
                SetUpLiv(currentCamera);
            }
            
            if (currentCamera.cullingMask != liv.spectatorLayerMask)
            {
            }
                liv.spectatorLayerMask = currentCamera.cullingMask;
            
            if (OWInput.IsNewlyPressed(InputLibrary.rollMode))
            {
                SetUpLiv(currentCamera);
            }
        }

        private void SetUpLivRemotNomaiCamera(Camera camera)
        {
            ModHelper.Console.WriteLine($"Setting up LIV with Remote NomaiCamera camera {camera.name}");
            
            if (liv)
            {
                ModHelper.Console.WriteLine($"LIV instance already exists. Destroying it.");
                Destroy(liv);
            }
            
            var cameraParent = camera.transform.parent.parent;

            var steamVrPose = cameraParent.GetComponentInChildren<SteamVR_Behaviour_Pose>();
            
            var stage = steamVrPose ? steamVrPose.transform.parent : cameraParent;

            var livCameraPrefabParent = new GameObject("LIVCameraPrefabParent").transform;
            var livCameraPrefab = new GameObject("LivCameraPrefab").AddComponent<Camera>();
            livCameraPrefab.transform.SetParent(livCameraPrefabParent, false);
            livCameraPrefabParent.gameObject.SetActive(false);

            liv = cameraParent.gameObject.AddComponent<LIV.SDK.Unity.LIV>();
            liv.stage = stage;
            liv.MRCameraPrefab = livCameraPrefab;
            liv.HMDCamera = camera;
            liv.fixPostEffectsAlpha = true;
            liv.spectatorLayerMask = camera.cullingMask;
            // liv.spectatorLayerMask &= ~(1 << LayerMask.NameToLayer("UI"));

            ModHelper.Console.WriteLine($"LIV created successfully with stage {stage}");
        }
        
        private void SetUpLivFlashback(Camera camera)
        {
            ModHelper.Console.WriteLine($"Setting up LIV with camera {camera.name}");
            
            if (liv)
            {
                ModHelper.Console.WriteLine($"LIV instance already exists. Destroying it.");
                Destroy(liv);
            }
            
            var cameraParent = new GameObject("LivFlashbackCameraParent").transform;
            cameraParent.SetParent(camera.transform.parent, false);
            cameraParent.position = new Vector3(camera.transform.position.x, -camera.transform.localPosition.y, camera.transform.position.z);

            var steamVrPose = cameraParent.GetComponentInChildren<SteamVR_Behaviour_Pose>();
            
            var stage = steamVrPose ? steamVrPose.transform.parent : cameraParent;

            liv = cameraParent.gameObject.AddComponent<LIV.SDK.Unity.LIV>();
            liv.stage = stage;
            liv.HMDCamera = camera;
            liv.fixPostEffectsAlpha = true;
            liv.spectatorLayerMask = camera.cullingMask;
            liv.spectatorLayerMask &= ~(1 << LayerMask.NameToLayer("UI"));
            liv.excludeBehaviours = new[]
            {
                "NomaiRemoteCamera",
                "AudioListener",
                "NomaiViewerImageEffect",
                "FlashbackScreenGrabImageEffect",
                "DebugHUD",
                "PlayerCameraController",
                "FirstPersonManipulator",
                "MindProjectorImageEffect",
                "RealityShatterImageEffect",
                "StreamingController",
                "VRMindProjectorImageEffect",
                "VRCameraManipulator",
                "ProximityDetector",
                "ViveFoveatedRendering",
                "Flashback",
                "FlashbackRecorder",
                "GameOverController",
                "LoadTimeTracker"
            };

            ModHelper.Console.WriteLine($"LIV created successfully with stage {stage}");
        }
        
        private void SetUpLiv(Camera camera)
        {
            ModHelper.Console.WriteLine($"Setting up LIV with camera {camera.name}");
            
            if (liv)
            {
                ModHelper.Console.WriteLine($"LIV instance already exists. Destroying it.");
                Destroy(liv.gameObject);
            }
            
            var cameraParent = camera.transform.parent;

            var steamVrPose = cameraParent.GetComponentInChildren<SteamVR_Behaviour_Pose>();
            
            var stage = steamVrPose ? steamVrPose.transform.parent : cameraParent;
            
            var livObject = new GameObject("LIV");
            livObject.gameObject.SetActive(false);
            livObject.transform.SetParent(cameraParent, false);

            liv = livObject.AddComponent<LIV.SDK.Unity.LIV>();
            liv.stage = stage;
            liv.HMDCamera = camera;
            liv.fixPostEffectsAlpha = true;
            liv.spectatorLayerMask = camera.cullingMask;
            liv.excludeBehaviours = new[]
            {
                "NomaiRemoteCamera",
                "AudioListener",
                "NomaiViewerImageEffect",
                "FlashbackScreenGrabImageEffect",
                "DebugHUD",
                "PlayerCameraController",
                "FirstPersonManipulator",
                "MindProjectorImageEffect",
                "RealityShatterImageEffect",
                "StreamingController",
                "VRMindProjectorImageEffect",
                "VRCameraManipulator",
                "ProximityDetector",
                "ViveFoveatedRendering",
                "Flashback",
                "FlashbackRecorder",
                "GameOverController",
                "LoadTimeTracker",
                "PostProcessingBehaviour"
            };

            ModHelper.Console.WriteLine($"LIV created successfully with stage {stage}");
            
            // TODO cleanup dont repeat
            livObject.gameObject.SetActive(true);
        }
        
        private AssetBundle LoadBundle(string assetName)
        {
            var bundle = AssetBundle.LoadFromFile($"{ModHelper.Manifest.ModFolderPath}/Assets/{assetName}");

            if (bundle == null)
            {
                ModHelper.Console.WriteLine($"Failed to load AssetBundle {assetName}", MessageType.Error);
            }

            return bundle;
        }
    }
}
