﻿#region Imports

using ReaLTaiizor.Design.Metro;
using ReaLTaiizor.Enum.Metro;
using ReaLTaiizor.Extension.Metro;
using ReaLTaiizor.Interface.Metro;
using ReaLTaiizor.Manager;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace ReaLTaiizor.Controls
{
    #region MetroTile

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(MetroTile), "Bitmaps.Button.bmp")]
    [Designer(typeof(MetroTileDesigner))]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    [ComVisible(true)]
    public class MetroTile : Control, IMetroControl
    {
        #region Interfaces

        [Category("Metro"), Description("Gets or sets the style associated with the control.")]
        public Style Style
        {
            get => StyleManager?.Style ?? _style;
            set
            {
                _style = value;
                switch (value)
                {
                    case Style.Light:
                        ApplyTheme();
                        break;
                    case Style.Dark:
                        ApplyTheme(Style.Dark);
                        break;
                    case Style.Custom:
                        ApplyTheme(Style.Custom);
                        break;
                    default:
                        ApplyTheme();
                        break;
                }
                Invalidate();
            }
        }

        [Category("Metro"), Description("Gets or sets the The Author name associated with the theme.")]
        public string ThemeAuthor { get; set; }

        [Category("Metro"), Description("Gets or sets the The Theme name associated with the theme.")]
        public string ThemeName { get; set; }

        [Category("Metro"), Description("Gets or sets the Style Manager associated with the control.")]
        public MetroStyleManager StyleManager
        {
            get => _styleManager;
            set
            {
                _styleManager = value;
                Invalidate();
            }
        }

        #endregion Interfaces

        #region Global Vars

        private readonly Utilites _utl;

        #endregion Global Vars

        #region Internal Vars

        private MouseMode _state;
        private Style _style;
        private MetroStyleManager _styleManager;

        private bool _isDerivedStyle = true;
        private TileAlign _textAlign;
        private Color _normalColor;
        private Color _normalBorderColor;
        private Color _normalTextColor;
        private Color _hoverColor;
        private Color _hoverBorderColor;
        private Color _hoverTextColor;
        private Color _pressColor;
        private Color _pressBorderColor;
        private Color _pressTextColor;
        private Color _disabledBackColor;
        private Color _disabledForeColor;
        private Color _disabledBorderColor;

        #endregion Internal Vars

        #region Constructors

        public MetroTile()
        {
            SetStyle
            (
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                    true
            );
            UpdateStyles();
            base.Font = MetroFonts.Light(10);
            _utl = new Utilites();
            ApplyTheme();
        }

        #endregion Constructors

        #region Draw Control

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle r = new(1, 1, Width - 2, Height - 2);
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            StringFormat sf = TileAlign switch
            {
                TileAlign.BottmLeft => new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far },
                TileAlign.BottomRight => new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far },
                TileAlign.Topleft => new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near },
                TileAlign.TopRight => new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near },
                TileAlign.TopCenter => new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near },
                TileAlign.BottomCenter => new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far },
                _ => throw new ArgumentOutOfRangeException(),
            };
            switch (_state)
            {
                case MouseMode.Normal:
                    using (SolidBrush bg = new(NormalColor))
                    {
                        using Pen p = new(NormalBorderColor, 2);
                        using SolidBrush tb = new(NormalTextColor);
                        if (BackgroundImage != null)
                        {
                            g.DrawImage(BackgroundImage, r);
                        }
                        else
                        {
                            g.FillRectangle(bg, r);
                            g.DrawRectangle(p, r);
                        }
                        g.DrawString(Text, Font, tb, r, sf);
                    }
                    break;
                case MouseMode.Hovered:
                    Cursor = Cursors.Hand;
                    using (SolidBrush bg = new(HoverColor))
                    {
                        using Pen p = new(HoverBorderColor, 2);
                        using SolidBrush tb = new(HoverTextColor);
                        if (BackgroundImage != null)
                        {
                            g.DrawImage(BackgroundImage, r);
                        }
                        else
                        {
                            g.FillRectangle(bg, r);
                        }

                        g.DrawString(Text, Font, tb, r, sf);
                        g.DrawRectangle(p, r);
                    }
                    break;
                case MouseMode.Pushed:
                    using (SolidBrush bg = new(PressColor))
                    {
                        using Pen p = new(PressBorderColor, 2);
                        using SolidBrush tb = new(PressTextColor);
                        if (BackgroundImage != null)
                        {
                            g.DrawImage(BackgroundImage, r);
                        }
                        else
                        {
                            g.FillRectangle(bg, r);
                        }

                        g.DrawString(Text, Font, tb, r, sf);
                        g.DrawRectangle(p, r);
                    }
                    break;
                case MouseMode.Disabled:
                    using (SolidBrush bg = new(DisabledBackColor))
                    {
                        using Pen p = new(DisabledBorderColor);
                        using SolidBrush tb = new(DisabledForeColor);
                        g.FillRectangle(bg, r);
                        g.DrawString(Text, Font, tb, r, sf);
                        g.DrawRectangle(p, r);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion Draw Control

        #region ApplyTheme

        private void ApplyTheme(Style style = Style.Light)
        {
            if (!IsDerivedStyle)
            {
                return;
            }

            switch (style)
            {
                case Style.Light:
                    NormalColor = Color.FromArgb(65, 177, 225);
                    NormalBorderColor = Color.FromArgb(65, 177, 225);
                    NormalTextColor = Color.White;
                    HoverColor = Color.FromArgb(65, 177, 225);
                    HoverBorderColor = Color.FromArgb(230, 230, 230);
                    HoverTextColor = Color.White;
                    PressColor = Color.FromArgb(65, 177, 225);
                    PressBorderColor = Color.FromArgb(65, 177, 225);
                    PressTextColor = Color.White;
                    DisabledBackColor = Color.FromArgb(204, 204, 204);
                    DisabledBorderColor = Color.FromArgb(155, 155, 155);
                    DisabledForeColor = Color.FromArgb(136, 136, 136);
                    ThemeAuthor = "Taiizor";
                    ThemeName = "MetroLight";
                    break;
                case Style.Dark:
                    NormalColor = Color.FromArgb(65, 177, 225);
                    NormalBorderColor = Color.FromArgb(65, 177, 225);
                    NormalTextColor = Color.White;
                    HoverColor = Color.FromArgb(65, 177, 225);
                    HoverBorderColor = Color.FromArgb(102, 102, 102);
                    HoverTextColor = Color.White;
                    PressColor = Color.FromArgb(65, 177, 225);
                    PressBorderColor = Color.FromArgb(51, 51, 51);
                    PressTextColor = Color.White;
                    DisabledBackColor = Color.FromArgb(80, 80, 80);
                    DisabledBorderColor = Color.FromArgb(109, 109, 109);
                    DisabledForeColor = Color.FromArgb(109, 109, 109);
                    ThemeAuthor = "Taiizor";
                    ThemeName = "MetroDark";
                    break;
                case Style.Custom:
                    if (StyleManager != null)
                    {
                        foreach (System.Collections.Generic.KeyValuePair<string, object> varkey in StyleManager.TileDictionary)
                        {
                            if (varkey.Key is null)
                            {
                                return;
                            }

                            if (varkey.Key == "NormalColor")
                            {
                                NormalColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "NormalBorderColor")
                            {
                                NormalBorderColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "NormalTextColor")
                            {
                                NormalTextColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "HoverColor")
                            {
                                HoverColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "HoverBorderColor")
                            {
                                HoverBorderColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "HoverTextColor")
                            {
                                HoverTextColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "PressColor")
                            {
                                PressColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "PressBorderColor")
                            {
                                PressBorderColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "PressTextColor")
                            {
                                PressTextColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "DisabledBackColor")
                            {
                                DisabledBackColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "DisabledBorderColor")
                            {
                                DisabledBorderColor = _utl.HexColor((string)varkey.Value);
                            }
                            else if (varkey.Key == "DisabledForeColor")
                            {
                                DisabledForeColor = _utl.HexColor((string)varkey.Value);
                            }
                        }
                    }

                    Refresh();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        #endregion Theme Changing

        #region Properties

        [Browsable(false)]
        public override Color BackColor => Color.Transparent;

        [Category("Metro"), Description("Gets or sets a value indicating whether the control can respond to user interaction.")]
        public new bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (value == false)
                {
                    _state = MouseMode.Disabled;
                }

                Invalidate();
            }
        }

        [Category("Metro"), Description("Gets or sets the BackgroundImage associated with the control.")]
        public override Image BackgroundImage { get => base.BackgroundImage; set => base.BackgroundImage = value; }

        [Category("Metro"), Description("Gets or sets the TileAlign associated with the control.")]
        [DefaultValue(TileAlign.BottmLeft)]
        public TileAlign TileAlign
        {
            get => _textAlign;
            set
            {
                _textAlign = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the control background color in normal mouse sate.")]
        public Color NormalColor
        {
            get => _normalColor;
            set
            {
                _normalColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the control border color in normal mouse sate.")]
        public Color NormalBorderColor
        {
            get => _normalBorderColor;
            set
            {
                _normalBorderColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the control Text color in normal mouse sate.")]
        public Color NormalTextColor
        {
            get => _normalTextColor;
            set
            {
                _normalTextColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the control background color in hover mouse sate.")]
        public Color HoverColor
        {
            get => _hoverColor;
            set
            {
                _hoverColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the control border color in hover mouse sate.")]
        public Color HoverBorderColor
        {
            get => _hoverBorderColor;
            set
            {
                _hoverBorderColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the control Text color in hover mouse sate.")]
        public Color HoverTextColor
        {
            get => _hoverTextColor;
            set
            {
                _hoverTextColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the control background color in pushed mouse sate.")]
        public Color PressColor
        {
            get => _pressColor;
            set
            {
                _pressColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the control border color in pushed mouse sate.")]
        public Color PressBorderColor
        {
            get => _pressBorderColor;
            set
            {
                _pressBorderColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the control Text color in pushed mouse sate.")]
        public Color PressTextColor
        {
            get => _pressTextColor;
            set
            {
                _pressTextColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets backcolor used by the control while disabled.")]
        public Color DisabledBackColor
        {
            get => _disabledBackColor;
            set
            {
                _disabledBackColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the forecolor of the control whenever while disabled.")]
        public Color DisabledForeColor
        {
            get => _disabledForeColor;
            set
            {
                _disabledForeColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the border color of the control while disabled.")]
        public Color DisabledBorderColor
        {
            get => _disabledBorderColor;
            set
            {
                _disabledBorderColor = value;
                Refresh();
            }
        }

        [Category("Metro")]
        [Description("Gets or sets the whether this control reflect to parent(s) style. \n " +
                     "Set it to false if you want the style of this control be independent. ")]
        public bool IsDerivedStyle
        {
            get => _isDerivedStyle;
            set
            {
                _isDerivedStyle = value;
                Refresh();
            }
        }

        #endregion

        #region Events

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _state = MouseMode.Hovered;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _state = MouseMode.Pushed;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _state = MouseMode.Hovered;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseEnter(e);
            _state = MouseMode.Normal;
            Invalidate();
        }

        #endregion Events
    }

    #endregion
}