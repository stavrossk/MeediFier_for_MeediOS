using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFairy
{
    public interface IIMDbPerson
    {
        string Name { get; set; }
        string URL { get; set; }
        bool IsDirector { get; set; }
        bool IsWriter { get; set; }
        bool IsActor { get; set; }
        string Birthday { get; set; }
        string Bio { get; set; }
        string Image { get; set; }
        string ID { get; set; }
        string Height { get; set; }

        void AddRole(string newRole);
        string GetRolesWithParens();
        string GetRoles();
    }
}
