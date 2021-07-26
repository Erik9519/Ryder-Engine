using Ryder_Engine.Utils;
using Shell32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Ryder_Engine.Forms
{
    public partial class App_Launcher : Form
    {
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        public class App
        {
            public App(string path, string args, int pictureBoxSize)
            {
                this.path = path;
                this.args = args;
                this.base64icon = "";
                x = -1;
                y = -1;
                pictureBox = new PictureBox();
                pictureBox.Size = new Size(pictureBoxSize, pictureBoxSize);
            }

            public string path;
            public string args;
            public string base64icon;
            public int x;
            public int y;
            public PictureBox pictureBox;
        }

        public EventHandler appLauncherUpdated = null;
        App currApp;
        bool isDragging = false;
        List<App> apps = new List<App>();
        int[] grid_size = new int[] { 15, 9 };
        int pictubeBoxSize = 48;
        Hashtable appsGrid = new Hashtable();

        public App_Launcher()
        {
            InitializeComponent();
            initializeAppLauncher();
        }

        private void snapToGrid(App app, int screenX, int screenY)
        {
            int x = Convert.ToInt16(Math.Floor(screenX / (float)pictubeBoxSize));
            int y = Convert.ToInt16(Math.Floor(screenY / (float)pictubeBoxSize));
            x = Convert.ToInt16(Math.Max(0, Math.Min(grid_size[0] - 1, x)));
            y = Convert.ToInt16(Math.Max(0, Math.Min(grid_size[1] - 1, y)));

            if (checkIfPositionFree(x, y)) claimPosition(app, x, y);
        }

        private void instantiateAppPictureBox(App app)
        {
            // Extrapolate Icon
            IntPtr hIcon = IconExtractor.GetJumboIcon(IconExtractor.GetIconIndex(app.path));
            ImageConverter converter = new ImageConverter();
            Bitmap scaledIcon;
            using (Bitmap ico = ((Icon)Icon.FromHandle(hIcon).Clone()).ToBitmap())
            {
                scaledIcon = IconExtractor.ProcessIcon(ico, clipToCircle: false);
            }
            app.base64icon = Convert.ToBase64String((byte[])converter.ConvertTo(scaledIcon, typeof(byte[])));
            IconExtractor.Shell32.DestroyIcon(hIcon); // Cleanup
            GC.Collect();
            EmptyWorkingSet(Process.GetCurrentProcess().Handle);
            // Instatiate App PictureBox
            app.pictureBox.Image = scaledIcon;
            app.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            appsPanel.Invoke((MethodInvoker)delegate { 
                appsPanel.Controls.Add(app.pictureBox); 
            });
        }

        private void addAppButton_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                MTAOpenFileDialog dialog = new MTAOpenFileDialog();
                dialog.OnComplete = addNewApp;
                dialog.ShowDialog();
            }).Start();
        }

        private void addNewApp(object sender, EventArgs e)
        {
            MTAOpenFileDialog.MTAOpenFileDialogEventArgs args = (MTAOpenFileDialog.MTAOpenFileDialogEventArgs)e;
            // Get Application
            App newApp = new App(args.FileName, "", pictubeBoxSize);
            //// Process shortcuts
            if (newApp.path.Substring(newApp.path.LastIndexOf('.') + 1).Equals("lnk"))
            {
                string pathOnly = System.IO.Path.GetDirectoryName(newApp.path);
                string filenameOnly = System.IO.Path.GetFileName(newApp.path);
                Shell shell = new Shell();
                Shell32.Folder folder = shell.NameSpace(pathOnly);
                FolderItem folderItem = folder.ParseName(filenameOnly);

                if (folderItem != null)
                {
                    Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                    Debug.WriteLine("File: " + link.WorkingDirectory + "," + link.Target.Name);
                    if (!link.WorkingDirectory.Equals(""))
                    {
                        newApp.path = link.WorkingDirectory + "\\" + link.Target.Name;
                    }
                    else
                    {
                        newApp.path = link.Path;
                    }
                    newApp.args = link.Arguments;
                }
            }
            // Get Image of Application
            instantiateAppPictureBox(newApp);
            // Claim a position in launcher
            for (short x = 0; x < grid_size[0]; x++)
            {
                for (short y = 0; y < grid_size[1]; y++)
                {
                    if (checkIfPositionFree(x, y))
                    {
                        claimPosition(newApp, x, y);
                        break;
                    }
                }
                // Free position found. Abort Loop
                if (newApp.x != -1 && newApp.y != 1) break;
            }
            // Add Events
            attachEventsToPictureBox(newApp);
            apps.Add(newApp);
        }

        private bool checkIfPositionFree(int newPosX, int newPosY)
        {
            if (appsGrid.Contains(newPosX))
            {
                if (!((HashSet<int>)appsGrid[newPosX]).Contains(newPosY))
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private void claimPosition(App app, int newPosX, int newPosY)
        {
            // Update Position Data
            if (app.x != newPosX || app.y != newPosY)
            {
                // Freeup old position
                if (app.x != -1 && app.y != -1)
                {
                    ((HashSet<int>)appsGrid[app.x]).Remove(app.y);
                }
                // Claim new position
                if (appsGrid.Contains(newPosX))
                {
                    ((HashSet<int>)appsGrid[newPosX]).Add(newPosY);
                }
                else
                {
                    appsGrid.Add(newPosX, new HashSet<int>());
                    ((HashSet<int>)appsGrid[newPosX]).Add(newPosY);
                }
            }
            app.x = newPosX; app.y = newPosY;
            // Update PictureBox Position
            if (app.pictureBox.InvokeRequired)
            {
                app.pictureBox.Invoke((MethodInvoker)delegate
                {
                    app.pictureBox.Location = new Point(app.x * app.pictureBox.Size.Width, app.y * app.pictureBox.Size.Height);
                });
            }
            else
            {
                app.pictureBox.Location = new Point(app.x * app.pictureBox.Size.Width, app.y * app.pictureBox.Size.Height);
            }
        }

        private void attachEventsToPictureBox(App app)
        {
            app.pictureBox.MouseDown += (s, e) => {
                currApp = app;
                isDragging = true;
            };
            app.pictureBox.MouseUp += (s, e) => {
                isDragging = false;
            };
            app.pictureBox.MouseMove += (s, e) =>
            {
                if (isDragging)
                {
                    Point pos = appsPanel.PointToClient(Cursor.Position);
                    snapToGrid(currApp, pos.X, pos.Y);
                }
            };
            app.pictureBox.MouseDoubleClick += (s, e) =>
            {
                isDragging = false;
                appsPanel.Controls.Remove(app.pictureBox);
                ((HashSet<int>)appsGrid[app.x]).Remove(app.y);
                apps.Remove(app);
            };
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Apps_path != null)
            {
                Properties.Settings.Default.Apps_path.Clear();
                Properties.Settings.Default.Apps_args.Clear();
                Properties.Settings.Default.Apps_pos_x.Clear();
                Properties.Settings.Default.Apps_pos_y.Clear();
                Properties.Settings.Default.Apps_icons.Clear();
            } else
            {
                Properties.Settings.Default.Apps_path = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Apps_args = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Apps_pos_x = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Apps_pos_y = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Apps_icons = new System.Collections.Specialized.StringCollection();
            }

            for (int i = 0; i < apps.Count; i++)
            {
                Properties.Settings.Default.Apps_path.Add(apps[i].path);
                Properties.Settings.Default.Apps_args.Add(apps[i].args);
                Properties.Settings.Default.Apps_pos_x.Add(apps[i].x.ToString());
                Properties.Settings.Default.Apps_pos_y.Add(apps[i].y.ToString());
                Properties.Settings.Default.Apps_icons.Add(apps[i].base64icon);
            }
            Properties.Settings.Default.Save();
            if (appLauncherUpdated != null) appLauncherUpdated(this, null);
        }

        private void initializeAppLauncher()
        {
            try
            {
                if (Properties.Settings.Default.Apps_path != null)
                {
                    var appst = Properties.Settings.Default.Apps_icons;
                    for (int i = 0; i < Properties.Settings.Default.Apps_path.Count; i++)
                    {
                        App app = new App(
                            Properties.Settings.Default.Apps_path[i],
                            Properties.Settings.Default.Apps_args[i],
                            pictubeBoxSize
                        );
                        app.x = -1;
                        app.y = -1;
                        app.base64icon = Properties.Settings.Default.Apps_icons[i];
                        // Initialize PictureBox
                        app.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        byte[] byteBuffer = Convert.FromBase64String(app.base64icon);
                        MemoryStream memoryStream = new MemoryStream(byteBuffer);
                        app.pictureBox.Image = new Bitmap((Bitmap)Bitmap.FromStream(memoryStream));
                        memoryStream.Close();
                        attachEventsToPictureBox(app);
                        appsPanel.Controls.Add(app.pictureBox);
                        // Snap to grid
                        claimPosition(
                            app,
                            Int16.Parse(Properties.Settings.Default.Apps_pos_x[i]),
                            Int16.Parse(Properties.Settings.Default.Apps_pos_y[i])
                        );
                        // Add to list apps
                        apps.Add(app);
                    }
                }
            } catch
            {
                appsGrid = new Hashtable();
                apps.Clear();
            }
        }

        public static bool launchApp(int n)
        {
            if (Properties.Settings.Default.Apps_path != null && n < Properties.Settings.Default.Apps_path.Count)
            {
                Process.Start(Properties.Settings.Default.Apps_path[n], Properties.Settings.Default.Apps_args[n]);
                return true;
            }
            return false;
        }

        public static string getAppLauncherJson()
        {
            if (Properties.Settings.Default.Apps_path != null) {
                string data = "[";
                for (int i = 0; i < Properties.Settings.Default.Apps_path.Count; i++)
                {
                    data += "{";
                    data += "\"x\":" + Properties.Settings.Default.Apps_pos_x[i].ToString() + ",";
                    data += "\"y\":" + Properties.Settings.Default.Apps_pos_y[i].ToString() + ",";
                    data += "\"icon\":\"" + Properties.Settings.Default.Apps_icons[i] + "\"";
                    data += "}";
                    if (i < Properties.Settings.Default.Apps_path.Count - 1) data += ",";
                }
                data += "]";
                return data;
            }
            return null;
        }
    }
}
