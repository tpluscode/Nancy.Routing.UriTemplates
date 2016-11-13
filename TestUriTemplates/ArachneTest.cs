using System;
using Arachne.Uri;
using Microsoft.FSharp.Collections;
using Shouldly;

namespace TestUriTemplates
{
    public class ArachneTest : TestBase<Arachne.Uri.Template.UriTemplate, Template.UriTemplateData>
    {
        protected override Template.UriTemplateData GetMatch(Template.UriTemplate template, string uri)
        {
            return template.Match(uri);
        }

        protected override Template.UriTemplate CreateTemplate(string template)
        {
            return Template.UriTemplate.parse.Invoke(template);
        }

        protected override void VariableHasValue(Template.UriTemplateData match, string varName, string value)
        {
            throw new NotImplementedException();
        }

        protected override void VariableHasValues(Template.UriTemplateData match, string varName, params string[] value)
        {
            var list = Template.UriTemplateValue.NewList(ListModule.OfSeq(value));
            match.Item[Template.UriTemplateKey.NewKey(varName)].Equals(list).ShouldBeTrue();
        }
    }
}