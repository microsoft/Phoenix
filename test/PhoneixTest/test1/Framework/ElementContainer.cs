
namespace Phoenix.Test.UI.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Phoenix.Test.UI.Framework.WebPages;

    public abstract class ElementContainer
    {
        private const BindingFlags NonPublicAndFlattenedBindingOptions = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        public ISearchContext SearchContext { get; private set; }

        protected ElementContainer(ISearchContext searchContext)
        {
            SearchContext = searchContext;
        }

        protected ElementContainer(ISearchContext searchContext, By bys)
            : this(new ScopedSearchContext(searchContext, bys))
        { }

        /// <summary>
        /// Initializes the HtmlControl (and perhaps Section in the future) members of this Page Object that
        /// have the ControlAttribute (or SectionAttribute).
        /// </summary>
        /// <param name="owner"></param>
        protected void InitElements(Page owner)
        {
            var type = GetType();
            var members = new List<MemberInfo>();
            members.AddRange(type.GetFields(NonPublicAndFlattenedBindingOptions));
            members.AddRange(type.GetProperties(NonPublicAndFlattenedBindingOptions));

            foreach (var member in members)
            {
                var attributes = Attribute.GetCustomAttributes(member, typeof(FindsByAttribute), true)
                    .Select(a => (FindsByAttribute)a).ToArray();

                if (attributes.Any())
                {
                    var bys = attributes.Where(a => !String.IsNullOrEmpty(a.Using))
                        .OrderBy(a => a.Priority)
                        .Select(a => CreateBy(a.How, a.Using))
                        .ToArray();
                    var by = new ByChained(bys);

                    var field = member as FieldInfo;
                    var property = member as PropertyInfo;
                    if (field != null)
                    {
                        var T = field.FieldType;
                        field.SetValue(this, CreateInstance(T, owner, @by));
                    }
                    else if (property != null)
                    {
                        var T = property.PropertyType;
                        property.SetValue(this, CreateInstance(T, owner, @by), null);
                    }
                }
            }
        }


        public object CreateInstance(Type type, Page page, ByChained @by)
        {
            return Activator.CreateInstance(type, page, @by);
        }

        /// <summary>
        /// Create a By accessor from a By method (How) and string (Using).
        /// </summary>
        /// <param name="how">type of By</param>
        /// <param name="usingValue">string to use</param>
        /// <returns>By accessor created</returns>
        public static By CreateBy(How how, string usingValue)
        {
            switch (how)
            {
                case How.Id:
                    return By.Id(usingValue);
                case How.Name:
                    return By.Name(usingValue);
                case How.TagName:
                    return By.TagName(usingValue);
                case How.ClassName:
                    return By.ClassName(usingValue);
                case How.CssSelector:
                    return By.CssSelector(usingValue);
                case How.LinkText:
                    return By.LinkText(usingValue);
                case How.PartialLinkText:
                    return By.PartialLinkText(usingValue);
                case How.XPath:
                    return By.XPath(usingValue);
                default:
                    //Log.Error(String.Format("Cannot construct By from How={0}, Using={1}", how, usingValue));
                    throw new ArgumentException();
            }
        }

        private class ScopedSearchContext : ISearchContext
        {
            private readonly By containerLocator;
            private readonly ISearchContext parentSearchContext;

            public ScopedSearchContext(ISearchContext parentSearchContext, By containerLocator)
            {
                this.containerLocator = containerLocator;
                this.parentSearchContext = parentSearchContext;
            }

            public IWebElement FindElement(By @by)
            {
                return @by.FindElement(containerLocator.FindElement(parentSearchContext));
            }

            public ReadOnlyCollection<IWebElement> FindElements(By @by)
            {
                return @by.FindElements(containerLocator.FindElement(parentSearchContext));
            }
        }
    }
}
