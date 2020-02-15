using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace GraduationWork
{
    public class SQLWork
    {
        public static void LoadDBData(MySqlConnection dbConnection, DBRequest dbRequest, string fileName, DateTime dateTime, MainWindow mw)
        {
            //Structure for keeping id data from different tables
            IDInformation idInfo = new IDInformation();
            InsertionFunctions.DBBigInsertion(dbConnection, dbRequest, fileName, dateTime, idInfo, mw);
        }
        
        public bool BeforeUploadCheck(MySqlConnection dbConnection, string fileName, DateTime dateTime, MainWindow mw)
        {
            MySqlCommand sqlCommand = dbConnection.CreateCommand();
            string helpDate = InsertionFunctions.ConvertDateTime(dateTime);
            sqlCommand.CommandText = "select * from file where filename = ?fileName and date = ?date;";
            sqlCommand.Parameters.Add("?fileName", MySqlDbType.VarChar).Value = fileName;
            sqlCommand.Parameters.Add("?date", MySqlDbType.DateTime).Value = helpDate;
            if (sqlCommand.ExecuteScalar() != null)
            {
                string message = "File " + fileName +  "with creation date " +dateTime.ToString() + " already exist in the db\n";
                Application.Current.Dispatcher.Invoke(() => 
                {
                    mw.topBox.AppendText(message);
                });
                ///return false;
                return true;

            }
            return true;
        }
    }
}
