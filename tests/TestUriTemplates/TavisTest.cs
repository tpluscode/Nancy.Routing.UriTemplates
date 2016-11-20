using System;
using Shouldly;
using Tavis.UriTemplates;

namespace TestUriTemplates
{
    public class TavisTest : TestBase<Tavis.UriTemplates.UriTemplate, TemplateMatch>
    {
        protected override TemplateMatch GetMatch(Tavis.UriTemplates.UriTemplate template, string uri)
        {
            var uriTemplateTable = new UriTemplateTable();
            uriTemplateTable.Add("test", template);
            return uriTemplateTable.Match(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        protected override Tavis.UriTemplates.UriTemplate CreateTemplate(string template)
        {
            return new Tavis.UriTemplates.UriTemplate(template);
        }

        protected override void VariableHasValue(TemplateMatch match, string varName, string value)
        {
            object actualVal;
            match.Parameters.TryGetValue(varName, out actualVal);
            actualVal.ShouldBe(value);
        }

        protected override void VariableHasValues(TemplateMatch match, string varName, params string[] value)
        {
            object actualVal;
            match.Parameters.TryGetValue(varName, out actualVal);
            actualVal.ShouldBe(value);
        }
    }
}