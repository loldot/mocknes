﻿using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Mocknes;

public static class Mocknes
{
    public static T Mock<T>(Action<MockProxy<T>>? setup) where T : class
    {
        var proxy = DispatchProxy.Create<T, MockProxy<T>>() as MockProxy<T>;
        setup?.Invoke(proxy);

        return proxy as T;
    }
}

public class MockProxy<T> : DispatchProxy where T : class
{
    private Dictionary<string, object> results = new();
    public T? Target { get; set; }

    public StubCollection Stubs = new StubCollection();

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (Stubs.TryGet(targetMethod!.Name, args!, out var result))
        {
            return result;
        }

        if (Target is not null)
        {
            return targetMethod?.Invoke(Target, args);
        }

        throw new NotImplementedException("Unable to invoke stub or proxied method");
    }



    public StubBuilder<T> When(Expression<Func<T, object>> expression)
    {
        MethodInfo? methodInfo = null;
        ArgumentEvaluation[] filters = Array.Empty<ArgumentEvaluation>();
        if (expression.Body is UnaryExpression unary)
        {
            if (unary.Operand is MethodCallExpression innerMce)
            {
                methodInfo = innerMce.Method;
                filters = innerMce.Arguments.Select(x => x.Reduce() is ConstantExpression c
                    ? new ArgumentEvaluation(c.Value)
                    : new ArgumentEvaluationAny()).ToArray();
            }
        }

        if (expression.Body is MethodCallExpression methodCallExpression)
        {
            methodInfo = methodCallExpression.Method;
            filters = methodCallExpression.Arguments.Select(x => x.Reduce() is ConstantExpression c
                    ? new ArgumentEvaluation(c.Value)
                    : new ArgumentEvaluationAny()).ToArray();
        }

        return new StubBuilder<T>(this, methodInfo.Name, filters);
    }
}
