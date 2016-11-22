using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nancy.Routing.UriTemplates
{
    public class TemplateRoute<T> : TemplateRoute
    {
        private readonly Func<dynamic, CancellationToken, Task<T>> action;

        public TemplateRoute(string name, string method, string templateString, Func<NancyContext, bool> condition, Func<dynamic, CancellationToken, Task<T>> action)
            : base(name, method, templateString, condition)
        {
            this.action = action;
        }

        public override async Task<object> Invoke(DynamicDictionary parameters, CancellationToken cancellationToken)
        {
            return await this.action.Invoke(parameters, cancellationToken);
        }
    }
}