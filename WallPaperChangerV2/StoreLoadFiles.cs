using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WallPaperChangerV2
{
    [Serializable]
    public class StoreLoadFiles
    {
        string fileLocation;
        bool isRandom;
        bool dualScreen;
        double timeInMinutes;

        public StoreLoadFiles(string FileLocation, bool IsRandom, double TimeInMinutes)
        {
            this.FileLocation = FileLocation;
            this.IsRandom = IsRandom;
            this.TimeInMinutes = TimeInMinutes;
        }

        public string FileLocation
        {
            get
            {
                return fileLocation;
            }

            set
            {
                fileLocation = value;
            }
        }

        public bool IsRandom
        {
            get
            {
                return isRandom;
            }

            set
            {
                isRandom = value;
            }
        }

        public double TimeInMinutes
        {
            get
            {
                return timeInMinutes;
            }

            set
            {
                if(value <= 0)
                {
                    timeInMinutes = 1;
                    return;
                }
                timeInMinutes = value;
            }
        }

        public static void Store(string fileName, object userData)
        {
            BinaryFormatter binFormat = new BinaryFormatter();

            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);

            binFormat.Serialize(stream, userData);
            stream.Close();
        }

        public static object Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("File not found!");
            }

            BinaryFormatter binFormat = new BinaryFormatter();

            Stream instream = File.OpenRead(fileName);
            object obj = binFormat.Deserialize(instream);
            instream.Close();
            return obj;
        }
    }
}
