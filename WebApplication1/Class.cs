using WebApplication1.Controllers;

namespace WebApplication1
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class XptoAttribute : Attribute
    {
        public XptoAttribute(string template) { }

        public string? Name { get; set; }

    }

    public class Zulu
    {
        public Zulu(string template) { }

        public string? Name { get; set; }

    }

    public class Beta
    {
        [Xpto(template: "aaa", Name = "bbbbb")]  //template is parameter of method and Name is a Property
        public void Method()
        {
            var zz = new Zulu(template: "aaa") { Name = "bbbbb" };
        }
    }


}


