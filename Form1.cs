using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mogre;


namespace MOgreEditor
{
    public partial class Form1 : Form
    {
        Root root;
        RenderWindow window;
        SceneManager sceneManager;
        Camera camera;
        Viewport viewport;
        bool isRunning;
        public Form1()
        {
            InitializeComponent();
            IntPtr t = MOgreControl1.Handle;


            root = new Root();
            if (root.ShowConfigDialog())
            {

                NameValuePairList misc = new NameValuePairList();
                misc["externalWindowHandle"] = t.ToString();

                root.Initialise(false, "MOgre3D");

                window = root.CreateRenderWindow("Main RenderWindow", 800, 600, false, misc);


                sceneManager = root.CreateSceneManager(SceneType.ST_GENERIC);
                camera = sceneManager.CreateCamera("Camera");
                camera.SetPosition(0, 0, 50);

                camera.LookAt(new Vector3(0, 0, 0));
                camera.NearClipDistance = 5.0f;

                viewport = window.AddViewport(camera);
                viewport.BackgroundColour = ColourValue.Black;

                camera.AspectRatio = (float)viewport.ActualWidth / (float)viewport.ActualHeight;

                ResourceGroupManager.Singleton.AddResourceLocation("../../Media/packs/Sinbad.zip", "Zip");
                ResourceGroupManager.Singleton.InitialiseAllResourceGroups();
                Entity ent = sceneManager.CreateEntity("Sinbad.mesh");
                SceneNode node = sceneManager.RootSceneNode.CreateChildSceneNode("Sinbad");
                node.AttachObject(ent);
                //node.ShowBoundingBox = true;

                AxisAlignedBox axisAlignedBox = ent.BoundingBox;
                

                isRunning = true;
                Thread thread = new Thread(Go);
                thread.Start();

            }

        }

        public void Go()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long prev = stopwatch.ElapsedMilliseconds;
            long now = stopwatch.ElapsedMilliseconds;
            long delta;
            while (root != null && isRunning)
            {                
                now = stopwatch.ElapsedMilliseconds;
                delta = now - prev;
                if (delta>16)
                {
                    root.RenderOneFrame();
                    prev = now;
                }
                else
                {
                    Thread.Sleep(1);
                }
                Application.DoEvents();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false;
        }
    }
}
