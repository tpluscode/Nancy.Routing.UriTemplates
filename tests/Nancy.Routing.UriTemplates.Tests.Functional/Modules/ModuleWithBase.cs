namespace Nancy.Routing.UriTemplates.Tests.Functional.Modules
{
    public class ModuleWithBase : UriTemplateModule
    {
        public ModuleWithBase(string basePath)
            : base(basePath)
        {
            using (this.Templates)
            {
                this.Get("{/optional}/path", o => this.Response.AsJson((DynamicDictionary)o));
            }
        }
    }
}