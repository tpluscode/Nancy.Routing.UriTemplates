using System;

namespace TestUriTemplates
{
    public class EarlTest : TestBase<Earl.UriTemplate, object>
    {
        protected override object GetMatch(Earl.UriTemplate template, string uri)
        {
            return new NotImplementedException("no Match method");
        }

        protected override Earl.UriTemplate CreateTemplate(string template)
        {
            return new Earl.UriTemplate(template);
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