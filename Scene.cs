using Mogre;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MOgreEditor
{
    [Serializable]
    public class SaveSceneNode
    {
        public string name;
        public string meshName;
        public string objType;
        public Vector3 position;
        public Quaternion orientation;
        public SaveSceneNode() { }
    }
    [Serializable]
    public class SaveScene
    {
        public string cameraName;
        public Vector3 cameraPosition;
        public Vector3 cameraLookAt;
        public Quaternion cameraOrientation;
        public List<SaveSceneNode> nodes = new List<SaveSceneNode>();
        public SaveScene()
        {

        }
        public static bool ReadScene(string name, EditorScene editorScene)
        {
            try
            {
                SaveScene scene;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveScene));
                using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate))
                {
                    scene = (SaveScene)xmlSerializer.Deserialize(fs);
                }

                if (scene != null)
                {
                    editorScene.children.Clear();                    
                    editorScene.sceneManager.ClearScene();
                    editorScene.camera = new Camera(scene.cameraName, editorScene.sceneManager);

                    editorScene.camera.Position = scene.cameraPosition;
                    editorScene.camera.Orientation = scene.cameraOrientation;
                    editorScene.camera.Direction = scene.cameraLookAt;
                    editorScene.AddEditorSceneNode(scene.cameraName, "Camera", "Camera");
                    foreach (SaveSceneNode saveSceneNode in scene.nodes) 
                    {
                        EditorSceneNode editorSceneNode = new EditorSceneNode();
                        SceneNode sceneNode  = editorScene.AddEditorSceneNode(saveSceneNode.name, saveSceneNode.meshName,saveSceneNode.objType);
                        sceneNode.Position = saveSceneNode.position;
                        sceneNode.Orientation = saveSceneNode.orientation;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static void WriteScene(string name, EditorScene scene)
        {
            SaveScene saveScene;
            try
            {
                saveScene = new SaveScene();
                saveScene.cameraName = scene.camera.Name;
                saveScene.cameraPosition = scene.camera.Position;
                saveScene.cameraOrientation = scene.camera.Orientation;
                saveScene.cameraLookAt = scene.camera.Direction;

                foreach (EditorSceneNode node in scene.children)
                {
                    SaveSceneNode saveSceneNode = new SaveSceneNode();
                    saveSceneNode.name = node.name;
                    saveSceneNode.objType = node.objType;
                    saveSceneNode.meshName = node.meshName;
                    saveSceneNode.orientation = node.sceneNode.Orientation;
                    saveSceneNode.position = node.sceneNode.Position;
                    saveScene.nodes.Add(saveSceneNode);
                }

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveScene));
                // получаем поток, куда будем записывать сериализованный объект
                using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(fs, saveScene);
                }
            }
            catch { }
        }
    }

    public class EditorSceneNode
    {

        public Entity entity;
        public string name;
        public string objType;
        public string meshName;
        public SceneNode sceneNode;
        public TreeNode treeNode;

        public LinkedList<EditorSceneNode> children;

        public EditorSceneNode()
        {
            children = null;
        }
    }
    public class EditorScene
    {
        public LinkedList<EditorSceneNode> children;
        public TreeView treeView1;
        public TreeNode rootNode;
        public SceneNode rootSceneNode;
        public SceneManager sceneManager;
        public Camera camera;

        public EditorScene(TreeView treeView, SceneManager sceneManager)
        {
            treeView1 = treeView;
            children = new LinkedList<EditorSceneNode>();
            rootNode = new TreeNode("Root");
            // Добавляем rootNode вместе с дочерними узлами в TreeView
            treeView1.Nodes.Add(rootNode);
            this.rootSceneNode = sceneManager.RootSceneNode;
            this.sceneManager = sceneManager;
        }
        public void Dispose()
        {
            foreach(EditorSceneNode node in children)
            {
                node.entity.Dispose();
                node.entity = null;
                node.sceneNode.Dispose();
                node.sceneNode = null;
                node.treeNode.Remove();
                node.treeNode = null;   
            }
            children.Clear();
            camera.Dispose();
            camera = null;
        }
        public SceneNode GetEditorSceneNodeByName(string nodeName)
        {
            try
            {
                foreach (EditorSceneNode node in children)
                {
                    if (node.name == nodeName)
                    {
                        return node.sceneNode;
                    }
                }
            }
            catch { }
            return null;
        }
        public SceneNode AddEditorSceneNode(string nodeName, string v, string objType)
        {
            try
            {
                if (objType == "Entity")
                {
                    Entity entity = sceneManager.CreateEntity(v);

                    SceneNode sceneNode = rootSceneNode.CreateChildSceneNode(nodeName);
                    sceneNode.AttachObject(entity);
                    EditorSceneNode editorSceneNode = new EditorSceneNode();
                    editorSceneNode.name = nodeName;
                    editorSceneNode.meshName = v;
                    editorSceneNode.objType = objType;
                    editorSceneNode.entity = entity;
                    editorSceneNode.sceneNode = sceneNode;
                    editorSceneNode.treeNode = new TreeNode(nodeName);
                    this.children.AddLast(editorSceneNode);
                    rootNode.Nodes.Add(editorSceneNode.treeNode);

                    return sceneNode;
                }
                if (objType == "Camera")
                {
                    EditorSceneNode editorSceneNode = new EditorSceneNode();
                    editorSceneNode.name = nodeName;
                    editorSceneNode.meshName = "Camera";
                    editorSceneNode.objType = objType;
                    editorSceneNode.entity = null;
                    this.camera = sceneManager.GetCamera(nodeName);
                    editorSceneNode.sceneNode = null;
                    editorSceneNode.treeNode = new TreeNode(nodeName);
                    // this.children.AddLast(editorSceneNode);
                    rootNode.Nodes.Add(editorSceneNode.treeNode);
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*Plane plane = new Plane(new Vector3(0, 1, 0), 0);

        MeshManager.getSingleton().createPlane("ground", "General", plane, 2560, 2560, 20, 20, true, 1, 5, 5, new Vector3(0, 0, 1));
            Entity ent = scnMgr.createEntity("GroundEntity", "ground");
        SceneNode node = scnMgr.getRootSceneNode().createChildSceneNode("GroundNode", new Vector3(640.0f, 0.0f, 640.0f));
        node.attachObject(ent);
            ent.setMaterialName("Examples/RustySteel");*/
    }

}
