using System;
using System.Windows;

//
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;

using System.Data.Linq;
/*

The whole User Instance and AttachDbFileName= approach is flawed - at best! When running your app in Visual Studio, it will be copying around the .mdf file (from your App_Data directory to the output directory - typically .\bin\debug - where you app runs) and most likely, your INSERT works just fine - but you're just looking at the wrong .mdf file in the end!

If you want to stick with this approach, then try putting a breakpoint on the myConnection.Close() call - and then inspect the .mdf file with SQL Server Mgmt Studio Express - I'm almost certain your data is there.

The real solution in my opinion would be to

install SQL Server Express (and you've already done that anyway)

install SQL Server Management Studio Express

create your database in SSMS Express, give it a logical name (e.g. YourDatabase)

connect to it using its logical database name (given when you create it on the server) - and don't mess around with physical database files and user instances. In that case, your connection string would be something like:

Data Source=.\\SQLEXPRESS;Database=YourDatabase;Integrated Security=True

and everything else is exactly the same as before...

*/
namespace GraduationWork
{
    public partial class MainWindow : Window
    {
        private void UpdateDisplayInner(string txt)
        {
            topBox.AppendText(txt + "\r\n");
        }
        string _workSpaceName = "";
        public MainWindow()
        {
            try
            {
                _workSpaceName = "WorkSpace";
                /* MessageBox.Show("Choose WorkSpace file");
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
                folderBrowserDialog.ShowDialog();
                _workSpaceName = folderBrowserDialog.SelectedPath;*/
                if (_workSpaceName == "")
                    throw new Exception("WorkSpace was not chosen");
                InitializeComponent();
                this.Show();

                KerberosAnalyze krbAnalyze = new KerberosAnalyze();

                Thread workThread = new Thread(() => krbAnalyze.kerberosStart(_workSpaceName, this));
                workThread.IsBackground = true;
                workThread.Start();
            }
            catch (Exception e) 
            { 
                MessageBox.Show(e.ToString());
                Environment.Exit(1);
            }
        }
        private void fileOpen()
        {
            OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog.Filter = "All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                // читаем файл в строку
                string fileText = System.IO.File.ReadAllText(filename);
                //textBox1.Text = fileText;
                //textBox.Text = filename;
            }
        }
        private void fileOpen(string filename)
        {
            OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog.FileName = filename;

            if (openFileDialog.ShowDialog() == true)
            {
                string fileText = System.IO.File.ReadAllText(filename);
                //textBox1.Text = fileText;
                //textBox.Text = filename;
            }
        }

        /*public static string getFileName(string fullname)
         {
             return System.IO.Path.GetFileNameWithoutExtension(fullname);
         }*/
        private static void createDbFile(string dbFilePath)
        {

        }
        public static void insertFileToBD(FileInfo fileInfo)
        {/*
            const string Connect = "Database=kerberosDB;Data Source=localhost;User Id=Client1;Password=Password12";
            MySqlConnection con = new MySqlConnection(Connect);
            con.Open(); //Устанавливаем соединение с базой данных.
            MySqlCommand cmd = new MySqlCommand();*/
        }
        // Создать
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //textBox.Text = "";
            //textBox1.Text = "";
        }
        // Открыть
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            fileOpen();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                //System.IO.File.WriteAllText(filename, textBox1.Text);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            fileOpen();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {/*
             Connector data = new Connector();
             Window1 nW = new Window1(this, ref data);
             nW.ShowDialog();
             if (data.changed == true)
             {
                 fileOpen(data.filename);
                 int countPacket = countWord(textBox1.Text, "<packet>");
                 lblCursorPosition.Text = "Всего пакетов " + countPacket.ToString();
             }
             */
        }
       // public string GetTextBoxText()
        //{
            //return textBox.Text;
        //}

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            /*Connector data = new Connector();
            //Window2 nW = new Window2(this, ref data);
            nW.ShowDialog();
            if (data.changed == true)
            {
                fileOpen(data.filename);
                int countPacket = countWord(textBox1.Text, "<packet>");
                lblCursorPosition.Text = "Всего пакетов " + countPacket.ToString();
            }*/
        }
    }
}
