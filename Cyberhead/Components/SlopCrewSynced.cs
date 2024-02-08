using UnityEngine;

namespace Cyberhead;

public class SlopCrewSynced : MonoBehaviour {
    public Vector3 SyncedPosition;
    public Quaternion SyncedRotation;

    private void Update() {
        this.transform.position = this.SyncedPosition;
        this.transform.rotation = this.SyncedRotation;
    }
}
