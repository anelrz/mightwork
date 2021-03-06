﻿using Agilent.Agilent33220.Interop;
using Agilent.AgilentE36xx.Interop;
using Ivi.Driver.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication16
{
    public partial class Form1 : Form
    {
        AgilentE36xx E3641A;
        Agilent33220 driver; 
        public string seba;
        public myXML myxml;
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.InitialDirectory = Application.StartupPath;
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ",";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addfixedpreset(null);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level > 0)
            {
                textBox2.Text = e.Node.Tag.ToString();
                Console.WriteLine(e.Node.Tag);
                Console.WriteLine(" Level: " + e.Node.Level);
            }
            else
            {
                Console.WriteLine("NULL");
                Console.WriteLine(" Level: " + e.Node.Level);
            }
        }

        public void addpreset(string name, double frequency, double amplitude, double offset)
        {
            if (name == null)
                name = textBox2.Text;
            myTreeNode root_node = new myTreeNode(name);

            myTreeNode myNode1 = new myTreeNode("Frequency", frequency);
            ppreset(myNode1, root_node);

            myTreeNode myNode2 = new myTreeNode("Amplitude", amplitude);
            ppreset(myNode2, root_node);

            myTreeNode myNode3 = new myTreeNode("Offset", offset);
            ppreset(myNode3, root_node);

            treeView1.Nodes.Add(root_node);
        }

        private void addfixedpreset(string name)
        {
            if (name == null)
                name = textBox2.Text;
            myTreeNode root_node = new myTreeNode(name);

            myTreeNode myNode1 = new myTreeNode("Frequency", 1000);
            ppreset(myNode1, root_node);

            myTreeNode myNode2 = new myTreeNode("Amplitude", 1);
            ppreset(myNode2, root_node);

            myTreeNode myNode3 = new myTreeNode("Offset", 0);
            ppreset(myNode3, root_node);

            treeView1.Nodes.Add(root_node);
        }
        private void ppreset(myTreeNode mytreenode, myTreeNode root)
        {
            mytreenode.Text = mytreenode.Type + ": " + mytreenode.Tag.ToString();
            root.Nodes.Add(mytreenode);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (treeView1.SelectedNode.Level > 0)
                {
                    treeView1.SelectedNode.Tag = textBox2.Text;
                    treeView1.SelectedNode.ForeColor = Color.Red;
                    treeView1.SelectedNode.Text = ((myTreeNode)treeView1.SelectedNode).Type + ": " + treeView1.SelectedNode.Tag.ToString();
                    if (treeView1.SelectedNode.NextNode != null)
                        treeView1.SelectedNode = treeView1.SelectedNode.NextNode;
                }
                else
                {
                    treeView1.SelectedNode.Tag = textBox2.Text;
                    treeView1.SelectedNode.Text = treeView1.SelectedNode.Tag.ToString();
                    if (treeView1.SelectedNode.NextNode != null)
                        treeView1.SelectedNode = treeView1.SelectedNode.NextNode;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myxml = new myXML(this.treeView1, this);
            driver = new Agilent33220();
            E3641A = new AgilentE36xx();
            
        }


        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (treeView1.SelectedNode != null)
                {
                    Console.WriteLine("delete");
                    treeView1.SelectedNode.Remove();
                    
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {      
            if (e.Node.Level > 0)
            {
                textBox2.Focus();
                textBox2.Text = e.Node.Tag.ToString();
                textBox2.SelectAll();
            }
            else
            {
            Console.WriteLine("treenode selected");

                if (driver.Initialized != false || E3641A.Initialized != false)
                {
                    try
                    {
                        if(Convert.ToDouble(e.Node.Nodes[0].Tag) != 0 || E3641A.Initialized == false)
                        {
                            driver.Output.Voltage.Units = Agilent33220OutputVoltageUnitEnum.Agilent33220OutputVoltageUnitVrms;
                            driver.Apply.SetSinusoid(Convert.ToDouble(e.Node.Nodes[0].Tag), Convert.ToDouble(e.Node.Nodes[1].Tag), Convert.ToDouble(e.Node.Nodes[2].Tag) - Convert.ToDouble(textBox3.Text));
                            driver.System.Beeper();
                            comboBox1.SelectedIndex = 0;
                            textBox2.Text = "Output ON";
                        }
                        else
                        {
                            AgilentE36xxOutput output = E3641A.Outputs.Item["Output1"];
                            
                            //E3641A.Display.Clear();
                            output.VoltageLevel = Math.Abs(Convert.ToDouble(e.Node.Nodes[2].Tag));
                            E3641A.Outputs.Enabled = true;
                        }
                    }
                    catch { }
                }
            }
        }

        private void treeView1_Enter(object sender, EventArgs e)
        {
            if(treeView1.SelectedNode != null && treeView1.SelectedNode.Tag != null)
                textBox2.Focus();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                if ((((ComboBox)sender).SelectedItem.ToString() == "ON"))
                {
                    if (driver.Initialized == false)
                        driver.Initialize(textBox1.Text, true, true, "Simulate=false, DriverSetup= Model=33220A");
                        //NOT TESTED !!!
                        //driver.Initialize("usb0::2391::1031::MY44039785::INSTR", true, true, "Simulate=false, DriverSetup= Model=33220A");
                    driver.Output.State = true;
                    textBox2.Text = "Output ON";
                    driver.System.EnableLocalControls();
                }
                else
                {
                    driver.Output.State = false;
                    textBox2.Text = "Output OFF";
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                textBox2.Text = "Connection failed";
            }
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox co = (ComboBox)sender;
            if (driver.Initialized)
            {
                switch (co.SelectedIndex)
                {
                    case 0:
                        driver.Output.SetLoadInfinity();
                        driver.System.Beeper();
                        break;
                    case 1:
                        driver.Output.SetLoadMin();
                        driver.System.Beeper();
                        break;
                    case 2:
                        driver.Output.SetLoadMax();
                        driver.System.Beeper();
                        break;
                    default:
                        break;
                }
            }
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            ComboBox co = (ComboBox)sender;
            try
            {
                double d = Convert.ToDouble(co.Text);
                driver.Output.Load = d;
                driver.System.Beeper();
            }
            catch(Exception e3)
            {
                Console.WriteLine(e3.Message);
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //myxml.save(openFileDialog1.FileName);
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Node Double CLicked");
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Hello");
                Convert.ToDouble(textBox3.Text);
                errorProvider1.SetError(textBox3, String.Empty);
            }
            catch (FormatException)
            {
                errorProvider1.SetError(textBox3, "Not Valid number");
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox4.Text = openFileDialog1.SafeFileName;
            myxml.load(openFileDialog1.FileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                myxml.save(openFileDialog1.FileName);
            }
            catch { }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox CB = (CheckBox) sender;
            if(CB.CheckState == CheckState.Checked)
            {
                Console.WriteLine("checked");
                try
                {
                    
                    E3641A.Initialize("ASRL" + numericUpDown1.Value.ToString() + "::INSTR", true, true, "Cache=true");
                        if (E3641A.Initialized)
                        {
                            Console.WriteLine("initialized");
                            E3641A.Outputs.Item["Output1"].CurrentLimit = 0.08;
                            //E3641A.Display.Text = "I'm ready";
                        }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    checkBox1.CheckState = CheckState.Unchecked;
                }
            }
            else
            {
                Console.WriteLine("un-checked");
                if (E3641A.Initialized)
                {
                    Console.WriteLine("Connection closed");
                    E3641A.Display.Text = "Good bye";
                    E3641A.Close();
                }
                else
                {
                    Console.WriteLine("Close skiped because connection not initialized");
                }
                
            }
        }
    }
}
