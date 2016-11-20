using System;
using System.Collections.Generic;
using Shouldly;
using TunnelVisionLabs.Net;

namespace TestUriTemplates
{
    public class TunnelVisionLabsTest : TestBase<TunnelVisionLabs.Net.UriTemplate, UriTemplateMatch>
    {
        protected override UriTemplateMatch GetMatch(TunnelVisionLabs.Net.UriTemplate template, string uri)
        {
            return template.Match(new Uri(Uri.EscapeUriString(uri), UriKind.RelativeOrAbsolute));
        }

        protected override TunnelVisionLabs.Net.UriTemplate CreateTemplate(string template)
        {
            return new TunnelVisionLabs.Net.UriTemplate(template);
        }

        protected override void VariableHasValue(UriTemplateMatch match, string varName, string value)
        {
            KeyValuePair<VariableReference, object> actualValue;
            match.Bindings.TryGetValue(varName, out actualValue);

            actualValue.Value.ShouldBe(value);
        }

        protected override void VariableHasValues(UriTemplateMatch match, string varName, params string[] value)
        {
            KeyValuePair<VariableReference, object> actualValue;
            match.Bindings.TryGetValue(varName, out actualValue);

            actualValue.Value.ShouldBe(value);
        }
    }
}