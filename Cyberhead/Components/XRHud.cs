using Reptile;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Cyberhead;

public class XRHud : MonoBehaviour {
    private Canvas canvas;
    private GameObject? cutsceneView;

    private void Awake() {
        this.canvas = this.GetComponent<Canvas>();
        this.canvas.renderMode = RenderMode.WorldSpace;
        this.transform.localScale = Vector3.one * 0.001f;
        this.gameObject.layer = Layers.Default;
    }

    private void Update() {
        if (Plugin.XRRig == null) return;
        var cameraTf = Plugin.XRRig.transform.Find("CameraOffset/XR Camera");
        if (cameraTf == null) return;
        var camera = cameraTf.GetComponent<XRCamera>();
        if (camera == null) return;
        var hudBone = Plugin.XRRig.transform.Find("CameraOffset/XR Camera/XR HUD");
        if (hudBone == null) return;

        var worldHandler = WorldHandler.instance;
        if (worldHandler == null) return;
        var player = worldHandler.GetCurrentPlayer();
        if (player == null) {
            this.DoTheater(camera);
            return;
        }

        var gameplayCam = player.cam;
        if (gameplayCam == null || !gameplayCam.enabled || !gameplayCam.cam.enabled) {
            this.DoTheater(camera);
            return;
        }

        camera.LockedPos = null;
        if (this.cutsceneView != null) {
            Destroy(this.cutsceneView);
            this.transform.localScale = Vector3.one * 0.001f;
        }

        this.transform.position = hudBone.position;
        this.transform.rotation = hudBone.rotation;

        Core.Instance.UIManager.effects.gameObject.SetActive(false);
    }

    private void DoTheater(XRCamera cam) {
        var lockPos = Vector3.zero;
        cam.LockedPos = lockPos;

        if (this.cutsceneView == null) {
            this.transform.localScale = Vector3.one;

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var offset = cam.transform.parent.parent.gameObject.GetComponent<XROrigin>().CameraYOffset;
            quad.transform.position = lockPos + new Vector3(0, offset, 2);

            var rt = Plugin.CutsceneRenderTexture;
            var height = 2f;
            var width = height * rt.width / rt.height;
            quad.transform.localScale = new Vector3(width, height, 1);

            var mat = new Material(Shader.Find("Unlit/Texture"));
            mat.mainTexture = rt;
            quad.GetComponent<MeshRenderer>().material = mat;

            this.cutsceneView = quad;
        }

        this.transform.position = this.cutsceneView.transform.position - new Vector3(0, 0, 0.1f);
        this.transform.rotation = Quaternion.identity;

        var scale = this.cutsceneView.transform.localScale;
        var hudWidth = (float) Screen.width;
        var hudHeight = (float) Screen.height;
        // Scale appropriately to fit in the cutscene view
        this.transform.localScale = new Vector3(
            scale.x / hudWidth,
            scale.y / hudHeight,
            1
        );
    }
}
