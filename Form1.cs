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
using System.Windows.Media.Imaging;

using Mogre;
using MogreNewt.CollisionPrimitives;
//using Ogre;



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
        Vector3 currentObjectPosition;
        Rotation currentObjectRotation;
        SceneNode selectedObjectSceneNode;
        RaySceneQuery mRaySceneQuery;

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


                editorScene = new EditorScene(treeView1, sceneManager);


                sinbadNode = editorScene.AddEditorSceneNode("Sinbad", "Sinbad.mesh");
                sinbadNode = editorScene.AddEditorSceneNode("Sinbad1", "Sinbad.mesh");
                editorScene.AddEditorSceneNode("Camera", "Camera");

                editorScene.GetEditorSceneNodeByName("Sinbad1").Position = new Vector3(10.0f, 2.0f, 3.0f);

                camera.Position = new Vector3(0, 0, -50);
                camera.LookAt(editorScene.GetEditorSceneNodeByName("Sinbad1").Position);

                MOgreControl1.myMouseMoved += mouseMovedEvent;
                MOgreControl1.myMouseDown += mouseDownEvent;

                mRaySceneQuery = sceneManager.CreateRayQuery(new Ray());

                isRunning = true;
                Thread thread = new Thread(Go);
                thread.Start();

            }

        }

        private void mouseDownEvent(object sender, EventArgs e)
        {
            int x1 = ((System.Windows.Forms.MouseEventArgs)e).X;
            int y1 = ((System.Windows.Forms.MouseEventArgs)e).Y;
            label7.Text = "X = " + x1.ToString() + " Y = " + y1.ToString();

            // Setup the ray scene query
            float screenX = x1 / (float)800;
            float screenY = y1 / (float)600;
            Ray mouseRay = camera.GetCameraToViewportRay(screenX, screenY);
            mRaySceneQuery.Ray = mouseRay;

            // Execute query
            RaySceneQueryResult result = mRaySceneQuery.Execute();
            RaySceneQueryResult.Enumerator itr = (RaySceneQueryResult.Enumerator)(result.GetEnumerator());

            // Get results, create a node/entity on the position
            if (itr != null && itr.MoveNext())
            {

                int yy = 1;

                // ((Mogre.SceneNode)itr.Current.movable.ParentNode).ShowBoundingBox = !((Mogre.SceneNode)itr.Current.movable.ParentNode).ShowBoundingBox;
                if (selectedObjectSceneNode != null)
                    selectedObjectSceneNode.ShowBoundingBox = false;
                selectedObjectSceneNode = (Mogre.SceneNode)itr.Current.movable.ParentNode;
                UpdateTextObject(0);





                /*if (useCurrent)
                {
                    mCurrentObject.Position = itr.Current.worldFragment.singleIntersection;
                }
                else
                {
                    Entity ent = sceneMgr.CreateEntity(
                                      "Robot" + mCount.ToString(), "robot.mesh");

                    mCurrentObject = sceneMgr.RootSceneNode.CreateChildSceneNode(
                        "RobotNode" + mCount.ToString(),
                        itr.Current.worldFragment.singleIntersection);

                    mCount++;
                    mCurrentObject.AttachObject(ent);
                    mCurrentObject.SetScale(0.1f, 0.1f, 0.1f);
                }*/
            }


            //   throw new NotImplementedException();
        }

        private void mouseMovedEvent(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
        }


        public void Go()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long prev = stopwatch.ElapsedMilliseconds;
            long prevGUI = stopwatch.ElapsedMilliseconds;
            long now = stopwatch.ElapsedMilliseconds;
            long delta;
            long deltaGUI;
            while (root != null && isRunning)
            {
                now = stopwatch.ElapsedMilliseconds;
                delta = now - prev;
                deltaGUI = now - prevGUI;
                if (delta > 16)
                {
                    root.RenderOneFrame();
                    prev = now;
                }
                else
                {
                    if (deltaGUI > 500)
                    {
                        prevGUI = now;

                    }

                    /*
                    Euler euler = new Euler(sinbadNode.Orientation);
                    euler.AddYaw(new Degree(0.1f));
                    sinbadNode.Orientation = euler.ToQuaternion();
                   
                    camera.LookAt(sinbadNode.Position);
                    Vector3 tt1 = new Vector3(0, 0, -50);        
                    camera.Position = sinbadNode.Position + sinbadNode.Orientation * tt1;
                    */

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
                            if (sceneNode.sceneNode != null)
                            {
                                sceneNode.sceneNode.ShowBoundingBox = false;
                                if (sceneNode.name == treeNode.Text)
                                {
                                    sceneNode1 = sceneNode;
                                    found = true;
                                }
                            }
                        }
                        if (found)
                        {
                            selectedObjectSceneNode = sceneNode1.sceneNode;
                            UpdateTextObject(0);
                        }
                        else
                        {
                            if (treeNode.Text == "Camera")
                            {
                                if (selectedObjectSceneNode != null)
                                    selectedObjectSceneNode.ShowBoundingBox = false;
                                selectedObjectSceneNode = null;
                                UpdateTextObject(1);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void UpdateTextObject(int v)
        {
            try
            {
                if (v == 0)
                {
                    if (selectedObjectSceneNode != null)
                    {
                        ushort t1 = selectedObjectSceneNode.NumChildren();

                        if (t1 == 0)
                        {
                            int yyyy1 = selectedObjectSceneNode.NumAttachedObjects();
                            if (yyyy1 == 1)
                            {
                                MovableObject movableObject1 = selectedObjectSceneNode.GetAttachedObject(0);
                                int hfdgh = 1;
                                label8.Text = selectedObjectSceneNode.Name;
                                label9.Text = ((Mogre.Entity)movableObject1).MovableType;
                                label10.Text = movableObject1.Name;
                                //label11.Text = itr.Current.distance.ToString("F3");
                            }
                            else
                            {
                                SceneNode.ObjectIterator ii = selectedObjectSceneNode.GetAttachedObjectIterator();
                                int yyy1 = ii.Count<MovableObject>();
                                while (ii.MoveNext())
                                {
                                    MovableObject movableObject = ii.Current;
                                    string movableType = movableObject.MovableType;
                                }
                            }
                            Node.ChildNodeIterator gg = selectedObjectSceneNode.GetChildIterator();
                            Node hh = gg.Current;


                        }


                        selectedObjectSceneNode.ShowBoundingBox = true;
                        tbPositionX.Invoke(new Action(() => tbPositionX.Text = selectedObjectSceneNode.Position.x.ToString()));
                        tbPositionY.Invoke(new Action(() => tbPositionY.Text = selectedObjectSceneNode.Position.y.ToString()));
                        tbPositionZ.Invoke(new Action(() => tbPositionZ.Text = selectedObjectSceneNode.Position.z.ToString()));

                        Euler euler = new Euler(selectedObjectSceneNode.Orientation);

                        tbYaw.Invoke(new Action(() => tbYaw.Text = euler.Yaw.ValueDegrees.ToString()));
                        tbPitch.Invoke(new Action(() => tbPitch.Text = euler.Pitch.ValueDegrees.ToString()));
                        tbRoll.Invoke(new Action(() => tbRoll.Text = euler.Roll.ValueDegrees.ToString()));

                    }
                }
                else if (v == 1)
                {
                    if ((editorScene != null) && (editorScene.camera != null))
                    {
                        Camera camera = editorScene.camera;
                        tbPositionX.Invoke(new Action(() => tbPositionX.Text = camera.Position.x.ToString()));
                        tbPositionY.Invoke(new Action(() => tbPositionY.Text = camera.Position.y.ToString()));
                        tbPositionZ.Invoke(new Action(() => tbPositionZ.Text = camera.Position.z.ToString()));

                        Euler euler = new Euler(camera.Orientation);

                        tbYaw.Invoke(new Action(() => tbYaw.Text = euler.Yaw.ValueDegrees.ToString()));
                        tbPitch.Invoke(new Action(() => tbPitch.Text = euler.Pitch.ValueDegrees.ToString()));
                        tbRoll.Invoke(new Action(() => tbRoll.Text = euler.Roll.ValueDegrees.ToString()));

                    }
                }
            }
            catch { }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files(*.xml)|*.xml|All files(*.*)|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
            {
                SaveScene.WriteScene(saveFileDialog.FileName, editorScene);
            }
        }

        private void LoadMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                editorScene.Dispose();
                editorScene.treeView1.Nodes.Clear();
                editorScene = new EditorScene(treeView1, sceneManager);
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "XML files(*.xml)|*.xml|All files(*.*)|*.*";
                if (openFileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    SaveScene.ReadScene(openFileDialog.FileName, editorScene);
                    selectedObjectSceneNode = null;
                }
            }
            catch { }
        }

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                editorScene.Dispose();
                editorScene.treeView1.Nodes.Clear();
                editorScene = new EditorScene(treeView1, sceneManager);
                selectedObjectSceneNode = null;
                editorScene.children.Clear();
                editorScene.sceneManager.ClearScene();
                editorScene.camera = new Camera("Camera", editorScene.sceneManager);

                editorScene.camera.Position = new Vector3(0, 0, -50);
                editorScene.camera.Orientation = new Quaternion(0, 0, 1, 0);
                editorScene.camera.Direction = new Vector3(0, 0, 0);
                editorScene.AddEditorSceneNode("Camera", "Camera");

            }
            catch { }

        }

        private void tbPositionX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                /*  if (selectedObjectSceneNode != null)
                  {
                      Vector3 pos = selectedObjectSceneNode.Position;
                      float tz1;
                      bool t2 = float.TryParse(tbPositionX.Text, out tz1);
                      if (t2)
                      {
                          Vector3 vector = new Vector3(tz1, pos.y, pos.z);
                          selectedObjectSceneNode.Position = vector;
                      }
                  }*/
            }
            catch { }
        }

        private void tbPositionY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                /*  if (selectedObjectSceneNode != null)
                  {
                      Vector3 pos = selectedObjectSceneNode.Position;
                      float tz1;
                      bool t2 = float.TryParse(tbPositionY.Text, out tz1);
                      if (t2)
                      {
                          Vector3 vector = new Vector3(pos.x, tz1, pos.z);
                          selectedObjectSceneNode.Position = vector;
                      }
                  }*/
            }
            catch { }
        }

        private void tbPositionZ_TextChanged(object sender, EventArgs e)
        {
            try
            {
                /* if (selectedObjectSceneNode != null)
                 {
                     Vector3 pos = selectedObjectSceneNode.Position;
                     float tz1;
                     bool t2 = float.TryParse(tbPositionZ.Text, out tz1);
                     if (t2)
                     {
                         Vector3 vector = new Vector3(pos.x, pos.y, tz1);
                         selectedObjectSceneNode.Position = vector;
                     }
                 }*/
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedObjectSceneNode != null)
            {
                UpdateTextObject(0);
            }
            else
            {
                UpdateTextObject(1);
            }


            /*  if ((editorScene != null) && (editorScene.camera != null))
              {
                  Camera camera = editorScene.camera;
                  tbPositionX.Invoke(new Action(() => tbPositionX.Text = camera.Position.x.ToString()));
                  tbPositionY.Invoke(new Action(() => tbPositionY.Text = camera.Position.y.ToString()));
                  tbPositionZ.Invoke(new Action(() => tbPositionZ.Text = camera.Position.z.ToString()));

                  Euler euler = new Euler(camera.Orientation);

                  tbYaw.Invoke(new Action(() => tbYaw.Text = euler.Yaw.ValueDegrees.ToString()));
                  tbPitch.Invoke(new Action(() => tbPitch.Text = euler.Pitch.ValueDegrees.ToString()));
                  tbRoll.Invoke(new Action(() => tbRoll.Text = euler.Roll.ValueDegrees.ToString()));

              }*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedObjectSceneNode != null)
            {
                // Vector3 pos = selectedObjectSceneNode.Position;
                float tz1;
                float ty1;
                float tx1;
                bool t2 = float.TryParse(tbPositionZ.Text, out tz1);
                t2 &= float.TryParse(tbPositionX.Text, out tx1);
                t2 &= float.TryParse(tbPositionY.Text, out ty1);
                if (t2)
                {
                    Vector3 vector = new Vector3(tx1, ty1, tz1);
                    selectedObjectSceneNode.Position = vector;
                }
                t2 = float.TryParse(tbPitch.Text, out tz1);
                t2 &= float.TryParse(tbYaw.Text, out tx1);
                t2 &= float.TryParse(tbRoll.Text, out ty1);
                if (t2)
                {
                    selectedObjectSceneNode.Yaw(new Radian(new Degree(tx1)));
                    selectedObjectSceneNode.Pitch(new Radian(new Degree(tz1)));
                    selectedObjectSceneNode.Roll(new Radian(new Degree(ty1)));
                }
            }
            else
            {
                int hh = 1;
                if ((editorScene != null) && (editorScene.camera != null))
                {
                    // Vector3 pos = selectedObjectSceneNode.Position;
                    float tz1;
                    float ty1;
                    float tx1;
                    bool t2 = float.TryParse(tbPositionZ.Text, out tz1);
                    t2 &= float.TryParse(tbPositionX.Text, out tx1);
                    t2 &= float.TryParse(tbPositionY.Text, out ty1);
                    if (t2)
                    {
                        Vector3 vector = new Vector3(tx1, ty1, tz1);
                        editorScene.camera.Position = vector;
                    }
                    // editorScene.camera.
                    t2 = float.TryParse(tbPitch.Text, out tz1);
                    t2 &= float.TryParse(tbYaw.Text, out tx1);
                    t2 &= float.TryParse(tbRoll.Text, out ty1);
                    if (t2)
                    {
                        editorScene.camera.Yaw(new Radian(new Degree(tx1)));
                        editorScene.camera.Pitch(new Radian(new Degree(tz1)));
                        editorScene.camera.Roll(new Radian(new Degree(ty1)));
                    }

                }
            }
        }
    }
}

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

// sinbadNode.Yaw(new Radian(0.01f));
//  sinbadNode.Pitch(new Radian(0.01f));
/*Quaternion t1 = sinbadNode.Orientation;

Matrix3 rotMatrix= t1.ToRotationMatrix();
Radian mYaw;
Radian mPitch;
Radian mRoll;
rotMatrix.ToEulerAnglesYXZ(out mYaw, out mPitch, out mRoll);*/