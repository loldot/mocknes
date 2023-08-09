namespace Mocknes;

public class ArgumentEvaluation
{
    private readonly dynamic value;

    public ArgumentEvaluation(dynamic value)
    {
        this.value = value;
    }
    public virtual bool IsMatch(dynamic argument) => argument is null && value is null || argument?.Equals(value);
}
public class ArgumentEvaluationAny : ArgumentEvaluation
{
    public ArgumentEvaluationAny() : base(null)
    {
    }

    public override bool IsMatch(dynamic argument) => true;
}

public class InvocationFilter
{
    public string Method { get; }
    public ArgumentEvaluation[] ArgEvalutions { get; }

    public InvocationFilter(string method,  ArgumentEvaluation[] filters)
    {
        Method = method;
        ArgEvalutions = filters;
    }

    public bool IsMatch(string methodName, object[] arguments)
    {
        if (Method != methodName)
        {
            return false;
        }

        if (arguments.Length != ArgEvalutions.Length)
        {
            return false;
        }

        for (int i = 0; i < arguments.Length; i++)
        {
            if (!ArgEvalutions[i].IsMatch(arguments[i]))
            {
                return false;
            }
        }

        return true;
    }
}
public class StubBuilder<T> where T : class
{
    private MockProxy<T> parent;
    private string method;
    private ArgumentEvaluation[]  filters;

    public StubBuilder(MockProxy<T> parent, string methodName, ArgumentEvaluation[] filters)
    {
        this.parent = parent;
        this.method = methodName;
        this.filters = filters;
    }

    public void ThenReturn(object result)
    {
        parent.Stubs.Add(Build(), result);
    }

    public void ThenReturnAsync<TReturn>(TReturn result)
    {
        ThenReturn(Task.FromResult(result)); 
    }

    private InvocationFilter Build()
    {
        return new InvocationFilter(method, filters);
    }
}

public class Stub
{
    public Stub(InvocationFilter filter, object result)
    {
        Filter = filter;
        Result = result;
    }

    public InvocationFilter Filter { get; set; }
    public object Result { get; set; }
}

public class StubCollection
{
    private List<Stub> stubs = new();

    public void Add(InvocationFilter filter, object result)
    {
        stubs.Add(new Stub(filter, result));
    }

    public bool TryGet(string methodName, object[] arguments, out object result)
    {
        var stub = stubs.FirstOrDefault(s => s.Filter.IsMatch(methodName, arguments));
        if(stub is not null)
        {
            result = stub.Result;
            return true;
        }
        result = null;
        return false;
    }
}