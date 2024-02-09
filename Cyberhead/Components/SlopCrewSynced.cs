using UnityEngine;

namespace Cyberhead;

public class SlopCrewSynced : MonoBehaviour {
    public Vector3 SyncedPosition;
    public Quaternion SyncedRotation;

    private void LateUpdate() {
        this.transform.position = this.SyncedPosition;
        this.transform.rotation = this.SyncedRotation;
    }
}
