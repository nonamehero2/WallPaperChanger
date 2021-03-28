using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallPaperChangerV2
{
    public class ScreenInfo
    {
        /// <summary>
        /// Size of the screen.
        /// </summary>
        private Size screenSize;
        /// <summary>
        /// Actual location of the screen in windows;
        /// </summary>
        private Point actualLocation;
        /// <summary>
        /// Location of screen in a bitmap;
        /// </summary>
        private Point bitmapLocation;

        public Size ScreenSize
        {
            get
            {
                return screenSize;
            }

            set
            {
                screenSize = value;
            }
        }

        public Point ActualLocation
        {
            get
            {
                return actualLocation;
            }

            set
            {
                actualLocation = value;
            }
        }

        public Point BitmapLocation
        {
            get
            {
                return bitmapLocation;
            }

            set
            {
                bitmapLocation = value;
            }
        }

        public ScreenInfo(Size size, Point actualLocation, Point bitmapLocation)
        {
            this.ScreenSize = size;
            this.ActualLocation = actualLocation;
            this.BitmapLocation = bitmapLocation;
        }

        public override string ToString()
        {
            return String.Format("Height: {0} Width: {1}\nActual Location: ({2},{3})\nBitmap Location: ({4},{5})\n\n",
                                    screenSize.Height, screenSize.Width,
                                    actualLocation.X, actualLocation.Y,
                                    bitmapLocation.X, bitmapLocation.Y);
        }
    }

    
}
