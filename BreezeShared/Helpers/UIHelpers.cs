using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Breeze.AssetTypes.DataBoundTypes;
using Breeze.Shared.AssetTypes;

namespace Breeze.Helpers
{
    public static class UIHelpers
    {

        public static  T FindChildOfType<T>(DataboundContainterAsset asset) where T : DataboundAsset
        {
            if (asset.Children.Value != null)
            {
                if (asset.Children.Value.FirstOrDefault(x => x is T) is T found)
                {
                    return found;
                }

                foreach (DataboundContainterAsset databoundAsset in asset.Children.Value.Where(x => x is DataboundContainterAsset))
                {
                    T foundInChildren = FindChildOfType<T>(databoundAsset);

                    if (foundInChildren != null)
                    {
                        return foundInChildren;
                    }
                }
            }

            return null;
        }

        public static List<T> FindChildrenOfType<T>(DataboundContainterAsset asset) where T : DataboundAsset
        {
            List<T> results = new List<T>();
            if (asset.Children.Value != null)
            {
                foreach (DataboundAsset databoundAsset in asset.Children.Value)
                {
                    if ((databoundAsset as T) != null)
                    {
                        results.Add((T)databoundAsset);
                    }

                    if (databoundAsset is DataboundContainterAsset databoundContainterAsset)
                    {
                        List<T> foundInChildren = FindChildrenOfType<T>(databoundContainterAsset);

                        if (foundInChildren != null)
                        {
                            results.AddRange(foundInChildren);
                        }
                    }
                }
            }

            return results;
        }

        public static List<DataboundAsset> FlattenTree(DataboundAsset asset)
        {
            List<DataboundAsset> result = new List<DataboundAsset> { asset };

            if (asset is DataboundContainterAsset dbc)
            {
                foreach (DataboundAsset databoundAsset in GetVirtualChildren(dbc))
                {
                    result.AddRange(FlattenTree(databoundAsset));
                }
            }


            return result.Where(t => !(t is ContentAsset)).ToList();
        }

        public static List<DataboundAsset> FlattenTree(List<DataboundAsset> assets)
        {
            return assets.SelectMany(FlattenTree).ToList();
        }




        public static List<DataboundAsset> GetVirtualChildren(DataboundAsset screenAsset)
        {
            List<DataboundAsset> result = new List<DataboundAsset>();

            if (screenAsset is TemplateAsset templateAsset)
            {
                if (!string.IsNullOrWhiteSpace(templateAsset.Template.Value))
                {
                  //  string templateName = templateAsset.Template.Value;

                  //  DataboundAsset templateContent = Templates[templateName].DeepClone();
                  //  templateContent.ParentPosition = templateAsset.ActualPosition;

                    //if (templateContent is DataboundContainterAsset templateContainterAsset)
                    //{
                    //    List<DataboundAsset> childs = templateAsset.Children.Value.DeepClone();
                    //    foreach (DataboundAsset databoundAsset in childs)
                    //    {
                    //        databoundAsset.ParentPosition = templateAsset.ActualPosition;
                    //    }
                    //    List<ContentAsset> content = FindChildrenOfType<ContentAsset>(templateContainterAsset);



                    //    foreach (var cont in content)
                    //    {
                    //        cont.Children.Value = childs;
                    //        cont.ParentPosition = templateAsset.ActualPosition;
                    //    }

                    //  //  templateContainterAsset.Children = new DataboundAsset.DataboundValue<List<DataboundAsset>>();
                    //}

                   // result.Add(templateContent);
                    //result.AddRange(GetVirtualChildren(templateContent));


                }
            }
            else if (screenAsset is DataboundContainterAsset asset)
            {
                if (asset.Children.Value != null)
                {
                    result.AddRange(asset.Children.Value);
                }
            }

            return result;
        }

        public static List<DataboundAsset> GetAssetsAndTheirChildren(List<DataboundAsset> assets)
        {
            List<DataboundAsset> result = new List<DataboundAsset>();
            foreach (var screenAsset in assets)
            {
                result.Add(screenAsset);
                if (screenAsset is DataboundContainterAsset asset)
                {
                    result.AddRange(GetAssetsAndTheirChildren(GetVirtualChildren(asset)).ToList());
                }
            }

            return result;
        }
    }
}
