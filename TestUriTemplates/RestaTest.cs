using System;

namespace TestUriTemplates
{
    public class RestaTest : TestBase<Resta.UriTemplates.UriTemplate, object>
    {
        protected override object GetMatch(Resta.UriTemplates.UriTemplate template, string uri)
        {
            throw new NotImplementedException("No Match method");
        }

        protected override Resta.UriTemplates.UriTemplate CreateTemplate(string template)
        {
            return new Resta.UriTemplates.UriTemplate(template);
        }

        protected override void VariableHasValue(object match, string varName, string value)
        {
            throw new NotImplementedException();
        }

        protected override void VariableHasValues(object match, string varName, params string[] value)
        {
            throw new NotImplementedException();
        }
    }
}