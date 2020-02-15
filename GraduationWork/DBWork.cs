using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;


namespace GraduationWork
{
    public class DBWork
    {
        public static MySqlConnection ConnectToDB()
        {
            // Connection string to db
            string connectionString = "server=localhost;user=KerberosUser;database=KerberosDB;password=Password12;";
            MySqlConnection dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            if (dbConnection.State.ToString() != "Open")
                throw new Exception("Data Base connection Error");

            return dbConnection;
        }
        public static void DBInsertRequest(List<string> kerberosRequestList, string fileName, DateTime dateTime, MainWindow mw)
        {
            foreach (string value in kerberosRequestList)
                InsertRecord(value, fileName, dateTime, mw);
        }
        public static void InsertRecord(string requestString, string fileName, DateTime dateTime, MainWindow mw)
        {
            //words = requestString.Split(new char[] { '$', '/', ':' }, StringSplitOptions.RemoveEmptyEntries);

            //Creation of structure which consists of request elements
            DBRequest dbRequest = new DBRequest(requestString);
            RequestInsertion(dbRequest, fileName, dateTime, mw);
        }

        public static void RequestInsertion(DBRequest dbRequest, string fileName, DateTime dateTime, MainWindow mw)
        {
            MySqlConnection dbConnection = ConnectToDB();

            SQLWork.LoadDBData(dbConnection, dbRequest, fileName, dateTime, mw);
        }
    }
}
