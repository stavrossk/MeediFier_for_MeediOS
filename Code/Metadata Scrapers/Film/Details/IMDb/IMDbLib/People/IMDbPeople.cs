using System;
using System.Collections.Generic;
using System.Text;

namespace MeediFier
{
    internal class IMDbPersonCollection : List<IIMDbPerson>, IIMDbPersonCollection
    {

        #region Constants and Variables

        private string _roles = "";

        #endregion


        #region Public Methods

        public void SetActorRolesString(string roles)
        {


            _roles = roles;

        }


        public string GetDirectorString()
        {

            int index = 0, count = 0;
            
            StringBuilder sbDirector = new StringBuilder();
            
            foreach (IMDbPerson person in this)
            {
                if (person.IsDirector)
                    count++;
            }

            foreach (IMDbPerson person in this)
            {
                if (index == 0 && count > 1)
                {
                    sbDirector.Append("|");
                    index++;
                }
                else if (count <= 0)
                    return "";

                if (!person.IsDirector) continue;
                sbDirector.Append(person.Name);
                if (count > 1)
                    sbDirector.Append("|");
            }
            return sbDirector.ToString();
        
        }



        public string GetWriterString()
        {

            int index = 0, count = 0;
            
            StringBuilder sbWriter = new StringBuilder();
            
            foreach (IMDbPerson person in this)
            {
                if (person.IsWriter)
                    count++;
            }
            
            foreach (IMDbPerson person in this)
            {
                if (index == 0 && count > 1)
                {
                    sbWriter.Append("|");
                    index++;
                }
                else if (count <= 0)
                    return "";

                if (!person.IsWriter) continue;
                sbWriter.Append(person.Name);
                if (count > 1)
                    sbWriter.Append("|");
            }

            return sbWriter.ToString();
        
        }


        public string GetActorsString(bool getRoles)
        {

            int index = 0, count = 0;
            
            StringBuilder sbActors = new StringBuilder();
            
            foreach (IMDbPerson person in this)
            {
                if (person.IsActor)
                    count++;
            }
            
            foreach (IMDbPerson person in this)
            {
                if (index == 0 && count > 1)
                {
                    sbActors.Append("|");
                    index++;
                }
                else if (count <= 0)
                    return "";

                if (!person.IsActor) continue;
                sbActors.Append(person.Name);
                if (getRoles)
                {
                    string roles = person.GetRolesWithParens();
                    if (roles.Trim() != "")
                    {
                        sbActors.Append(" ");
                        sbActors.Append(roles);
                    }
                }
                if (count > 1)
                    sbActors.Append("|");
            }
            
            return sbActors.ToString();
        
        }


        public string GetRolesString()
        {

            if (_roles.Length > 0)
                return _roles;
            
            int index = 0;
            
            StringBuilder sbRoles = new StringBuilder();
            

            foreach (IMDbPerson person in this)
            {

                if (index == 0)
                {
                    sbRoles.Append("|");
                    index++;
                }

                if (!person.IsActor) continue;
                
                string sRoles = person.GetRoles();

                if (sRoles.Length <= 0) 
                    continue;
                
                sbRoles.Append(sRoles);
                
                sbRoles.Append("|");
            
            }


            return sbRoles.Length <= 1 ? "" : sbRoles.ToString();
        
        }

        public IIMDbPerson GetPersonByName(string name)
        {
            for (int i = 0; i < base.Count; i++)
            {
                if (base[i].Name.ToLower().Trim() == name.ToLower().Trim())
                {
                    return base[i];
                }
            }
            return null;
        }


        public IIMDbPerson GetPersonByURL(string url)
        {
            for (int i = 0; i < base.Count; i++)
            {
                if (base[i].URL.ToLower().Trim() == url.ToLower().Trim())
                {
                    return base[i];
                }
            }
            return null;
        }

        public IIMDbPerson GetPersonByID(string ID)
        {
            for (int i = 0; i < base.Count; i++)
            {
                if (base[i].ID.ToLower().Trim() == ID.ToLower().Trim())
                {
                    return base[i];
                }
            }
            return null;
        }

        public void Remove(string Name)
        {
            for (int i = 0; i < base.Count; i++)
            {

                if (base[i].Name.ToLower().Trim() != Name.ToLower().Trim()) continue;
                
                RemoveAt(i);
                
                break;
            
            }
        
        }

        #endregion

    }
}