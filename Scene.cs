using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MOgreEditor
{
    [Serializable]
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
        public SceneNode AddEditorSceneNode(string nodeName, string v)
        {
            try
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
            catch (Exception ex) 
            {
                return null;
            }
        }
    }

}
