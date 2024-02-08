using Reptile;
using UnityEngine;

namespace Cyberhead;

public class XRHud : MonoBehaviour {
    private Canvas canvas;

    private void Awake() {
        this.canvas = this.GetComponent<Canvas>();
        this.canvas.renderMode = RenderMode.WorldSpace;
        this.transform.localScale = Vector3.one * 0.001f;
        this.gameObject.layer = Layers.Default;
    }

    private void Update() {
        var worldHandler = WorldHandler.instance;
        if (worldHandler == null) return;
        var player = worldHandler.GetCurrentPlayer();
        if (player == null) return;

        var bone = Plugin.XRRig?.transform.Find("CameraOffset/XR Camera/XR HUD");
        if (bone == null) return;
        this.transform.position = bone.position;
        this.transform.rotation = bone.rotation;

        Core.Instance.UIManager.effects.gameObject.SetActive(false);
    }
}
