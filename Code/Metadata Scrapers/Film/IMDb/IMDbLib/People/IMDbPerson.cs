using System;
using System.Collections.Generic;
using System.Text;

namespace MeediFier
{
    internal class IMDbPerson : IIMDbPerson
    {

        #region Constants and Variables
        
        private string _name = "";
        private string _bday = "";
        private string _bio = "";
        private string _image = "";
        private string _personID = "";
        private string _height = "";
        private string _url = "";
        private bool _isDirector = false;
        private bool _isWriter = false;
        private bool _isActor = false;
        private List<string> _roles = new List<string>();
        
        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string URL
        {
            get { return _url; }
            set { _url = value; }
        }

        public bool IsDirector
        {
            get { return _isDirector; }
            set { _isDirector = value; }
        }

        public bool IsWriter
        {
            get { return _isWriter; }
            set { _isWriter = value; }
        }

        public bool IsActor
        {
            get { return _isActor; }
            set { _isActor = value; }
        }

        public string Birthday
        {
            get { return _bday; }
            set { _bday = value; }
        }

        public string Bio
        {
            get { return _bio; }
            set { _bio = value; }
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public string ID
        {
            get { return _personID; }
            set { _personID = value; }
        }

        public string Height
        {
            get { return _height; }
            set { _height = value; }
        }

        #endregion

        public void AddRole(string newRole)
        {
            bool okToAdd = true;
            if (_roles.Count > 0)
            {
                foreach (string role in _roles)
                {
                    if (role.ToLower().Trim() == newRole.ToLower().Trim())
                        okToAdd = false;
                }
            }
            if (okToAdd)
                _roles.Add(newRole);
        }

        public string GetRolesWithParens()
        {
            StringBuilder sbRoles = new StringBuilder();
            if (_roles.Count > 0)
            {
                sbRoles.Append("(");
                foreach (string sRole in _roles)
                {
                    sbRoles.Append(sRole);
                    sbRoles.Append(", ");
                }
                sbRoles.Length -= 2;
                sbRoles.Append(")");
                return sbRoles.ToString();
            }
            else
                return "";
        }

        public string GetRoles()
        {
            StringBuilder sbRoles = new StringBuilder();
            if (_roles.Count > 0)
            {
                foreach (string sRole in _roles)
                {
                    sbRoles.Append(sRole);
                    sbRoles.Append(", ");
                }
                sbRoles.Length -= 2;
                return sbRoles.ToString();
            }
            else
                return "";
        }
    } 
}
