using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Resources;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace CalApp
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

            label2.Text = "Author: Christoph Ziegler";
            AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
            label3.Text = copyright.Copyright;
            label4.Text = "Version: " + Application.ProductVersion;
            linkLabel1.Text = "Contact";
            groupBox1.Text = Application.ProductName;
            textBox1.Text = Resource.Information;
            textBox1.DeselectAll();
            groupBox2.Text = "Licence Information";
            textBox2.Text = Resource.Licence;// "This program is free to use within the Infineon Technologies AG company.";
            textBox2.DeselectAll();
        }

        //public static string Title
        //{
        //    get { var assm = Assembly.GetExecutingAssembly(); };
        //}

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:christophzieglerapps@outlook.com?subject=Calendar%20Week%20Info");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            textBox2.DeselectAll();
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
            button1.Left = (this.Width / 2 - button1.Width / 2);
            textBox2.DeselectAll();
        }

        private void Button1_SizeChanged(object sender, EventArgs e)
        {
            //button1.Left = (this.Width / 2 - button1.Width / 2);
        }
    }
}
