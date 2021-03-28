using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;

namespace WallPaperChangerV2
{
    public class ScreenFetcher : List<ScreenInfo>
    {
        //Size of the monitor space
        private Size size;
        //Offset point to compensate for the primary screen not being top left
        private Point offset;

        public Size Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }

        public Point Offset
        {
            get
            {
                return offset;
            }

            set
            {
                offset = value;
            }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ScreenFetcher()
        {
            getScreens();
        }

        /// <summary>
        /// Refreshes data stored in this class with current data from the Screens class.
        /// </summary>
        public void RefreshScreens()
        {
            this.Clear();
            getScreens();
        }

        /// <summary>
        /// Gets all the screens for the system and saves it to the screens list.
        /// </summary>
        private void getScreens()
        {
            // 'Magic' that clears the Screen Static class so it can refresh with current information
            typeof( Screen ).GetField( "screens", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic ).SetValue( null, null );
            Screen[] systemScreens = Screen.AllScreens;

            offset = new Point( 0, 0 );

            //Determine 0,0 and Height/Width of screen area.
            foreach ( Screen screen in systemScreens )
            {
                if ( screen.Bounds.Left < Offset.X )
                {
                    offset.X = screen.Bounds.Left;
                }

                if ( screen.Bounds.Top < Offset.Y )
                {
                    offset.Y = screen.Bounds.Top;
                }

                if ( screen.Bounds.Right > size.Width )
                {
                    size.Width = screen.Bounds.Right;
                }

                if ( screen.Bounds.Bottom > size.Height )
                {
                    size.Height = screen.Bounds.Bottom;
                }
            }

            //Adjust size to include offset
            size.Width = size.Width - offset.X;
            size.Height = size.Height - offset.Y;

            //Adds the screen
            foreach ( Screen screen in systemScreens )
            {
                Point bitmapLocation = new Point( screen.Bounds.Location.X - Offset.X,
                                                  screen.Bounds.Location.Y - Offset.Y );

                ScreenInfo currentScreen = new ScreenInfo( screen.Bounds.Size,
                        screen.Bounds.Location,
                        bitmapLocation );

                this.Add( currentScreen );
            }
        }
    }


}
