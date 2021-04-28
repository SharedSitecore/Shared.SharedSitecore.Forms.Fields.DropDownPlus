using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Mvc;
using Sitecore.ExperienceForms.Mvc.Pipelines.ExecuteSubmit;
using Sitecore.Mvc.Pipelines;
using System.Linq;

namespace SharedSitecore.Forms.Fields.DropDownPlus.Pipelines
{
    public class ExecuteSubmit : MvcPipelineProcessor<ExecuteSubmitActionsEventArgs>
    {
        private readonly IFormRenderingContext _formRenderingContext;

        /// <summary>
        /// ID of the Field on the Form
        /// </summary>
        public string FieldId { get; set; }

        public ExecuteSubmit(IFormRenderingContext renderingContext)
        {
            _formRenderingContext = renderingContext;
        }

        public override void Process(ExecuteSubmitActionsEventArgs args)
        {
            Assert.ArgumentNotNull(FieldId, nameof(FieldId));
            if (!args.FormSubmitContext.Fields.Any(f => f.Name == nameof(DropDownPlus))) return;

            var fields = args.FormSubmitContext.Fields.Where(f => f.Name == nameof(DropDownPlus));
            if (fields != null) 
            {
                foreach (var field in fields)
                {
                    ReplaceTokensIfApplicable((DropDownPlusViewModel)field);
                }
            }
        }

        protected virtual void ReplaceTokensIfApplicable(DropDownPlusViewModel viewModel)
        {
            Assert.ArgumentNotNull(viewModel, nameof(viewModel));
            Assert.ArgumentNotNull(viewModel.Value, nameof(viewModel.Value));
            if (viewModel.ShowValues || viewModel.IsDynamic) return;

            var value = string.Join(",", viewModel.Value);
            if (string.IsNullOrEmpty(value)) return;

            var values = value.Split(',');
            if (values == null) return;

            foreach (var v in values)
            {
                if (!ID.TryParse(v, out ID valueId)) continue;

                if (!ID.IsNullOrEmpty(valueId))
                {
                    var it = Context.Database.GetItem(valueId);
                    if (it != null)
                    {
                        var itemValue = !string.IsNullOrEmpty(viewModel.ValueFieldName) && it.Fields[viewModel.ValueFieldName] != null ? it.Fields[viewModel.ValueFieldName].ToString() : it.DisplayName;

                        if (!string.IsNullOrEmpty(itemValue) && itemValue != (viewModel.Value == null ? string.Empty : string.Join(",", viewModel.Value)))
                        {
                            viewModel.Value = itemValue.Split(',').ToList();
                        }
                    }
                }
            }
        }
    }
}