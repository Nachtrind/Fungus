using System.Collections.Generic;

public class ParameterStorage
{
    Dictionary<string, object> parameters = new Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);

    public void SetParameter(string name, object value)
    {
        if (parameters.ContainsKey(name))
        {
            parameters[name] = value;
        }
        else
        {
            parameters.Add(name, value);
        }
    }

    public bool HasParameter(string name)
    {
        if (parameters.ContainsKey(name))
        {
            return true;
        }
        return false;
    }

    public bool TryGetParameter<T>(string name, out T value)
    {
        object existing;
        if (parameters.TryGetValue(name, out existing))
        {
            if (existing is T && !existing.Equals(default(T)))
            {
                value = (T)(existing);
                return true;
            }
            value = default(T);
            return false;
        }
        value = default(T);
        return false;
    }
}