
using Newtonsoft.Json;
using DilmerGames.Core.Singletons;

public class Serializer : Singleton<Serializer>
{
  
    public string SerializeToJSON(object o)
    {
        return JsonConvert.SerializeObject(o);
    }

    public object DeserializeFromJSON(string s)
    {
        return JsonConvert.DeserializeObject(s);
    }
}
