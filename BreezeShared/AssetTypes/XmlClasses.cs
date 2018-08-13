//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Breeze.Shared.AssetTypes
//{
//    class XmlClasses
//    {

//        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
//        /// <remarks/>
//        [System.SerializableAttribute()]
//        [System.ComponentModel.DesignerCategoryAttribute("code")]
//        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
//        public partial class EmGooey
//        {

//            private EmGooeyBoxShadowAsset boxShadowAssetField;

//            /// <remarks/>
//            public EmGooeyBoxShadowAsset BoxShadowAsset
//            {
//                get
//                {
//                    return this.boxShadowAssetField;
//                }
//                set
//                {
//                    this.boxShadowAssetField = value;
//                }
//            }
//        }

//        /// <remarks/>
//        [System.SerializableAttribute()]
//        [System.ComponentModel.DesignerCategoryAttribute("code")]
//        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//        public partial class EmGooeyBoxShadowAsset
//        {

//            private EmGooeyBoxShadowAssetRectangleAsset rectangleAssetField;

//            private string colorField;

//            private string positionField;

//            /// <remarks/>
//            public EmGooeyBoxShadowAssetRectangleAsset RectangleAsset
//            {
//                get
//                {
//                    return this.rectangleAssetField;
//                }
//                set
//                {
//                    this.rectangleAssetField = value;
//                }
//            }

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public string Color
//            {
//                get
//                {
//                    return this.colorField;
//                }
//                set
//                {
//                    this.colorField = value;
//                }
//            }

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public string Position
//            {
//                get
//                {
//                    return this.positionField;
//                }
//                set
//                {
//                    this.positionField = value;
//                }
//            }
//        }

//        /// <remarks/>
//        [System.SerializableAttribute()]
//        [System.ComponentModel.DesignerCategoryAttribute("code")]
//        [System.Xml.Serialization.XmlType]
//        public partial class EmGooeyBoxShadowAssetRectangleAsset
//        {

//            private EmGooeyBoxShadowAssetRectangleAssetStackAsset stackAssetField;

//            private byte brushSizeField;

//            private string positionField;

//            private string backgroundColorField;

//            /// <remarks/>
//            public EmGooeyBoxShadowAssetRectangleAssetStackAsset StackAsset
//            {
//                get
//                {
//                    return this.stackAssetField;
//                }
//                set
//                {
//                    this.stackAssetField = value;
//                }
//            }

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public byte BrushSize
//            {
//                get
//                {
//                    return this.brushSizeField;
//                }
//                set
//                {
//                    this.brushSizeField = value;
//                }
//            }

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public string Position
//            {
//                get
//                {
//                    return this.positionField;
//                }
//                set
//                {
//                    this.positionField = value;
//                }
//            }

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public string BackgroundColor
//            {
//                get
//                {
//                    return this.backgroundColorField;
//                }
//                set
//                {
//                    this.backgroundColorField = value;
//                }
//            }
//        }

//        /// <remarks/>
//        [System.SerializableAttribute()]
//        [System.ComponentModel.DesignerCategoryAttribute("code")]
//        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//        public partial class EmGooeyBoxShadowAssetRectangleAssetStackAsset
//        {

//            private EmGooeyBoxShadowAssetRectangleAssetStackAssetRectangleAsset rectangleAssetField;

//            private string positionField;

//            /// <remarks/>
//            public EmGooeyBoxShadowAssetRectangleAssetStackAssetRectangleAsset RectangleAsset
//            {
//                get
//                {
//                    return this.rectangleAssetField;
//                }
//                set
//                {
//                    this.rectangleAssetField = value;
//                }
//            }

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public string Position
//            {
//                get
//                {
//                    return this.positionField;
//                }
//                set
//                {
//                    this.positionField = value;
//                }
//            }
//        }

//        /// <remarks/>
//        [System.SerializableAttribute()]
//        [System.ComponentModel.DesignerCategoryAttribute("code")]
//        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//        public partial class EmGooeyBoxShadowAssetRectangleAssetStackAssetRectangleAsset
//        {

//            private byte brushSizeField;

//            private string positionField;

//            private string colorField;

//            private byte blurField;

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public byte BrushSize
//            {
//                get
//                {
//                    return this.brushSizeField;
//                }
//                set
//                {
//                    this.brushSizeField = value;
//                }
//            }

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public string Position
//            {
//                get
//                {
//                    return this.positionField;
//                }
//                set
//                {
//                    this.positionField = value;
//                }
//            }

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public string Color
//            {
//                get
//                {
//                    return this.colorField;
//                }
//                set
//                {
//                    this.colorField = value;
//                }
//            }

//            /// <remarks/>
//            [System.Xml.Serialization.XmlAttributeAttribute()]
//            public byte Blur
//            {
//                get
//                {
//                    return this.blurField;
//                }
//                set
//                {
//                    this.blurField = value;
//                }
//            }
//        }


//    }
//}
