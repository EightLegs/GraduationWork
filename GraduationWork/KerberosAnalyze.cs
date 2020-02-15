using System;
using System.Windows;

//\
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Diagnostics;
using LinqToDB.Data;

using System.Linq;
using System.Data.SqlClient;

using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace GraduationWork
{
    public class KerberosAnalyze
    {
        public  void kerberosStart(string workSpaceName, MainWindow mw)
        {
            while (true)
            {
                if (Directory.Exists(workSpaceName) == true)
                {
                    DirectoryInfo my = new DirectoryInfo(workSpaceName);
                    foreach (FileInfo fileInfo in my.GetFiles())
                    {
                        MySqlConnection dbConnection = DBWork.ConnectToDB();
                        SQLWork sqlWork = new SQLWork();
                        //Check file existence in db before working with chosen file
                        if (sqlWork.BeforeUploadCheck(dbConnection, System.IO.Path.GetFileName(fileInfo.FullName), fileInfo.CreationTime, mw))
                            uploadFile(fileInfo, mw);
                    }
                }
                else
                    throw new Exception("WorkSpace doesn`t exist");
            }
        }
        public static int countWord(string source, string search)
        {
            string pattern = $"{Regex.Escape(search)}";
            return new Regex(pattern, RegexOptions.IgnoreCase).Matches(source).Count;
        }
        public static int filterTrafficFile(FileInfo fileInfo, string helpFileName)
        {
            //В данной функции файл с трафиком преобразуется в pdml формат, содержащий только файлы протокола Kerberos
            string helpStr = "tshark -2 -r " + '\"' + fileInfo.FullName + '\"' + " -R \"tcp.dstport==88 or udp.dstport==88 or tcp.srcport==88 or udp.srcport==88\" -T pdml >> " + '\"' + helpFileName + '\"';
            ProcessStartInfo psi = new ProcessStartInfo("cmd", @"/c " + helpStr);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;
            psi.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = psi;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();

            string fileText = File.ReadAllText(helpFileName);
            return countWord(fileText, "<packet>");
        }
        public static List<string> makeKerberosRequest(string helpFileName)
        {
            string resultFile = "data.pdml";
            //var stream = File.Create(resultFile);

            // Данная функция формирует заявки на расшифрование паролей из пакетов протокола Kerberos и возвращет их
            string helpStr = "py " + "krb2john.py "  + helpFileName  +  " > " + resultFile; /* +" >> " + '\"' + filename + '\"'*/
            ProcessStartInfo psi;
            psi = new ProcessStartInfo("cmd", @"/c " + helpStr);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;
            psi.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = psi;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();

            List<string> requestList = new List<string>();

            StreamReader f = new StreamReader(resultFile);
            while (!f.EndOfStream)
                requestList.Add(f.ReadLine());
            f.Close();
            File.Delete(resultFile);
            return requestList;
        }
        public static void uploadFile(FileInfo fileInfo, MainWindow mw)
        {
            string fileText = System.IO.File.ReadAllText(fileInfo.FullName);

            string helpFileName = "helpFile.txt";
            var stream = File.Create(helpFileName);
            stream.Close();
            int countKerberosPacket = filterTrafficFile(fileInfo, helpFileName);
            
            // List creation of all requests in chosen file
            List<string> kerberosRequestList = makeKerberosRequest(helpFileName);
            string message = "File " + System.IO.Path.GetFileName(fileInfo.FullName) + " with creation date " + fileInfo.CreationTime.ToString() + " with "  + kerberosRequestList.Count.ToString() + " requests\n";
            Application.Current.Dispatcher.Invoke(() =>
            {
                mw.topBox.AppendText(message);
            });

            //int countKerberosRequest = countWord(kerberosRequestString, "$krb5");
            File.Delete(helpFileName);

            //Request list insertion into db
            DBWork.DBInsertRequest(kerberosRequestList,System.IO.Path.GetFileName(fileInfo.FullName), fileInfo.CreationTime, mw);
        }
    }
}