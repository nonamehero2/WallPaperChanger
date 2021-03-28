using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace WallPaperChangerV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Location of the serialized settings file.
        string settingsLocation;
        StoreLoadFiles settings;
        GenerateWallpaper generator;
        string interval;
        private static Mutex mut = new Mutex();
        public MainWindow()
        {
            //Command Line Arguments.
            string[] e = Environment.GetCommandLineArgs();
            settingsLocation = Directory.GetCurrentDirectory() + '/' + System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            //Start window.
            InitializeComponent();

            //Load existing settings.
            if ( File.Exists( settingsLocation ) )
            {
                settings = ( StoreLoadFiles )StoreLoadFiles.Load( settingsLocation );
            }
            //Create new settings file.
            else
            {
                settings = new StoreLoadFiles( "", false, 10 );
                StoreLoadFiles.Store( settingsLocation, settings );
            }

            //Populating on screen elements.
            folder_Textbox.Text = settings.FileLocation;
            interval_textbox.Text = settings.TimeInMinutes.ToString();
            //dualScreen_checkBox.IsChecked = settings.DualScreen;
            random_checkBox.IsChecked = settings.IsRandom;

            //Hide the application if its started with '-hide' commandline arg.
            if ( e.Contains( "-hide" ) )
            {
                startButton_click( null, null );
                this.Hide();
            }
        }

        private void startButton_click( object sender, RoutedEventArgs e )
        {
            //Text for interval.
            interval = interval_textbox.Text;
            if ( interval.Equals( "" ) )
            {
                interval = "1";
            }

            //Stores interval in settings file.
            StoreLoadFiles settings = new StoreLoadFiles( folder_Textbox.Text, ( bool )random_checkBox.IsChecked, Convert.ToDouble( interval ) );
            StoreLoadFiles.Store( settingsLocation, settings );

            //Generator for wallpaper.
            generator = new GenerateWallpaper( new ScreenFetcher(), folder_Textbox.Text, settings.IsRandom );
            this.Hide();

            Thread loopingThread = new Thread( ChangeWallpaper );
            loopingThread.Start( true );

            Thread checkThread = new Thread( CheckForScreenChange );
            checkThread.Start();

            // ChangeWallpaper(true);
        }

        private void ChangeWallpaper( object _loop )
        {
            bool loop = ( bool )_loop;
            //Wallpaper loop
            while ( loop )
            {
                mut.WaitOne();
                WallpaperClass.SetWallpaper( Directory.GetCurrentDirectory() + "\\" + generator.generateImage() );
                mut.ReleaseMutex();
                if ( loop )
                {
                    Thread.Sleep( ( int )( 60000 * Convert.ToDouble( interval ) ) );
                }
            }
        }

        private void CheckForScreenChange()
        {
            const int CHECK_TIME = 1000;
            Screen[] prevScreen = Screen.AllScreens;
            while ( true )
            {
                typeof( Screen ).GetField( "screens", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic ).SetValue( null, null );
                bool equal = false;
                for ( int i = 0; ( Screen.AllScreens.Count() == prevScreen.Count() ) && i < prevScreen.Count(); i++ )
                {
                    if ( prevScreen[i].Bounds.Bottom != Screen.AllScreens[i].Bounds.Bottom ||
                            prevScreen[i].Bounds.Left != Screen.AllScreens[i].Bounds.Left ||
                            prevScreen[i].Bounds.Right != Screen.AllScreens[i].Bounds.Right ||
                            prevScreen[i].Bounds.Top != Screen.AllScreens[i].Bounds.Top ||
                            prevScreen[i].Bounds.Location.X != Screen.AllScreens[i].Bounds.Location.X ||
                            prevScreen[i].Bounds.Location.Y != Screen.AllScreens[i].Bounds.Location.Y )
                    {
                        equal = false;
                        break;
                    }
                    else
                    {
                        equal = true;
                    }
                }

                if ( !equal )
                {
                    mut.WaitOne();
                    WallpaperClass.SetWallpaper( Directory.GetCurrentDirectory() + "\\" + generator.generateImage() );
                    mut.ReleaseMutex();
                }
                //Screen.AllScreens.CopyTo(prevScreen, 0);
                prevScreen = Screen.AllScreens;
                Thread.Sleep( CHECK_TIME );
            }
        }

        //Controls the range of characters that can be entered in the textbox.
        private void TextBox_PreviewKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            int key = ( int )e.Key;

            e.Handled = !( key >= 34 && key <= 43 ||
                           key >= 74 && key <= 83 || key == 2 || key == 88 );
        }

        //File browser menu.
        private void browse_button_Click( object sender, RoutedEventArgs e )
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();
            browser.ShowDialog();
            folder_Textbox.Text = browser.SelectedPath;
        }
    }
}
