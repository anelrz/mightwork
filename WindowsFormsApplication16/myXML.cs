using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApplication16
{
    public partial class myXML
    {
        private TreeView treeView1;
        private Form1 f;
        public XmlDocument xDoc;
        //private string xmlfile = "books.xml";

        public myXML(TreeView tv, Form1 f)
        {
            this.treeView1 = tv;
            this.f = f;
        }
        public void save(string xmlfile)
        {
            Console.WriteLine(xDoc.DocumentElement.InnerText);
            XmlElement xl = xDoc.DocumentElement;
            xl.RemoveAll();
            foreach (TreeNode tn in treeView1.Nodes)
            {
                XmlElement elem = xDoc.CreateElement("Preset");

                XmlElement elem0 = xDoc.CreateElement("name");
                XmlElement elem1 = xDoc.CreateElement("frequency");
                XmlElement elem2 = xDoc.CreateElement("amplitude");
                XmlElement elem3 = xDoc.CreateElement("offset");

                string s = tn.Name;
                XmlText text0 = xDoc.CreateTextNode(Convert.ToString(tn.Text));
                XmlText text1 = xDoc.CreateTextNode(Convert.ToString(tn.Nodes[0].Tag));
                XmlText text2 = xDoc.CreateTextNode(Convert.ToString(tn.Nodes[1].Tag));
                XmlText text3 = xDoc.CreateTextNode(Convert.ToString(tn.Nodes[2].Tag));

                elem0.AppendChild(text0);
                elem1.AppendChild(text1);
                elem2.AppendChild(text2);
                elem3.AppendChild(text3);

                elem.AppendChild(elem0);
                elem.AppendChild(elem1);
                elem.AppendChild(elem2);
                elem.AppendChild(elem3);

                xl.AppendChild(elem);
            }
            xDoc.Save(xmlfile);

           // treeView1.Nodes
        }

        public void load(string xmlfile)
        {
            try
            {
                xDoc = new XmlDocument();
                xDoc.Load(xmlfile);
                
                XmlNodeList xl = xDoc.ChildNodes.Item(1).ChildNodes;
                
                foreach (XmlNode node in xl)
                {
                    //string s = node["name"].InnerText;
                    f.addpreset(node["name"].InnerText, Convert.ToDouble(node["frequency"].InnerText), Convert.ToDouble(node["amplitude"].InnerText), Convert.ToDouble(node["offset"].InnerText));
                 
                }
                
            }
            catch (XmlException ex)
            {
                
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
            }
        }
    }
}

