using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AppConfig;

namespace TestApp
{
    
    public partial class MainForm : Form
    {
        public static Configuration config = new Configuration();

        private void InitConfig()
        {
            config.AppName = Application.ProductName;
            config.InitAttributes.Add("color", new AttributeInfo("red"));
            config.InitAttributes.Add("pages", new AttributeInfo("5"));
            config.LoadConfig();
        }

        private void Trace(string str)
        {
            DebugBox.Text += Environment.NewLine + str; 
        }

        private void TraceValue(string key)
        {
            DebugBox.Text += Environment.NewLine; 
            DebugBox.Text += String.Format("{0} : {1}", key, config.Get(key));
        }

        private void TraceValue(string action, string key)
        {
            DebugBox.Text += Environment.NewLine;
            DebugBox.Text += String.Format("{0} : ( {1} : {2} )", action, key, config.Get(key));
        }

        public MainForm()
        {
            InitializeComponent();
            InitConfig();
            Trace("start");
            TraceValue("color");
            TraceValue("pages");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            config.Set("new", "asd");
          
            TraceValue("add", "new");
            config.Flush();
            TraceValue("add", "new");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            config.Set("color", "blue");

            TraceValue("change", "color");
            config.Flush();
            TraceValue("change", "color");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            config.InitAttributes.Add("init", new AttributeInfo("1"));
            //error
            //TraceValue("add", "init");
            config.Flush();
            TraceValue("add", "init");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                TraceValue(textBox1.Text);
            }
            catch
            {
                Trace("Error");
            }
        }
    }
}
