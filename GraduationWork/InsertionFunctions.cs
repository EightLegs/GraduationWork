using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using MySql.Data.MySqlClient;
namespace GraduationWork
{
    class InsertionFunctions
    {
        public static string ConvertDateTime(DateTime dateTime)
        {// '2020-01-08 11:26:59', 'Example.pcapng'
            return dateTime.Year.ToString() + '-' + dateTime.Month.ToString() + '-' + dateTime.Day.ToString() + ' ' + dateTime.Hour.ToString() + ':' + dateTime.Minute.ToString() + ':' + dateTime.Second.ToString();
        }
        public static void DBFileInsertion(MySqlConnection dbConnection, string fileName, DateTime dateTime, IDInformation idInfo, MainWindow mw)
        {
            string fileID = null;
            MySqlCommand sqlCommand = dbConnection.CreateCommand();
            string helpDate = ConvertDateTime(dateTime);
            sqlCommand.CommandText = "select * from file where filename = ?fileName and date = ?date;";
            sqlCommand.Parameters.Add("?fileName", MySqlDbType.VarChar).Value = fileName;
            sqlCommand.Parameters.Add("?date", MySqlDbType.DateTime).Value = helpDate;
            // This file already exists in db
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                fileID = reader.GetString(0);
                reader.Close();
                idInfo.SetFileID(fileID);
                /*                string message = "File " + fileName + "already exists in db\n";
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    mw.topBox.AppendText(message);
                                });*/
                return;
            }
            reader.Close();

            sqlCommand.CommandText = "insert into file (date,fileName) values (?date,?fileName);";
            reader = sqlCommand.ExecuteReader();
            //Records affected amount after insertion
            if (reader.RecordsAffected == 0)
            {
                string message = "#Error.File could not be inserted into db. File " + fileName + " \n";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mw.topBox.AppendText(message);
                });
                return;
            }
            reader.Close();
            sqlCommand.CommandText = "select last_insert_id();";
            reader = sqlCommand.ExecuteReader();
            reader.Read();
            fileID = reader.GetString(0);
            idInfo.SetFileID(fileID);
            reader.Close();
        }
        public static void DBCipherInsertion(MySqlConnection dbConnection, DBRequest dbRequest, string fileName, DateTime dateTime, IDInformation idInfo, MainWindow mw)
        {
            string cipherID = null;
            MySqlCommand sqlCommand = dbConnection.CreateCommand();
            sqlCommand.CommandText = "insert into cipher (etypeID,cipherContent,salt) values (?etypeID,?cipherContent,?salt);";
            sqlCommand.Parameters.Add("?etypeID", MySqlDbType.Int32).Value = dbRequest.GetEType();
            sqlCommand.Parameters.Add("?cipherContent", MySqlDbType.Text).Value = dbRequest.GetRequest();
            sqlCommand.Parameters.Add("?salt", MySqlDbType.VarChar).Value = dbRequest.GetSalt();
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            //count will be the number of rows updated. will be zero if no rows updated.
            if (reader.RecordsAffected == 0)
            {
                reader.Close();
                string message = "#Error.Cipher from " + fileName + "could not be inserted into db\n";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mw.topBox.AppendText(message);
                });
                return;
            }
            reader.Close();
            //Taking realmID from existed row
            sqlCommand.CommandText = "select last_insert_id();";
            reader = sqlCommand.ExecuteReader();
            reader.Read();
            cipherID = reader.GetString(0);
            idInfo.SetCipherID(cipherID);
            reader.Close();
        }
        public static void DBRealmInsertion(MySqlConnection dbConnection, DBRequest dbRequest, string fileName, DateTime dateTime, IDInformation idInfo, MainWindow mw)
        {
            string realmID = null;
            //Row with applied realmName existence check
            MySqlCommand sqlCommand = dbConnection.CreateCommand();
            sqlCommand.CommandText = "select * from realm where realmName =(?realmName);";
            sqlCommand.Parameters.Add("?realmName", MySqlDbType.VarChar).Value = dbRequest.GetRealm();
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                realmID = reader.GetString(0);
                idInfo.SetRealmID(realmID);
                reader.Close();
                return;
            }
            reader.Close();

            sqlCommand.CommandText = "select * from realm where realmName =(?realmName);";
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.Add("?realmName", MySqlDbType.VarChar).Value = dbRequest.GetHostName() + '.' +dbRequest.GetRealm();
            reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                realmID = reader.GetString(0);
                idInfo.SetRealmID(realmID);
                reader.Close();
                return;
            }
            reader.Close();

            //Realm insertion
            sqlCommand.CommandText = "insert into realm (realmName) values (?realmName);";
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.Add("?realmName", MySqlDbType.VarChar).Value = dbRequest.GetRealm();
            reader = sqlCommand.ExecuteReader();
            //count will be the number of rows updated. will be zero if no rows updated.
            if (reader.RecordsAffected == 0)
            {
                reader.Close();
                string message = "#Error.Realm " + dbRequest.GetRealm() +  "from file " + fileName + "could not be inserted into db\n";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mw.topBox.AppendText(message);
                });
                return;
            }
            reader.Close();
            //Taking realmID from existed row
            sqlCommand.CommandText = "select last_insert_id();";
            reader = sqlCommand.ExecuteReader();
            reader.Read();
            realmID = reader.GetString(0);
            idInfo.SetRealmID(realmID);
            reader.Close();
        }
        public static void DBUserInsertion(MySqlConnection dbConnection, DBRequest dbRequest, string fileName, DateTime dateTime, IDInformation idInfo, MainWindow mw)
        {
            string userID = null;
            //Row with applied realmName existence check
            MySqlCommand sqlCommand = dbConnection.CreateCommand();
            sqlCommand.CommandText = "select * from user where userName =(?userName) and realmID = (?realmID);";
            sqlCommand.Parameters.Add("?userName", MySqlDbType.VarChar).Value = dbRequest.GetUser();
            sqlCommand.Parameters.Add("?realmID", MySqlDbType.Int32).Value = Convert.ToInt32(idInfo.GetRealmID());
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                userID = reader.GetString(0);
                idInfo.SetUserID(userID);
                reader.Close();
                return;
            }
            reader.Close();

            //Realm insertion
            sqlCommand.CommandText = "insert into user (userName, realmID) values (?userName, ?realmID);";
            reader = sqlCommand.ExecuteReader();
            //count will be the number of rows updated. will be zero if no rows updated.
            if (reader.RecordsAffected == 0)
            {
                reader.Close();
                string message = "#Error.User " + dbRequest.GetUser() + "from file " + fileName + "could not be inserted into db\n";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mw.topBox.AppendText(message);
                });
                return;
            }
            reader.Close();
            //Taking realmID from existed row
            sqlCommand.CommandText = "select last_insert_id();";
            reader = sqlCommand.ExecuteReader();
            reader.Read();
            userID = reader.GetString(0);
            idInfo.SetUserID(userID);
            reader.Close();
        }
        public static void DBHostInsertion(MySqlConnection dbConnection, DBRequest dbRequest, string fileName, DateTime dateTime, IDInformation idInfo, MainWindow mw)
        {
            string hostID = null;
            //Row with applied realmName existence check
            MySqlCommand sqlCommand = dbConnection.CreateCommand();
            sqlCommand.CommandText = "select * from host where hostname =(?hostname) and realmID = (?realmID);";
            sqlCommand.Parameters.Add("?hostname", MySqlDbType.VarChar).Value = dbRequest.GetHostName();
            sqlCommand.Parameters.Add("?realmID", MySqlDbType.Int32).Value = Convert.ToInt32(idInfo.GetRealmID());
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                hostID = reader.GetString(0);
                idInfo.SetHostID(hostID);
                reader.Close();
                return;
            }
            reader.Close();

            //Realm insertion
            sqlCommand.CommandText = "insert into host (hostname, realmID) values (?hostname, ?realmID);";
            reader = sqlCommand.ExecuteReader();
            //count will be the number of rows updated. will be zero if no rows updated.
            if (reader.RecordsAffected == 0)
            {
                reader.Close();
                string message = "#Error.Hostname " + dbRequest.GetHostName() + "from file " + fileName + "could not be inserted into db\n";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mw.topBox.AppendText(message);
                });
                return;
            }
            reader.Close();
            //Taking realmID from existed row
            sqlCommand.CommandText = "select last_insert_id();";
            reader = sqlCommand.ExecuteReader();
            reader.Read();
            hostID = reader.GetString(0);
            idInfo.SetHostID(hostID);
            reader.Close();
        }
        public static void DBServiceInsertion(MySqlConnection dbConnection, DBRequest dbRequest, string fileName, DateTime dateTime, IDInformation idInfo, MainWindow mw)
        {
            string serviceID = null;
            //Row with applied realmName existence check
            MySqlCommand sqlCommand = dbConnection.CreateCommand();
            if(idInfo.GetHostID() == null)
            {
                sqlCommand.CommandText = "select * from service where serviceName =(?serviceName);";
                sqlCommand.Parameters.Add("?serviceName", MySqlDbType.VarChar).Value = dbRequest.GetServiceClass();
            }
            else
            {
                sqlCommand.CommandText = "select * from service where serviceName =(?serviceName) and hostID= (?hostID);";
                sqlCommand.Parameters.Add("?serviceName", MySqlDbType.VarChar).Value = dbRequest.GetServiceClass();
                sqlCommand.Parameters.Add("?hostID", MySqlDbType.Int32).Value = Convert.ToInt32(idInfo.GetHostID());
            }
            
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                serviceID = reader.GetString(0);
                reader.Close();
                return;
            }
            reader.Close();

            //Realm insertion
            if (idInfo.GetHostID() == null)
                sqlCommand.CommandText = "insert into service (serviceName) values (?serviceName);";
            else
                sqlCommand.CommandText = "insert into service (serviceName, hostID) values (?serviceName, ?hostID);";
            reader = sqlCommand.ExecuteReader();
            //count will be the number of rows updated. will be zero if no rows updated.
            if (reader.RecordsAffected == 0)
            {
                reader.Close();
                string message = "#Error.Service " + dbRequest.GetServiceClass() + "from file " + fileName + "could not be inserted into db\n";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mw.topBox.AppendText(message);
                });
                return;
            }
            reader.Close();
            //Taking realmID from existed row
            sqlCommand.CommandText = "select last_insert_id();";
            reader = sqlCommand.ExecuteReader();
            reader.Read();
            serviceID = reader.GetString(0);
            reader.Close();
        }
        public static void TakePacketID(MySqlConnection dbConnection, DBRequest dbRequest, IDInformation idInfo)
        {
            string packetID = null;
            MySqlCommand sqlCommand = dbConnection.CreateCommand();
            sqlCommand.CommandText = "select packetID from packettype where packetName = (?packetName);";
            sqlCommand.Parameters.Add("?packetName", MySqlDbType.VarChar).Value = dbRequest.GetPacketName();

            MySqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                packetID = reader.GetString(0);
                idInfo.SetPacketID(packetID);
                reader.Close();
                return;
            }
            reader.Close();
        }
        public static void DBRequestInsertion(MySqlConnection dbConnection, DBRequest dbRequest, string fileName, DateTime dateTime, IDInformation idInfo, MainWindow mw)
        {
            string requestID = null;
            MySqlCommand sqlCommand = dbConnection.CreateCommand();
            sqlCommand.CommandText = "insert into request (packetID,userID,cipherID,fileID,serviceID) values (?packetID,?userID,?cipherID,?fileID,?serviceID);";
            sqlCommand.Parameters.Add("?packetID", MySqlDbType.Int32).Value = idInfo.GetPacketID();
            sqlCommand.Parameters.Add("?userID", MySqlDbType.Int32).Value = idInfo.GetUserID();
            sqlCommand.Parameters.Add("?cipherID", MySqlDbType.Int32).Value = idInfo.GetCipherID();
            sqlCommand.Parameters.Add("?fileID", MySqlDbType.Int32).Value = idInfo.GetFileID();
            sqlCommand.Parameters.Add("?serviceID", MySqlDbType.Int32).Value = idInfo.GetServiceID();
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            //count will be the number of rows updated. will be zero if no rows updated.
            if (reader.RecordsAffected == 0)
            {
                reader.Close();
                string message = "#Error.Request " + dbRequest.GetRequest() + "from file " + fileName + "could not be inserted into db\n";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mw.topBox.AppendText(message);
                });
                return;
            }
            else
            {
                string message = "Request " + dbRequest.GetRequest() + "from file " + fileName + "was inserted into db\n";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mw.topBox.AppendText(message);
                });
            }
            reader.Close();
            //Taking realmID from existed row
            sqlCommand.CommandText = "select last_insert_id();";
            reader = sqlCommand.ExecuteReader();
            reader.Read();
            requestID = reader.GetString(0);
            idInfo.SetRequestID(requestID);
            reader.Close();
        }
        public static void DBBigInsertion(MySqlConnection dbConnection, DBRequest dbRequest, string fileName, DateTime dateTime, IDInformation idInfo, MainWindow mw)
        {
            DBFileInsertion(dbConnection, fileName, dateTime, idInfo, mw);
            DBCipherInsertion(dbConnection, dbRequest, fileName, dateTime, idInfo,mw);
            if(dbRequest.GetRealm() != null)
                DBRealmInsertion(dbConnection, dbRequest, fileName, dateTime, idInfo,mw);
            if(dbRequest.GetUser() != null)
                DBUserInsertion(dbConnection, dbRequest, fileName, dateTime,idInfo,mw);
            if (dbRequest.GetHostName() != null)
                DBHostInsertion(dbConnection, dbRequest, fileName, dateTime,idInfo,mw);
            if (dbRequest.GetServiceClass() != null)
                DBServiceInsertion(dbConnection, dbRequest, fileName, dateTime,idInfo,mw);
            TakePacketID(dbConnection, dbRequest, idInfo);
            DBRequestInsertion(dbConnection, dbRequest, fileName, dateTime, idInfo,mw);
        }
    }
}
