using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
namespace Lab1_sppr.TagHelpers
{
    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {
        [HtmlAttributeName("current-page")]
        public int CurrentPage { get; set; }

        [HtmlAttributeName("total-pages")]
        public int TotalPages { get; set; }

        [HtmlAttributeName("category")]
        public string? Category { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            Console.WriteLine($"Current Page: {CurrentPage}, Total Pages: {TotalPages}");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            AddPageLink(ul, "<<", CurrentPage > 1 ? CurrentPage - 1 : 1, CurrentPage == 1);

            for (int i = 1; i <= TotalPages; i++)
            {
                AddPageLink(ul, i.ToString(), i, CurrentPage == i);
            }

            AddPageLink(ul, ">>", CurrentPage < TotalPages ? CurrentPage + 1 : TotalPages, CurrentPage == TotalPages);

            output.TagName = "nav";
            output.Content.AppendHtml(ul);

            Console.WriteLine(output.Content.GetContent());
        }


        private void AddPageLink(TagBuilder ul, string text, int pageNo, bool isDisabledOrActive)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");
            if (isDisabledOrActive)
            {
                li.AddCssClass(text == CurrentPage.ToString() ? "active" : "disabled");
            }

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");
            a.Attributes["href"] = $"?category={Category}&pageNo={pageNo}";
            a.InnerHtml.Append(text);

            li.InnerHtml.AppendHtml(a);
            ul.InnerHtml.AppendHtml(li);
        }
    }
}
