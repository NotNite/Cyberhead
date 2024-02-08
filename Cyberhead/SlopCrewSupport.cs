using System;
using System.IO;
using System.Timers;
using Reptile;
using SlopCrew.API;
using UnityEngine;

namespace Cyberhead;

public class SlopCrewSupport : IDisposable {
    private ISlopCrewAPI? api;
    private Timer? timer;

    public SlopCrewSupport() {
        var api = APIManager.API;
        if (api is not null) {
            this.InitApi(api);
        } else {
            APIManager.OnAPIRegistered += this.InitApi;
        }
    }

    private void InitApi(ISlopCrewAPI api) {
        this.api = api;
        this.api.OnCustomPacketReceived += this.OnCustomPacketReceived;

        this.timer = new Timer(1000f / 5f);
        this.timer.Elapsed += this.SendVrIkPacket;
        this.timer.Start();
    }

    public void Dispose() {
        if (this.api is not null) {
            this.api.OnCustomPacketReceived -= this.OnCustomPacketReceived;
        }

        this.timer?.Stop();
        this.timer?.Dispose();
    }

    private void SendVrIkPacket(object? sender, ElapsedEventArgs e) {
        if (Plugin.XRRig is null) return;

        var handL = Plugin.XRRig.transform.Find("CameraOffset/XR Hand L/IK");
        var handR = Plugin.XRRig.transform.Find("CameraOffset/XR Hand R/IK");
        var packet = new VrIkPacket {
            LeftHandPos = handL.position,
            LeftHandRot = handL.rotation,
            RightHandPos = handR.position,
            RightHandRot = handR.rotation
        };

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        packet.Write(writer);

        this.api?.SendCustomPacket("Cyberhead.VrIK", stream.ToArray());
    }

    private void OnCustomPacketReceived(uint player, string id, byte[] data) {
        if (id != "Cyberhead.VrIK") return;

        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);
        var packet = new VrIkPacket();
        try {
            packet.Read(reader);
        } catch {
            return;
        }

        var playerPath = this.api?.GetGameObjectPathForPlayerID(player);
        if (playerPath is null) return;

        var playerObj = GameObject.Find(playerPath);
        if (playerObj is null) return;

        var playerController = playerObj.GetComponent<Player>();
        var handL = playerController.characterVisual.handL;
        var handR = playerController.characterVisual.handR;

        var ikL = handL.transform.Find("IK");
        if (ikL == null) {
            ikL = new GameObject("IK").transform;
            ikL.SetParent(handL.transform, false);
            ikL.gameObject.AddComponent<SlopCrewSynced>();
        }

        var ikR = handR.transform.Find("IK");
        if (ikR == null) {
            ikR = new GameObject("IK").transform;
            ikR.SetParent(handR.transform, false);
            ikR.gameObject.AddComponent<SlopCrewSynced>();
        }

        playerController.characterVisual.handIKActiveL = true;
        playerController.characterVisual.handIKActiveR = true;
        playerController.characterVisual.handIKTargetL = ikL;
        playerController.characterVisual.handIKTargetR = ikR;

        var syncedL = ikL.GetComponent<SlopCrewSynced>();
        syncedL.SyncedPosition = packet.LeftHandPos;
        syncedL.SyncedRotation = packet.LeftHandRot;

        var syncedR = ikR.GetComponent<SlopCrewSynced>();
        syncedR.SyncedPosition = packet.RightHandPos;
        syncedR.SyncedRotation = packet.RightHandRot;
    }

    public class VrIkPacket {
        public Vector3 LeftHandPos = Vector3.zero;
        public Quaternion LeftHandRot = Quaternion.identity;
        public Vector3 RightHandPos = Vector3.zero;
        public Quaternion RightHandRot = Quaternion.identity;

        public void Read(BinaryReader reader) {
            this.LeftHandPos = ReadVector3(reader);
            this.LeftHandRot = ReadQuaternion(reader);
            this.RightHandPos = ReadVector3(reader);
            this.RightHandRot = ReadQuaternion(reader);
        }

        public void Write(BinaryWriter writer) {
            WriteVector3(writer, this.LeftHandPos);
            WriteQuaternion(writer, this.LeftHandRot);
            WriteVector3(writer, this.RightHandPos);
            WriteQuaternion(writer, this.RightHandRot);
        }

        private static Vector3 ReadVector3(BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        private static void WriteVector3(BinaryWriter writer, Vector3 vec) {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }

        private static Quaternion ReadQuaternion(BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();
            return new Quaternion(x, y, z, w);
        }

        private static void WriteQuaternion(BinaryWriter writer, Quaternion quat) {
            writer.Write(quat.x);
            writer.Write(quat.y);
            writer.Write(quat.z);
            writer.Write(quat.w);
        }
    }
}
