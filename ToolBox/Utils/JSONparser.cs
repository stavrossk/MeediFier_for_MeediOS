using System.Collections.Generic;
using System.Web.Script.Serialization;





public class NameTypePair
{
    public string OBJECT_NAME { get; set; }
    public string OBJECT_TYPE { get; set; }
}



public enum PositionType { none, point }


public class Ref
{
    public int id { get; set; }
}



public class SubObject
{
    public NameTypePair attributes { get; set; }
    public Position position { get; set; }
}


public class Position
{
    public int x { get; set; }
    public int y { get; set; }
}




public class Foo
{
    public Foo() { objects = new List<SubObject>(); }
    public string displayFieldName { get; set; }
    public NameTypePair fieldAliases { get; set; }
    public PositionType positionType { get; set; }
    public Ref reference { get; set; }
    public List<SubObject> objects { get; set; }
}



