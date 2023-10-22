using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        SceneNode sinbadNode;
        SceneNode cameraNode;
        Viewport viewport;
        bool isRunning;
        EditorScene editorScene;
        public Form1()
        {
            InitializeComponent();
            IntPtr t = MOgreControl1.Handle;
            


            /*TreeNode rootNode = new TreeNode("Root");
            // Добавляем новый дочерний узел к rootNode
            rootNode.Nodes.Add(new TreeNode("Смартфоны"));
            // Добавляем rootNode вместе с дочерними узлами в TreeView
            treeView1.Nodes.Add(rootNode);
            // Добавляем второй очерний узел к первому узлу в TreeView
            treeView1.Nodes[0].Nodes.Add(new TreeNode("Планшеты"));*/


            root = new Root();
            if (root.ShowConfigDialog())
            {

                NameValuePairList misc = new NameValuePairList();
                misc["externalWindowHandle"] = t.ToString();

                root.Initialise(false, "MOgre3D");

                window = root.CreateRenderWindow("Main RenderWindow", 800, 600, false, misc);


                sceneManager = root.CreateSceneManager(SceneType.ST_GENERIC);
                camera = sceneManager.CreateCamera("Camera");
                //  camera.SetPosition(0, 0, 50);

                //  camera.LookAt(new Vector3(0, 0, 0));
                camera.NearClipDistance = 5.0f;

                viewport = window.AddViewport(camera);
                viewport.BackgroundColour = ColourValue.Black;

                camera.AspectRatio = (float)viewport.ActualWidth / (float)viewport.ActualHeight;

                ResourceGroupManager.Singleton.AddResourceLocation("../../Media/packs/Sinbad.zip", "Zip");
                ResourceGroupManager.Singleton.InitialiseAllResourceGroups();

                editorScene = new EditorScene(treeView1,sceneManager);


                sinbadNode = editorScene.AddEditorSceneNode("Sinbad", "Sinbad.mesh");
                SceneNode sinbadNode1  = editorScene.AddEditorSceneNode("Sinbad1", "Sinbad.mesh");
                // sinbadNode = sceneManager.RootSceneNode.CreateChildSceneNode("Sinbad");
                // sinbadNode.AttachObject(ent);
                /*                cameraNode = sinbadNode.CreateChildSceneNode();
                                cameraNode.Position = new Vector3(0, 0, 50);
                                cameraNode.LookAt(sinbadNode.Position, Node.TransformSpace.TS_WORLD);


                                 mNode->setPosition(targetPos + rotateQuaternion * aroundPos);
                // quite sure you want the SceneNode to face the "targetPos"
                mNode->lookAt(targetPos, Node::TS_WORLD);
                // or
                mNode->setOrientation(aroundPos.getRotationTo(targetPos));
                                 */


                /*SceneNode sinbadNode1 = sceneManager.RootSceneNode.CreateChildSceneNode("Sinbad1");
                sinbadNode1.AttachObject(ent1);*/
                sinbadNode1.Position = new Vector3(10.0f, 2.0f, 3.0f);

                //sinbadNode.ShowBoundingBox = true;

               // AxisAlignedBox axisAlignedBox = ent.BoundingBox;

                MOgreControl1.myMouseMoved += mouseMovedEvent;
                MOgreControl1.myMouseDown += mouseDownEvent;

                isRunning = true;
                Thread thread = new Thread(Go);
                thread.Start();

            }

        }

        private void mouseDownEvent(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void mouseMovedEvent(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
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
                if (delta > 16)
                {
                    root.RenderOneFrame();
                    prev = now;
                }
                else
                {
                    // sinbadNode.Yaw(new Radian(0.01f));
                    //  sinbadNode.Pitch(new Radian(0.01f));
                    /*Quaternion t1 = sinbadNode.Orientation;
                    
                    Matrix3 rotMatrix= t1.ToRotationMatrix();
                    Radian mYaw;
                    Radian mPitch;
                    Radian mRoll;
                    rotMatrix.ToEulerAnglesYXZ(out mYaw, out mPitch, out mRoll);*/
                    Euler euler = new Euler(sinbadNode.Orientation);
                    euler.AddYaw(new Degree(0.1f));
                    sinbadNode.Orientation = euler.ToQuaternion();

                   // camera.Position = cameraNode.Position;
                    camera.LookAt(sinbadNode.Position);
                    Vector3 tt1 = new Vector3(0, 0, -50);        
                    camera.Position = sinbadNode.Position + sinbadNode.Orientation * tt1;


                    Thread.Sleep(1);
                }
                Application.DoEvents();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode treeNode = e.Node;
            EditorSceneNode sceneNode1 = null;
            try
            {
                if (treeNode != null)
                {
                    if (treeNode.Name != null)
                    {
                        bool found = false;
                        foreach (EditorSceneNode sceneNode in editorScene.children)
                        {
                            sceneNode.sceneNode.ShowBoundingBox = false;
                            if (sceneNode.name == treeNode.Text)
                            {
                                sceneNode1 = sceneNode;
                                found = true;                                
                            }
                        }
                        if (found)
                        {
                            sceneNode1.sceneNode.ShowBoundingBox = true;
                            tbPosition.Text = sceneNode1.sceneNode.Position.ToString();
                            tbRotation.Text = sceneNode1.sceneNode.Orientation.ToString();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
