using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFairy
{
    internal class IMDbPersonCollection : List<IIMDbPerson>, IIMDbPersonCollection
    {

        #region Constants and Variables

        private string _roles = "";

        #endregion


        #region Public Methods

        public void SetActorRolesString(string roles)
        {
            #region " old way "
            //int num1 = 0, num2 = 0, num3 = 0, num4 = 0, numTmp = 0, index = 0;
            //while (num1 > -1)
            //{
            //    num1 = roles.IndexOf("|", num1);
            //    if (num1 > -1)
            //    {
            //        num1++;
            //        if (num1 < roles.Length)
            //        {
            //            num2 = roles.IndexOf("|");
            //            if (num2 > -1)
            //            {
            //                string role = roles.Substring(num1, (num2 - num1)).Trim();
            //                try
            //                {
            //                    Person person = (Person)base[index];
            //                    while (person.IsActor == false)
            //                    {
            //                        index++;
            //                        person = (Person)base[index];
            //                    }
            //                    while (num3 > -1)
            //                    {
            //                        numTmp = num3;
            //                        num3 = role.IndexOf(" / ", numTmp);
            //                        if (num3 < 0)
            //                        {
            //                            num3 = role.IndexOf("/", numTmp);
            //                            if (num3 < 0)
            //                            {
            //                                num3 = role.IndexOf(", ", numTmp);
            //                                if (num3 < 0)
            //                                {
            //                                    num3 = role.IndexOf(",", numTmp);
            //                                    if (num3 > -1)
            //                                        num3++;
            //                                }
            //                                else
            //                                    num3 += 2;
            //                            }
            //                            else
            //                                num3++;
            //                        }
            //                        else
            //                            num3 += 3;

            //                        if (num3 > -1)
            //                        {
            //                            num4 = role.IndexOf(" / ", num3);
            //                            if (num4 < 0)
            //                            {
            //                                num4 = role.IndexOf("/", num3);
            //                                if (num4 < 0)
            //                                {
            //                                    num4 = role.IndexOf(", ", num3);
            //                                    if (num4 < 0)
            //                                    {
            //                                        num4 = role.IndexOf(",", num3);
            //                                        if (num4 < 0)
            //                                        {
            //                                            num4 = role.IndexOf("|", num3);
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                            if (num4 > -1 && num4 != num3 && num4 != (num3 + 1))
            //                            {
            //                                person.Roles.Add(role.Substring(num3, (num4 - num3)).Trim());
            //                            }
            //                            else
            //                                num3 = -1;
            //                        }
            //                    }
            //                }
            //                catch
            //                {
            //                    // do nothing
            //                }
            //                index++;
            //            }
            //            else
            //            {
            //                num1 = -1;
            //            }
            //        }
            //    }
            //}
            #endregion

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

                if (person.IsDirector)
                {
                    sbDirector.Append(person.Name);
                    if (count > 1)
                        sbDirector.Append("|");
                }
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

                if (person.IsWriter)
                {
                    sbWriter.Append(person.Name);
                    if (count > 1)
                        sbWriter.Append("|");
                }
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

                if (person.IsActor)
                {
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
            }
            return sbActors.ToString();
        }

        public string GetRolesString()
        {
            if (_roles.Length <= 0)
            {
                int index = 0;
                StringBuilder sbRoles = new StringBuilder();
                foreach (IMDbPerson person in this)
                {
                    if (index == 0)
                    {
                        sbRoles.Append("|");
                        index++;
                    }
                    if (person.IsActor)
                    {
                        string sRoles = person.GetRoles();
                        if (sRoles.Length > 0)
                        {
                            sbRoles.Append(sRoles);
                            sbRoles.Append("|");
                        }
                    }
                }
                if (sbRoles.Length <= 1)
                    return "";
                else
                    return sbRoles.ToString();
            }
            else
                return _roles;
        }

        public IIMDbPerson GetPersonByName(string Name)
        {
            for (int i = 0; i < base.Count; i++)
            {
                if (base[i].Name.ToLower().Trim() == Name.ToLower().Trim())
                {
                    return base[i];
                }
            }
            return null;
        }

        public IIMDbPerson GetPersonByURL(string URL)
        {
            for (int i = 0; i < base.Count; i++)
            {
                if (base[i].URL.ToLower().Trim() == URL.ToLower().Trim())
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
                if (base[i].Name.ToLower().Trim() == Name.ToLower().Trim())
                {
                    base.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion

    }
}