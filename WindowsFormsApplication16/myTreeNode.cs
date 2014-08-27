using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication16
{
    public class myTreeNode : TreeNode
    {
        private string type;

        public myTreeNode(string Type, double Tag)
        {
            this.Type = Type;
            this.Tag = Tag;
        }
        public myTreeNode() : base() { }
        public myTreeNode(string x) : base(x) { }
       
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
    }
}
