using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Bluetooth;


namespace LovetapNF
{
    public static class BTManager
    {
        public static BluetoothDevice[] pairedDevices;
        public static string[] pairdDevicesNames;
        public static BluetoothDevice selectedDevice;
        public static bool scanning = false;
        public static GattCharacteristic plugComm;
        public static bool connecting = false;
        public static int plugIntensity = 0;


        public static void setPlugIntensity(int number)
        {
            if (number == plugIntensity)
                return;
            if (plugComm == null)
                return;
            if (selectedDevice == null)
                return;
            plugIntensity = number;
            var data = Encoding.ASCII.GetBytes("Vibrate:" + number + ";");
            plugComm.WriteValueWithoutResponseAsync(data);
        }

        public static async void rescanDevices()
        {
            scanning = true;
            var cl1 = await Bluetooth.GetPairedDevicesAsync();
            pairedDevices = new BluetoothDevice[cl1.Count];
            pairdDevicesNames = new string[cl1.Count];
            var i = 0;
            foreach (var dev in cl1)
            {
                pairedDevices[i] = dev;
                pairdDevicesNames[i] = dev.Name;
                i++;
            }
            scanning = false; 
        }

        public static async Task<bool> connectDevice(BluetoothDevice dev)
        {
            Console.WriteLine($"Trying to pair with {dev.Name}");
            connecting = true;
            await dev.Gatt.ConnectAsync();
            if (dev.Gatt.Connected)
            {
                selectedDevice = dev;
                connecting = false;
                checkPlug();
                Console.WriteLine("Device ready!");
                return true;
            }
            connecting = false;
           
            return false;
        }

        public static bool checkPlug()
        {
            if (selectedDevice == null)
                return false;
            Console.WriteLine("Trying PrimaryService at 5a300001-0023-4bd4-bbd5-a6920e4c5653");
            var devsvc = selectedDevice.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new Guid("5a300001-0023-4bd4-bbd5-a6920e4c5653"))).Result;
            Console.WriteLine("Primary service successfully allocated.");
            Console.WriteLine("Getting service characteristics...");

            try
            {
                plugComm = devsvc.GetCharacteristicsAsync().Result[0];
                Console.WriteLine("Got characteristic service " + plugComm.Uuid);
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
