using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using System.Numerics;
using static ImGuiNET.ImGuiNative;

namespace LovetapNF
{
    class Program
    {
        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;

        private static ImGuiController _controller;
        private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

        private static string ByteArrayToString(byte[] data)
        {
            if (data == null)
                return "<NULL>";
            return Encoding.ASCII.GetString(data);
        }

        static void Main(string[] args)
        {


            /*
            if (!ColliderCon.connect())
            {
                Console.WriteLine("Cannot connect to ColliderconVR service.\nMake Sure VRChat is running and that ColliderCon is loaded");
                Console.ReadLine();
                Environment.Exit(-1);
            }*/

            BTManager.rescanDevices();

            VeldridStartup.CreateWindowAndGraphicsDevice(
             new WindowCreateInfo(50, 50, 1024, 768, WindowState.Normal, "Lovetap"),
             new GraphicsDeviceOptions(true, null, true),
             out _window,
             out _gd);
            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                _controller.WindowResized(_window.Width, _window.Height);
            };

            _cl = _gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);
            CollideSystem.updateColliders();
            StartUI();
           // CollideSystem.colliders.Add(new WatchedCollider(CollideSystem.getColliderByName("celhandleft"), CollideSystem.getColliderByName("celhandright")));
            while (_window.Exists)
            {
                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }
                _controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                SubmitUI();

                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                _controller.Render(_gd, _cl);
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);
            }
            
        }

        public static unsafe void StartUI()
        {
         
        }
        private static Vector2 ZEROVEC = new Vector2(0, 0);
        public static unsafe void SubmitUI()
        {
            Interface.submit();
            CollideSystem.updateColliders();

            var nps = false;
            foreach (var colcheck in CollideSystem.colliders)
            {
                if (colcheck.tripped)
                {
                    nps = true;
                } 
            }

            if (nps==false)
            {
                BTManager.setPlugIntensity(0);
            } else
            {
                BTManager.setPlugIntensity(100);
            }
            /*
           
            ImGui.Begin("asd");
            var DrawList = ImGui.GetWindowDrawList();
            foreach (var w in CollideSystem.colliders)
            {
                var cc = w.rfirst;
                var cc1 = w.rsecond;
                var col = 0xFF0000FF;
                if (w.tripped==true)
                {
                    col = 0xFF00FF00;
                }
                DrawList.AddCircleFilled(new Vector2(cc.position.X * 20 + 600, cc.position.Z * 20 + 382), 5, col);
                DrawList.AddCircleFilled(new Vector2(cc1.position.X * 20 + 600, cc1.position.Z * 20 + 382), 5, col);
            }
            */

            ImGui.End();
        }
    }
}
