using System;
using System.Collections.Generic;
using System.Data;
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
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Views.Control {
/// <summary>
/// TreeControl.xaml 的交互逻辑
/// </summary>
public partial class TreeControl : UserControl {


    public TreeControl() {
        InitializeComponent();
    }

    // 构造方法
    public event Action<int> ClickTree;

    List<Node> outputList = new List<Node>();
    /// <summary>
    /// 绑定树
    /// </summary>
    List<Node> Bind(List<Node> nodes) {
        outputList.Clear();
        try {
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i].ParentID == 0 || nodes[i].ParentID==-1) {
                    outputList.Add(nodes[i]);

                } else {
                    Node node = FindDownward(nodes, nodes[i].ParentID);
                    if (node!=null) {
                        node.Nodes.Add(nodes[i]);
                    }
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof(TreeControl), ex);
        }
        return outputList;
    }

    /// <summary>
    /// 递归向下查找
    /// </summary>
    Node FindDownward(List<Node> nodes, int id) {
        try {
            if (nodes == null) {
                return null;
            }
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i].OrganizationId == id) {
                    return nodes[i];
                }
                Node node = FindDownward(nodes[i].Nodes, id);
                if (node != null) {
                    return node;
                }
            }
        } catch (Exception ex) {
            Log.Error(typeof(TreeControl), ex);
        }
        return null;
    }

    /// <summary>
    /// 展开树
    /// </summary>
    public void Expand() {
        try {

            Tree.Items.Refresh();
            Tree.UpdateLayout();
            if (Tree.Items.Count == 0) return;
            Node item = (Node)Tree.Items[0];
            System.Windows.Controls.TreeViewItem treeItem = Tree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
            if (treeItem == null) return;
            treeItem.IsExpanded = true;
        } catch (Exception ex) {
            Log.Error(typeof(TreeControl), ex);
        }
    }
    /// <summary>
    /// 加载树
    /// </summary>
    public void LoadTree() {

        try {
            List<OrganizationTable> list = OrganizationServices.getInstance().FindAllOrganization(null, null);

            List<Node> nodes = new List<Node>();
            for (int i = 0; i < list.Count; i++) {
                OrganizationTable model = list[i];
                Node node = new Node();
                node.Name = model.name;
                node.OrganizationId = model.organizationId.ToInt();
                node.ParentID = model.parentId.ToInt();
                nodes.Add(node);
            }
            // 绑定树
            if (this.Tree.ItemsSource == null) {
                this.Tree.ItemsSource = outputList;
            }

            Bind(nodes);
            Expand();
        } catch (Exception ex) {
            Log.Error(typeof(TreeControl), ex);
        }
    }

    /// <summary>
    /// 画面初始化
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        try {
            LoadTree();
            //List<Node> nodes = new List<Node>()
            //{
            //    new Node { OrganizationId = 1, Name = "中国" },
            //    new Node { OrganizationId = 2, Name = "北京市", ParentID = 1 },
            //    new Node { OrganizationId = 3, Name = "吉林省", ParentID = 1 },
            //    new Node { OrganizationId = 4, Name = "上海市", ParentID = 1 },
            //    new Node { OrganizationId = 5, Name = "海淀区", ParentID = 2 },
            //    new Node { OrganizationId = 6, Name = "朝阳区", ParentID = 2 },
            //    new Node { OrganizationId = 7, Name = "大兴区", ParentID = 2 },
            //    new Node { OrganizationId = 8, Name = "白山市", ParentID = 3 },
            //    new Node { OrganizationId = 9, Name = "长春市", ParentID = 3 },
            //    new Node { OrganizationId = 10, Name = "抚松县", ParentID = 8 },
            //    new Node { OrganizationId = 11, Name = "靖宇县", ParentID = 8 }
            //};
        } catch (Exception ex) {
            Log.Error(typeof(TreeControl), ex);
        }

    }

    /// <summary>
    /// 树选中节点变换
    /// </summary>
    /// <param Name="sender"></param>
    /// <param Name="e"></param>
    private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
        //try {
        //    Node node = e.NewValue as Node;
        //    if (ClickTree != null && e != null) {
        //        ClickTree.Invoke(node.OrganizationId);
        //    }

        //    //MessageBox.Show(a.Name);
        //} catch (Exception ex) {
        //    Log.Error(typeof(TreeControl), ex);
        //}
    }





    private void Tree_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        try {

            Node node = Tree.SelectedValue as Node;
            if (node == null) return;
            if (ClickTree != null && e != null) {
                ClickTree.Invoke(node.OrganizationId);
            }

            //MessageBox.Show(a.Name);
        } catch (Exception ex) {
            Log.Error(typeof(TreeControl), ex);
        }
    }
}

/// <summary>
/// 节点内部类
/// </summary>
public class Node {
    public Node() {
        this.Nodes = new List<Node>();
        this.ParentID = -1;
    }
    public int OrganizationId {
        get;
        set;
    }
    public string Name {
        get;
        set;
    }
    public int ParentID {
        get;
        set;
    }
    public List<Node> Nodes {
        get;
        set;
    }
}

/// <summary>
/// 内部类
/// </summary>
class TreeViewLineConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        TreeViewItem item = (TreeViewItem)value;
        ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
        return ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        return false;
    }
}
}
