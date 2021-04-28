using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Mvc.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedSitecore.Forms.Fields.DropDownPlus
{
	[Serializable]
	public class DropDownPlusViewModel : ListViewModel
	{
		public bool ShowEmptyItem { get; set; }
		public bool ShowItems { get; set; }
		public bool ShowValues { get; set; }
		public string TitleOverride { get; set; }
		public string DefaultSelection { get; set; }

		protected override void InitItemProperties(Item item)
		{
			Assert.ArgumentNotNull(item, nameof(item));
			base.InitItemProperties(item);

			ShowEmptyItem = MainUtil.GetBool(item.Fields["Show Empty Item"]?.Value, false);
			ShowItems = MainUtil.GetBool(item.Fields["Show Items"]?.Value, false);
			ShowValues = MainUtil.GetBool(item.Fields["Show Values"]?.Value, false);
			TitleOverride = StringUtil.GetString(item.Fields["Title Override"]);
			DefaultSelection = StringUtil.GetString(item.Fields["Default Selection"]);
		}

        protected override void UpdateItemFields(Item item)
		{
			Assert.ArgumentNotNull(item, nameof(item));
			base.UpdateItemFields(item);
			item.Fields["Show Empty Item"]?.SetValue(ShowEmptyItem ? "1" : string.Empty, false);
			item.Fields["Show Items"]?.SetValue(ShowItems ? "1" : string.Empty, false);
			item.Fields["Show Values"]?.SetValue(ShowValues ? "1" : string.Empty, false);
			item.Fields["Title Override"]?.SetValue(TitleOverride, false);
			item.Fields["Default Selection"]?.SetValue(DefaultSelection, false);
			//item.Fields["Default Selection"]?.SetValue($"{To};{Text}", false);
		}

        protected override void InitializeDataSourceSettings(Item item)
        {
			Assert.ArgumentNotNull(item, nameof(item));
            base.InitializeDataSourceSettings(item);
			var settings = DataSourceSettingsManager.GetSettings(item);
			if (settings != null)
            {
				Items.Clear();
				Items.AddRange(settings);
            }
        }

		protected override ListFieldItemCollection UpdateDataSourceSettings(Item item)
        {
			Assert.ArgumentNotNull(item, nameof(item));
			var listFieldItemCollection = new ListFieldItemCollection();
			listFieldItemCollection.AddRange(Items);
			DataSourceSettingsManager.SaveSettings(item, listFieldItemCollection);
			return listFieldItemCollection;
        }

        protected override void OnValueChanged(IEnumerable<string> value)
        {
			//hits here after ValueProvider but value is always == null
			if (value == null) return;
			base.OnValueChanged(value);
			Items.ForEach(delegate (ListFieldItem i)
			{
				i.Selected = value.Contains(i.Value);
			});
        }
    }
}