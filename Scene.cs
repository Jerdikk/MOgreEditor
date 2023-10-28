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
                    editorScene.treeView1.Nodes.Clear();
                    editorScene.sceneManager.ClearScene();
                    editorScene.camera = new Camera(scene.cameraName, editorScene.sceneManager);
                    editorScene.camera.Position = scene.cameraPosition;
                    editorScene.camera.Orientation = scene.cameraOrientation;
                    editorScene.camera.Direction = scene.cameraLookAt;
                    foreach(SaveSceneNode saveSceneNode in scene.nodes) 
                    {
                        EditorSceneNode editorSceneNode = new EditorSceneNode();
                        SceneNode sceneNode  = editorScene.AddEditorSceneNode(saveSceneNode.name, saveSceneNode.meshName);
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
        public SceneNode AddEditorSceneNode(string nodeName, string v)
        {
            try
            {
                if (v != "Camera")
                {
                    Entity entity = sceneManager.CreateEntity(v);

                    SceneNode sceneNode = rootSceneNode.CreateChildSceneNode(nodeName);
                    sceneNode.AttachObject(entity);
                    EditorSceneNode editorSceneNode = new EditorSceneNode();
                    editorSceneNode.name = nodeName;
                    editorSceneNode.meshName = v;
                    editorSceneNode.entity = entity;
                    editorSceneNode.sceneNode = sceneNode;
                    editorSceneNode.treeNode = new TreeNode(nodeName);
                    this.children.AddLast(editorSceneNode);
                    rootNode.Nodes.Add(editorSceneNode.treeNode);

                    return sceneNode;
                }
                else
                {
                    EditorSceneNode editorSceneNode = new EditorSceneNode();
                    editorSceneNode.name = nodeName;
                    editorSceneNode.meshName = "Camera";
                    editorSceneNode.entity = null;
                    this.camera = sceneManager.GetCamera(nodeName);
                    editorSceneNode.sceneNode = null;
                    editorSceneNode.treeNode = new TreeNode(nodeName);
                    // this.children.AddLast(editorSceneNode);
                    rootNode.Nodes.Add(editorSceneNode.treeNode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}
