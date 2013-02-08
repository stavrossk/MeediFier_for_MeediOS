using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFairy
{
    public interface IIMDbPersonCollection: IList<IIMDbPerson>
    {
        void SetActorRolesString(string roles);
        void Remove(string name);
        string GetDirectorString();
        string GetWriterString();
        string GetActorsString(bool getRoles);
        string GetRolesString();
        IIMDbPerson GetPersonByName(string name);
        IIMDbPerson GetPersonByURL(string url);
        IIMDbPerson GetPersonByID(string id);
    }
}
