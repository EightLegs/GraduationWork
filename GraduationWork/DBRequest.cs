using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationWork
{
    public class DBRequest
    {
        private string _packetName;
        private string _realm;
        private string _user;
        private string _etype;
        private string _salt;
        private string _request;
        private string _serviceClass;
        private string _hostname;
        private string _port;

        public DBRequest()
        {
            _packetName = null;
            _realm = null;
            _user = null;
            _etype = null;
            _salt = null;
            _request = null;
            _serviceClass = null;
            _hostname = null;
            _port = null;
        }
        public static int FindPacketType(string requestString /*string[] words*/)
        {
            string[] words = requestString.Split(new char[] { '$', '/', ':' }, StringSplitOptions.RemoveEmptyEntries);

            string krbString = "krb5";
            string krbTgsString = "krb5tgs";
            string krbAsRepString = "krb5asrep";

            for (int i = 0; i < words.Length; ++i)
            {
                if (words[i].IndexOf(krbString) != -1)
                {
                    if (words[i] == krbTgsString)
                        return 1;
                    else if (words[i] == krbAsRepString)
                        return 2;
                    else
                        break;
                }
            }
            return 0;
        }
        public static void FillStructurePA(DBRequest dbRequest, string requestString /*string[] words*/)
        {//"%s:$krb5pa$%s$%s$%s$%s$%s%s\n" % (_user,_etype, _user, _realm, _salt,enc_timestamp, checksum))
         //sys.stdout.write("%s:$krb5pa$%s$%s$%s$%s$%s\n" % (_user,_etype, _user, _realm, _salt,PA_DATA_ENC_TIMESTAMP))
            string[] words = requestString.Split(new char[] { '$', '/', ':' }, StringSplitOptions.RemoveEmptyEntries);
            string krbString = "krb5";
            for (int i = 0; i < words.Length; ++i)
            {
                if (dbRequest.GetUser() == null)
                {
                    dbRequest.SetUser(words[i]);
                    continue;
                }
                if (words[i].IndexOf(krbString) != -1)
                {
                    dbRequest.SetPacketName(words[i]);
                    continue;
                }
                if (dbRequest.GetEType() == null)
                {
                    int helpValue = 0;
                    if (Int32.TryParse(words[i], out helpValue))
                    {
                        dbRequest.SetEType(words[i]);
                        continue;
                    }
                }
                if (dbRequest.GetPacketName() != null && dbRequest.GetEType() != null)
                {
                    if (dbRequest.GetUser() != null)
                    {
                        if (dbRequest.GetUser() == words[i])
                            continue;
                        else if (dbRequest.GetRealm() == null)
                        {
                            dbRequest.SetRealm(words[i]);
                            continue;
                        }
                        else if (dbRequest.GetRealm() == words[i])
                            continue;
                        if (dbRequest.GetRealm() != null)
                        {
                            if (i == words.Length - 1)
                            {
                                dbRequest.SetRequest(words[i]);
                                break;
                            }
                            else
                            {
                                dbRequest.SetSalt(words[i]);
                                continue;
                            }
                        }
                    }
                }
            }
        }
        /*        public static int CountElementsBeforePacketType(string[] words)
                {
                    int result = 0;
                    string krbString = "krb5";
                    for (int i = 0; i < words.Length; ++i)
                    {
                        if (words[i].IndexOf(krbString) == -1)
                            ++result;
                        else
                            break;
                    }
                    return result;
                }*/
        public static int FindSPN(DBRequest dbRequest, string[] words)
        {
            int result = 0;
            for (int i = 0; i < words.Length; ++i)
            {
                if (words[i] != "krb5tgs")
                    ++result;
                else
                    break;
            }
            if (result == 0)
                return 0;

            string spnString = "";
            for (int i = 0; i < result; ++i)
                spnString += words[i];

            string[] helpStringArray = spnString.Split(new char[] { '/', ':' }, StringSplitOptions.RemoveEmptyEntries);

            bool foundPort = false;
            int num = 0;
            if(int.TryParse(helpStringArray[helpStringArray.Length - 1], out num))
            {
                dbRequest.SetPort(helpStringArray[helpStringArray.Length - 1]);
                foundPort = true;
            }

            string hostRealmString = "";
            //We found service name and port - the reason why we start for loop from i = 1 to Length -1 -1 
            if(foundPort == false)
                for (int i = 1; i < helpStringArray.Length; ++i)
                    hostRealmString += helpStringArray[i];
            else
                for (int i = 1; i < helpStringArray.Length - 1; ++i)
                    hostRealmString += helpStringArray[i];

            string[] hostRealmArray = hostRealmString.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string realmString = "";
            //Searching for hostname and  realm, minimum of realm length divided by '.' is 2
            if (hostRealmArray.Length > 2)
            {
                dbRequest.SetHostName(hostRealmArray[0]);
                for (int i = 1; i < hostRealmArray.Length; ++i)
                    realmString += hostRealmArray[i] + '.';
                realmString = realmString.Substring(0, realmString.Length - 1);
                dbRequest.SetRealm(realmString);
            }
            else
                dbRequest.SetRealm(hostRealmString);
            dbRequest.SetServiceClass(helpStringArray[0]);

            return result;
        }
        public static void FillStructureTGS(DBRequest dbRequest, string requestString /*string[] words*/)
        {//"%s:$krb5tgs$%s$%s$%s\n" % (spn, _etype, data[:32], data[32:])) SPN состоит из _serviceClass, он уникален для каждого типа сервиса, затем идет _hostname в форме FQDN и для некоторых сервисов – _port. 
            
            string[] words = requestString.Split(new char[] {'$'}, StringSplitOptions.RemoveEmptyEntries);
            //This variable shows us from element of words is 'krb5tgs'
            int loopStart = FindSPN(dbRequest, words);

            for (int i = loopStart; i < words.Length; ++i)
            {
                if(dbRequest.GetPacketName() == null)
                {
                    dbRequest.SetPacketName(words[i]);
                    continue;
                }
                if(dbRequest.GetEType() == null)
                {
                    int helpValue = 0;
                    if (Int32.TryParse(words[i], out helpValue))
                    {
                        dbRequest.SetEType(words[i]);
                        continue;
                    }
                }
                if (dbRequest.GetRequest() == null)
                {
                    if (i != words.Length - 1)
                        continue;
                    else
                    {
                        dbRequest.SetRequest(words[i - 1]+words[i]);
                        break;
                    }
                }
            }
        }
        public static void FillStructureASREP(DBRequest dbRequest, string requestString /*string[] words*/)
        {
            string[] words = requestString.Split(new char[] { '$', '/', ':' }, StringSplitOptions.RemoveEmptyEntries);
            //"$krb5asrep$%s$%s$%s$%s\n" % (_etype, _salt, data[0:-24], data[-24:]))
            //"$krb5asrep$%s$%s$%s\n" % (_etype, data[0:32], data[32:])
            for (int i = 0; i < words.Length; ++i)
            {
                if(dbRequest.GetPacketName() == null)
                {
                    dbRequest.SetPacketName(words[i]);
                    continue;
                }
                if (dbRequest.GetEType() == null)
                {
                    int helpValue = 0;
                    if (Int32.TryParse(words[i], out helpValue))
                    {
                        dbRequest.SetEType(words[i]);
                        continue;
                    }
                }
                if (dbRequest.GetSalt() == null && dbRequest.GetEType() != "23")
                {
                    dbRequest.SetSalt(words[i]);
                    continue;
                }
                if (i != words.Length - 1)
                    continue;
                else
                {
                    dbRequest.SetRequest(words[i - 1] + words[i]);
                    break;
                }
            }
        }
        public DBRequest(string requestString/*string[] words*/)
        {
            _packetName = null;
            _realm = null;
            _user = null;
            _etype = null;
            _salt = null;
            _request = null;
            _serviceClass = null;
            _hostname = null;
            _port = null;

            switch(FindPacketType(requestString))
            {
                case 0:
                    FillStructurePA(this, requestString);
                    break;
                case 1:
                    FillStructureTGS(this, requestString);
                    break;
                case 2:
                    FillStructureASREP(this, requestString);
                    break;
            }
        }
        public void SetPacketName(string packetName)
        {
            this._packetName = packetName;
        }
        public void SetRealm(string realm)
        {
            this._realm = realm;
        }
        public void SetUser(string user)
        {
            this._user = user;
        }
        public void SetEType(string etype)
        {
            this._etype = etype;
        }
        public void SetSalt(string salt)
        {
            this._salt = salt;
        }
        public void SetRequest(string request)
        {
            this._request = request;
        }
        public void SetServiceClass(string serviceClass)
        {
            this._serviceClass = serviceClass;
        }
        public void SetHostName(string hostname)
        {
            this._hostname = hostname;
        }
        public void SetPort(string port)
        {
            this._port = port;
        }
        public string GetPacketName()
        {
            return this._packetName;
        }
        public string GetRealm()
        {
            return this._realm;
        }
        public string GetUser()
        {
            return this._user;
        }
        public string GetEType()
        {
            return this._etype;
        }
        public string GetSalt()
        {
            return this._salt;
        }
        public string GetRequest()
        {
            return this._request;
        }
        public string GetServiceClass()
        {
            return this._serviceClass;
        }
        public string GetHostName()
        {
            return this._hostname;
        }
        public string GetPort()
        {
            return this._port;
        }
    }
}
/*if (this.GetPacketName() == null)
        {
            if (words[i].IndexOf(krbString) == -1)
            {
                if (this.GetUser() == null)
                {
                    this.SetUser(words[i]);
                    ++foundPosition;
                    continue;
                }

                else
                {
                    this.SetRealm(words[i]);
                    ++foundPosition;
                    continue;
                }
            }
            else
            {
                this.SetPacketName(words[i]);
                ++foundPosition;
                continue;
            }
        }
        if (this.GetEType() == null)
        {
            int helpValue = 0;
            if (Int32.TryParse(words[i], out helpValue))
            {
                this.SetEType(words[i]);
                ++foundPosition;
                continue;
            }
        }
        if (this.GetPacketName() != null && this.GetEType() != null)
        {
            if (this.GetUser() != null)
            {
                if (this.GetUser() == words[i])
                    continue;
                else if (this.GetRealm() == null)
                {
                    this.SetRealm(words[i]);
                    ++foundPosition;
                    continue;
                }
                else if (this.GetRealm() == words[i])
                    continue;
                if (this.GetRealm() != null)
                {
                    if (i == words.Length - 1)
                    {
                        this.SetRequest(words[i]);
                        ++foundPosition;
                        break;
                    }
                    else
                    {
                        this.SetSalt(words[i]);
                        ++foundPosition;
                        continue;
                    }
                }
            }
        }*/