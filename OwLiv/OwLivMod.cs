using LIV.SDK.Unity;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using Valve.VR;

namespace OwLiv
{
    public class OwLivMod : ModBehaviour
    {
        private LIV.SDK.Unity.LIV liv;
        private Camera cameraPrefab;
        private AssetBundle shaderBundle;
        private Camera previousCurrentCamera;
        private const string flashbackCameraParentName = "LivFlashbackCameraParent";
        
        private void Start()
        {
            shaderBundle = LoadBundle("liv-shaders");
            SDKShaders.LoadFromAssetBundle(shaderBundle);
            
            GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);
            LoadManager.OnCompleteSceneLoad += OnSceneLoaded;
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

            var camera = activeCamera.mainCamera;
            
            if (activeCamera.GetComponent<NomaiRemoteCamera>())
            {
                SetUpLivNomaiRemoteCamera(camera);
            }
            else if (activeCamera.GetComponent<Flashback>())
            {
                SetUpLivFlashback(camera);
            }
            else
            {
                SetUpLiv(camera);
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
                liv.spectatorLayerMask = currentCamera.cullingMask;
            }
            
            if (OWInput.IsNewlyPressed(InputLibrary.rollMode))
            {
                SetUpLiv(currentCamera);
            }
        }

        private void SetUpLivNomaiRemoteCamera(Camera camera)
        {
            if (!cameraPrefab)
            {
                var livCameraPrefabParent = new GameObject("LIVCameraPrefabParent").transform;
                cameraPrefab = new GameObject("LivCameraPrefab").AddComponent<Camera>();
                cameraPrefab.transform.SetParent(livCameraPrefabParent, false);
                livCameraPrefabParent.gameObject.SetActive(false);
            }
            SetUpLiv(camera, camera.transform.parent.parent, true);
        }
        
        private void SetUpLivFlashback(Camera camera)
        {
            var cameraParent = camera.transform.parent.Find(flashbackCameraParentName);
            if (!cameraParent)
            {
                cameraParent = new GameObject("LivFlashbackCameraParent").transform;
                cameraParent.SetParent(camera.transform.parent, false);
                cameraParent.position = new Vector3(camera.transform.position.x, -camera.transform.localPosition.y, camera.transform.position.z);
            }

            SetUpLiv(camera, cameraParent);
        }
        
        private void SetUpLiv(Camera camera, Transform parent = null, bool usePrefab = false)
        {
            ModHelper.Console.WriteLine($"Setting up LIV with camera {camera.name}");
            
            if (liv)
            {
                ModHelper.Console.WriteLine($"LIV instance already exists. Destroying it.");
                Destroy(liv.gameObject);
            }
            
            var cameraParent = parent ? parent : camera.transform.parent;

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
            if (usePrefab)
            {
                liv.MRCameraPrefab = cameraPrefab;
            }

            ModHelper.Console.WriteLine($"LIV created successfully with stage {stage}");
            
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
