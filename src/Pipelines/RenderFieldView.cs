using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc;
using Sitecore.ExperienceForms.Mvc.Html;
using Sitecore.ExperienceForms.Mvc.Pipelines.RenderField;
using Sitecore.Mvc.Pipelines;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace SharedSitecore.Forms.Fields.DropDownPlus.Pipelines.RenderField
{
    public class RenderFieldView : MvcPipelineProcessor<RenderFieldEventArgs>
    {
        private const string templateId = "{6ABEE1F2-4AB4-47F0-AD8B-BDB36F37F64C}";
        private const string sharedSitecoreFieldTypesId = "{57CE297C-5B57-4833-B107-CDE1086C9BF4}";
        private const string fields = "{EBDE78CC-AD3F-4623-B395-CC67DF70B5EA}";
        private const string fieldTypeTemplateId = "{A60EDCAF-1285-46B5-8380-D790BB8C8708}";

        protected IFormRenderingContext FormRenderingContext { get; }

        public RenderFieldView(IFormRenderingContext formRenderingContext)
        {
            Assert.ArgumentNotNull(formRenderingContext, "formRenderingContext");
            FormRenderingContext = formRenderingContext;
        }

        public override void Process(RenderFieldEventArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.ViewModel != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (ContainerElement containerElement = new ContainerElement(stringBuilder, args.ContainerSettings, args.FormBuilderContext.FormBuilderMode))
                {
                    SetFieldValues(args, containerElement);
                    MvcHtmlString mvcHtmlString = args.HtmlHelper.Partial(args.RenderingSettings.ViewPath, args.ViewModel);
                    containerElement.AddContent(mvcHtmlString);
                }
                args.Result = new MvcHtmlString(stringBuilder.ToString());
            }
        }

        protected virtual void SetFieldValues(RenderFieldEventArgs args, ContainerElement container)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(container, "container");
            if (args.FormBuilderContext.FormBuilderMode == FormBuilderMode.Load)
            {
                Type type = args.ViewModel.GetType();
                Type typeFromHandle = typeof(IValueField);
                if (typeFromHandle.IsAssignableFrom(type))
                {
                    args.HtmlHelper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = string.Empty;
                    MvcHtmlString mvcHtmlString = args.HtmlHelper.Hidden(FormRenderingContext.IndexName, args.ViewModel.ItemId, new Dictionary<string, object>
                    {
                        {
                            "id",
                            args.HtmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(FormRenderingContext.IndexName + "." + args.ViewModel.ItemId)
                        }
                    });
                    container.AddContent(mvcHtmlString);
                    args.HtmlHelper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = FormRenderingContext.CreateFieldPrefix(args.ViewModel.ItemId);
                    MvcHtmlString mvcHtmlString2 = args.HtmlHelper.Hidden("ItemId", args.ViewModel.ItemId);
                    container.AddContent(mvcHtmlString2);
                }
                else
                {
                    args.HtmlHelper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = FormRenderingContext.Prefix;
                }

                //var queryStringId = WebUtil.GetSafeQueryString("id");
                //if (type.Name == "DropDownPlusViewModel" && !string.IsNullOrEmpty(queryStringId) && ID.IsID(queryStringId))
                //{
                //    var it = Context.Database.GetItem(queryStringId);
                //    if (it != null)
                //    {
                //        Log.Debug(FormRenderingContext.IndexName, this);
                       
                //        var viewModel = (DropDownPlusViewModel)args.ViewModel;
                //        var name = it.Fields.FirstOrDefault(f => f.Name.ToLower() == "name")?.Value?.ToString();
                //        if (string.IsNullOrEmpty(name)) name = it.DisplayName;
                //        var value = it.Fields.FirstOrDefault(f => f.Name.ToLower() == "email")?.Value?.ToString();

                //        viewModel.DefaultSelection = $"{value};{name}";

                //        if (!viewModel.ShowItems) viewModel.Items.Clear();
                //        var item = viewModel.ShowItems ? viewModel.Items.FirstOrDefault(i => i.Text.ToLower() == name) : null;
                //        if (item == null)
                //        {
                //            item = new Sitecore.ExperienceForms.Mvc.Models.ListFieldItem { Text = name };
                //            viewModel.Items.Add(item);
                //        }
                //        item.Value = viewModel.ShowValues ? value : queryStringId;
                //        item.Selected = true;
                //    }
                //}
            }
            else
            {
                args.HtmlHelper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = FormRenderingContext.CreateFieldPrefix(args.ViewModel.ItemId);
            }
        }
    }
}