using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WallPaperChangerV2
{
    public class GenerateWallpaper
    {
        public static String OUTFILE = "Wallpaper.jpg";
        Bitmap wallBitmap;
        private Graphics wallPaper;
        private string files;
        private int numFiles;
        private ScreenFetcher screens;
        private List<string> currentImages;
        private int counter;
        private int outputFileNumber;
        bool isRandom;

        /// <summary>
        /// Constructor
        /// </summary>
        public GenerateWallpaper( ScreenFetcher screens, string files, bool isRandom )
        {
            this.screens = screens;
            this.files = files;
            this.isRandom = isRandom;
            counter = 0;
            outputFileNumber = 0;
        }

        /// <summary>
        /// Creates the image that will be displayed
        /// </summary>
        public string generateImage()
        {
            screens.RefreshScreens();

            // Clear the file that is stored on disk and set as wallpaper.
            if ( File.Exists( outputFileNumber + ".jpg" ) )
            {
                File.Delete( ( outputFileNumber + ".jpg" ) );
            }

            // Dispose of the bitmap thats used to make the file.
            if ( wallBitmap != null )
            {
                wallBitmap.Dispose();
            }

            // Clean the graphics class.
            if ( wallPaper != null )
            {
                wallPaper.Dispose();
            }

            // Reset the above items.
            wallBitmap = new Bitmap( screens.Size.Width, screens.Size.Height );
            wallPaper = Graphics.FromImage( wallBitmap );

            // Get a group of files that will be used for the first runthough.
            if ( currentImages == null )
            {
                currentImages = new List<string>();

                for ( int i = 0; i < screens.Count; i++ )
                {
                    currentImages.Add( getFile() );
                    Thread.Sleep( 100 );
                }

                counter++;
            }

            // Remove current image for current screen and regenerate the bitmap.
            for ( int i = 0; i < screens.Count; i++ )
            {
                // Clear storage for selected screen and get random image to use.
                if ( counter % screens.Count == i )
                {
                    currentImages.RemoveAt( i );
                    string nextImage = getFile();

                    while ( currentImages.Contains( nextImage ) )
                    {
                        nextImage = getFile();
                    }

                    currentImages.Insert( i, nextImage );
                }

                // Load the image
                Image loadedImage = Image.FromFile( currentImages.ElementAt( i ) );
                Bitmap currentBitmap = new Bitmap( screens.ElementAt( i ).ScreenSize.Width, screens.ElementAt( i ).ScreenSize.Height );

                // Get the image size and height that it should be scaled too.
                // TODO: Split into seperate method?
                // Remark: Its also done in resize image.
                int bitmapHeight;
                int bitmapWidth;

                if ( screens.ElementAt( i ).ScreenSize.Height < screens.ElementAt( i ).ScreenSize.Width )
                {
                    bitmapWidth = screens.ElementAt( i ).ScreenSize.Height * loadedImage.Width / loadedImage.Height;
                    bitmapHeight = screens.ElementAt( i ).ScreenSize.Height;
                }
                else
                {
                    bitmapWidth = screens.ElementAt( i ).ScreenSize.Width;
                    bitmapHeight  = screens.ElementAt( i ).ScreenSize.Width * loadedImage.Height / loadedImage.Width;
                }

                // Resize the image to fit the screen.
                ResizeImage( screens.ElementAt( i ).ScreenSize, loadedImage, currentBitmap );

                // Draw the image.
                Point point = new Point( screens.ElementAt( i ).BitmapLocation.X +
                                         ( screens.ElementAt( i ).ScreenSize.Width - bitmapWidth ) / 2,
                                         screens.ElementAt( i ).BitmapLocation.Y +
                                         ( screens.ElementAt( i ).ScreenSize.Height - bitmapHeight ) / 2 );

                wallPaper.DrawImage( currentBitmap, point );

                // Clean up.
                loadedImage.Dispose();
                currentBitmap.Dispose();

            }

            // Final clean up
            counter++;
            outputFileNumber = ++outputFileNumber % 5;
            wallBitmap.Save( ( outputFileNumber + ".jpg" ) );

            // The file name that was written.
            return ( outputFileNumber + ".jpg" );

        }

        /// <summary>
        /// Gets a list of files.
        /// </summary>
        private string getFile()
        {
            // Get a list of files.
            if ( isRandom )
            {
                FileInfo[] allFiles = GetFiles();
                return allFiles[getRandomNumber()].FullName;
            }
            else
            {
                FileInfo[] allFiles = GetFiles();
                return allFiles[counter % allFiles.Count()].FullName;
            }
        }

        /// <summary>
        /// Resize image to fit in screen.
        /// </summary>
        private void ResizeImage( Size size, Image tempImage, Bitmap currentBitmap )
        {
            // Use graphics and scale image to fit given size.
            using ( Graphics gr = Graphics.FromImage( currentBitmap ) )
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;

                if( size.Height < size.Width && tempImage.Height > tempImage.Width )
                {
                    gr.DrawImage( tempImage, new Rectangle( 0, 0, size.Height * tempImage.Height / tempImage.Width, size.Height ) );
                }
                else if ( size.Height < size.Width && tempImage.Height < tempImage.Width )
                {
                    gr.DrawImage( tempImage, new Rectangle( 0, 0, size.Height * tempImage.Width / tempImage.Height, size.Height ) );
                }
                else if ( size.Height > size.Width && tempImage.Height > tempImage.Width )
                {
                    gr.DrawImage( tempImage, new Rectangle( 0, 0, size.Width, size.Width * tempImage.Height / tempImage.Width ) );
                }
                else if ( size.Height > size.Width && tempImage.Height < tempImage.Width )
                {
                    gr.DrawImage( tempImage, new Rectangle( 0, 0, size.Width, size.Width * tempImage.Width / tempImage.Height ) );
                }

            }
        }

        /// <summary>
        /// Get all the files that will be used.
        /// </summary>
        private FileInfo[] GetFiles()
        {
            DirectoryInfo folder = new DirectoryInfo( this.files + "\\" );
            FileInfo[] files = folder.GetFiles();
            Array.Sort( files, ( f1, f2 ) => f1.Name.CompareTo( f2.Name ) );
            numFiles = folder.GetFiles().Count();
            return files;
        }

        /// <summary>
        /// Get a random number bounded by the number of files.
        /// </summary>
        private int getRandomNumber()
        {
            return new Random().Next( numFiles );
        }
    }


}
