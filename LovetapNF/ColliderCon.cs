using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;

namespace LovetapNF
{
    public static class ColliderCon
    {
        static MemoryMappedFile shmem;
        static Stream shstr;
        static BinaryReader bReader;
        public static bool ready;

        public static string error = "No Error";

        public static bool connect()
        {
            try
            {
                shmem = MemoryMappedFile.OpenExisting("ColliderconVR");
                shstr = shmem.CreateViewStream();
                bReader = new BinaryReader(shstr);
                ready = true;
                return true;
            } catch (Exception e)
            {
                error = e.ToString();
                return false;
            }
        }

        public static ColliderData[] getColliders()
        {
            if (shmem==null)
                return null;
            shstr.Position = 0; // Reset position
            bReader.ReadInt32(); // Skip first 4 bytes (C# pointer weird stuff.)
            var count = bReader.ReadInt32();
            var colData = new ColliderData[count];
            bReader.ReadByte(); // Skip next 4 bytes, padded the int32 to be safe
            for (int i = 0; i < count; i++)
            {
                var nColl = new ColliderData();
                nColl.position = new Vector3(bReader.ReadSingle(), bReader.ReadSingle(), bReader.ReadSingle());
                var nameLength = bReader.ReadByte();
                nColl.name = Encoding.ASCII.GetString(bReader.ReadBytes(nameLength));
                nColl.radius = bReader.ReadSingle();
                colData[i] = nColl;
            }
           return colData;
        }
    }


    public class ColliderData
    {
        public string name;
        public Vector3 position;
        public float radius;
    }
}
