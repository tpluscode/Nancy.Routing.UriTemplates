using Shouldly;
using Xunit;

namespace TestUriTemplates
{
    public abstract class TestBase<TTemplate, TMatch>
    {
        [Fact]
        public void Should_expand_all_variables()
        {
            // given
            string uri = "/base/path/a/b/c/.xyz,abc.dupa2/2?name=tomasz&lastname=pluskiewicz";
            var template = CreateTemplate("/base/path{/username*}/{.articles,dupa}{/page}{?name,lastname}");

            // when
            var match = GetMatch(template, uri);

            // then
            match.ShouldNotBeNull();
            VariableHasValues(match, "username", "a", "b", "c");
            VariableHasValues(match, "articles", "xyz", "abc");
            VariableHasValue(match, "dupa", "dupa2");
            VariableHasValue(match, "page", "2");
            VariableHasValue(match, "name", "tomasz");
            VariableHasValue(match, "lastname", "pluskiewicz");
            VariableHasValue(match, "polish", "ąę");
        }
        [Fact]
        public void Should_expand_when_diacritics_used()
        {
            // given
            string uri = "/base/path?query=ąę";
            var template = CreateTemplate("/base/path{?query}");

            // when
            var match = GetMatch(template, uri);

            // then
            match.ShouldNotBeNull();
            VariableHasValue(match, "query", "ąę");
        }

        protected abstract TMatch GetMatch(TTemplate template, string uri);
        protected abstract TTemplate CreateTemplate(string template);
        protected abstract void VariableHasValue(TMatch match, string varName, string value);
        protected abstract void VariableHasValues(TMatch match, string varName, params string[] value);
    }
}
