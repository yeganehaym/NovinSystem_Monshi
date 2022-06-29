using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Monshi.Web.TagHelpers;

[HtmlTargetElement("listItems")]
public class ListTagHelper:TagHelper
{
    [HtmlAttributeName("asp-items")]
    public List<string> Items { get; set; }
        
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }
        
        

    public HttpRequest Request => ViewContext.HttpContext.Request;
        
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
            
        if(Items==null)
            return;

        output.TagName = "ul";
        output.TagMode = TagMode.StartTagAndEndTag;
            
        foreach (var item in Items)
        {
            TagBuilder tagBuilder = new TagBuilder("li");

            TagBuilder a = new TagBuilder("a");
            a.InnerHtml.Append(item);
            a.Attributes.Add("href","https://www.google.com/search?q=" + item);
            a.Attributes.Add("target","_blank");

            tagBuilder.InnerHtml.AppendHtml(a);
            output.Content.AppendHtml(tagBuilder);


        }
        base.Process(context,output);
    }
}