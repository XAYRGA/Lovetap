using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.InteropServices;

namespace LovetapNF
{
    public static class Interface
    {
        static ColliderData[] colliderBaseList = new ColliderData[0];
        static string[] colliderNames = new string[0];
        static string[] colliderWatchlistNames = new string[0];

        static int btSelIndex = 0;
        static int cwlSelIndex = 0;


        public static void refreshColliderList()
        {
            colliderBaseList = ColliderCon.getColliders();
            if (colliderBaseList == null)
            {
                colliderBaseList = new ColliderData[0];
                colliderNames = new string[0];
            }
            colliderNames = new string[colliderBaseList.Length];
            var i = 0; 
            foreach (var coll in colliderBaseList)
            {
                colliderNames[i] = coll.name;
                i++;
            }
        }

        public static void refreshWatchedColliders()
        {
            colliderWatchlistNames = new string[CollideSystem.colliders.Count];
            var i = 0; 
            foreach (var ccol in CollideSystem.colliders)
            {
                colliderWatchlistNames[i] = ccol.first + "/" + ccol.second;
                i++;
            }
        }

        static int c1sel = 0;
        static int c2sel = 0;

        public static void submit()
        {

            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(340, 200));
            ImGui.Begin("Bluetooth Control");
            {
                if (BTManager.selectedDevice == null)
                {
                    var DrawList = ImGui.GetWindowDrawList();
                    ImGui.ListBox("Paired Devices", ref btSelIndex, BTManager.pairdDevicesNames, BTManager.pairdDevicesNames.Length);
                    if (ImGui.Button("Rescan Devices"))
                    {
                        BTManager.rescanDevices();
                    }
                    if (!BTManager.connecting)
                    {
                        if (ImGui.Button("Use Device"))
                        {
                            BTManager.connectDevice(BTManager.pairedDevices[btSelIndex]);
                        }
                    } else
                    {
                        ImGui.Text("Connecting....");
                    }

                    if (BTManager.scanning)
                    {
                        ImGui.LabelText("Scanning...", "");
                    }
                } else
                {
                    ImGui.Text($"Using Device: {BTManager.selectedDevice.Name}");
                }

            }
            ImGui.End();

            ImGui.SetNextWindowPos(new Vector2(340, 0));
            ImGui.SetNextWindowSize(new Vector2(200, 200));
            ImGui.Begin("VRC Collider Detection");
            if (!ColliderCon.ready)
            {
                ImGui.Text("Not connected.");
                if (ImGui.Button("Connect to VRC"))
                    ColliderCon.connect();
                ImGui.Text(ColliderCon.error);

            } else
            {
                ImGui.Text("Connected to collider system.");
            }
            ImGui.End();
            ImGui.SetNextWindowPos(new Vector2(0, 200));
            ImGui.SetNextWindowSize(new Vector2(540, 400));
            if (ColliderCon.ready)
            {
                ImGui.Begin("Add Collider Check");
                if (ImGui.Button("Refresh Colliders"))
                {
                    refreshColliderList();
                }
                ImGui.Combo("First Collider", ref c1sel, colliderNames, colliderNames.Length);
                ImGui.Combo("Second Collider", ref c2sel, colliderNames, colliderNames.Length);
                if (ImGui.Button("Add Collider Check"))
                {
                    var c1 = CollideSystem.getColliderByName(colliderBaseList[c1sel].name);
                    var c2 = CollideSystem.getColliderByName(colliderBaseList[c2sel].name);
                    if (c1 == null || c2 == null)
                        return;

                    CollideSystem.colliders.Add(new WatchedCollider(c1,c2));
                    refreshWatchedColliders();
                }

                ImGui.ListBox("Watched Colliders", ref cwlSelIndex, colliderWatchlistNames, colliderWatchlistNames.Length);
                if (ImGui.Button("Remove collider check"))
                {
                    CollideSystem.colliders.RemoveAt(cwlSelIndex);
                    refreshWatchedColliders();
                
                }
                ImGui.Text($"Current Toy Status: {BTManager.plugIntensity}");
                ImGui.End();
            }
        }
    }
}
