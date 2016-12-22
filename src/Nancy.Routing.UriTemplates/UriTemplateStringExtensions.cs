using System.Linq;
using UriTemplateString.Spec;

namespace Nancy.Routing.UriTemplates
{
    internal static class UriTemplateStringExtensions
    {
        internal static bool StartsWithSlash(this UriTemplateString.UriTemplateString template)
        {
            return StartsWithSlash((dynamic)template.Parts.First());
        }

        private static bool StartsWithSlash(Literal literalPart)
        {
            return literalPart.ToString().StartsWith("/");
        }

        private static bool StartsWithSlash(Expression expressionPart)
        {
            return expressionPart.Operator.Equals(Operator.PathSegment);
        }
    }
}