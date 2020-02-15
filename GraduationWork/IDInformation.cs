using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationWork
{
    class IDInformation
    {
        private string _requestID = null;
        private string _packetID = null;
        private string _userID = null;
        private string _cipherID = null;
        private string _fileID = null;
        private string _passwordID = null;
        private string _serviceID = null;
        private string _realmID = null;
        private string _hostID = null;
        public IDInformation()
        {
            _requestID = null;
            _packetID = null;
            _userID = null;
            _cipherID = null;
            _fileID = null;
            _passwordID = null;
            _serviceID = null;
            _realmID = null;
            _hostID = null;
        }
        public IDInformation(string requestID, string packetID, string userID, string cipherID, string fileID, string passwordID, string serviceID, string realmID, string hostID)
        {
            _requestID = requestID;
            _packetID = packetID;
            _userID = userID;
            _cipherID = cipherID;
            _fileID = fileID;
            _passwordID = passwordID;
            _serviceID = serviceID;
            _realmID = realmID;
            _hostID = hostID;
        }
        public void SetRequestID(string requestID)
        {
            this._requestID= requestID;
        }
        public void SetPacketID(string packetID)
        {
            this._packetID = packetID;
        }
        public void SetUserID(string userID)
        {
            this._userID = userID;
        }
        public void SetCipherID(string cipherID)
        {
            this._cipherID = cipherID;
        }
        public void SetFileID(string fileID)
        {
            this._fileID = fileID;
        }
        public void SetPasswordID(string passwordID)
        {
            this._passwordID = passwordID;
        }
        public void SetServiceID(string serviceID)
        {
            this._serviceID = serviceID;
        }
        public void SetRealmID(string realmID)
        {
            this._realmID = realmID;
        }
        public void SetHostID(string hostID)
        {
            this._hostID = hostID;
        }
        public string GetHostID()
        {
            return this._hostID;
        }
        public string GetRealmID()
        {
            return this._realmID;
        }
        public string GetRequestID()
        {
            return this._requestID;
        }
        public string GetPacketID()
        {
            return this._packetID;
        }
        public string GetUserID()
        {
            return this._userID;
        }
        public string GetCipherID()
        {
            return this._cipherID;
        }
        public string GetFileID()
        {
            return this._fileID;
        }
        public string GetPasswordID()
        {
            return this._passwordID;
        }
        public string GetServiceID()
        {
            return this._serviceID;
        }
    }
}
