using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WEngine;
using WEngine.Core.Input;

namespace WEngine.GUI
{
    public delegate void TextInputEditDelegate();

    public delegate void TextInputTextEdited(string text);

    public class TextInputField : Button
    {
        public event TextInputEditDelegate OnEnterEdit;
        public event TextInputEditDelegate OnLeaveEdit;
        public event TextInputTextEdited OnTextEdited;

        private bool _FirstRun = true;
        public bool Editing { get; private set; } = false;
        public string EmptyText { get; set; } = "Enter text...";
        public string Text { get; set; } = null;

        public bool AllowNewLine { get; set; } = true;

        public int MaxChars { get; set; } = int.MaxValue;

        public Color256 TextColor { get; set; } = Color256.White;
        public Color256 EmptyTextColor { get; set; } = Color256.DarkGray;
        public Color256 EditingColor { get; set; } = Color256.Blue;
        public Color256 NotEditingColor { get; set; } = Color256.White;
        public Color256 TextInputHoverColor { get; set; } = Color256.NordVPN;

        private static System.Windows.Forms.KeysConverter KeyConverter { get; } = new System.Windows.Forms.KeysConverter();

        protected internal override void Creation()
        {
            base.Creation();

            this.OnClick += () => 
            {
                if(!Editing)
                {
                    Editing = true;
                    this.OnEnterEdit?.Invoke();
                }
            };
        }

        protected internal override void Update()
        {
            base.Update();

            if (!Hovered && Editing && Input.IsPressing(Keys.MouseLeftButton))
            {
                Editing = false;
                this.OnLeaveEdit?.Invoke();
            }

            if (!Graphics.Window.Focused) Editing = false;

            bool textEdited = false;
            int chars = Text == null ? 0 : Text.Length;

            KeysModifiers modifiers = Input.GetModifiers();

            if (Editing)
            {
                Keys[] keys = Input.GetAllKeys(KeyStates.Pressing);

                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] != Keys.Back && Text?.Length >= MaxChars)
                    {
                        Text = Text.Substring(0, MaxChars);
                        textEdited = true;
                        break;
                    }

                    if (keys[i].IsKeyboard() && !keys[i].IsModifier())
                    {
                        if (keys[i] == Keys.Escape || keys[i] == Keys.Enter)
                        {
                            bool stop = true;
                            if (AllowNewLine)
                            {
                                if(modifiers.HasFlag(KeysModifiers.Shift))
                                {
                                    stop = false;
                                    Text += "\n";
                                    textEdited = true;
                                }
                            }
                            if (stop)
                            {
                                this.Editing = false;
                                this.OnLeaveEdit?.Invoke();
                                break;
                            }
                        }
                        else if (keys[i] == Keys.Back)
                        {
                            Text = Text.Substring(0, WMath.Clamp(Text.Length - 1, 0, int.MaxValue));
                            textEdited = true;
                        }
                        else if (keys[i] == Keys.Space)
                        {
                            Text += " ";
                            textEdited = true;
                        }
                        else if (keys[i] == Keys.Tab)
                        {
                            Text += "    ";
                            textEdited = true;
                        }
                        else
                        {
                            textEdited = true;
                            Text += keys[i].ToString(modifiers);
                        }
                    }
                }
            }

            if (_FirstRun || textEdited)
            {
                _FirstRun = false;

                if (!string.IsNullOrEmpty(this.Text))
                {
                    this.Label.Color = this.TextColor;
                    this.Label.Text = this.Text;
                }
                else
                {
                    this.Label.Color = this.EmptyTextColor;
                    this.Label.Text = this.EmptyText;
                }
            }

            if(Editing)
            {
                this.IdleColor = this.EditingColor;
                this.HoverColor = this.EditingColor;
            }

            else
            {
                this.IdleColor = this.NotEditingColor;
                this.HoverColor = this.TextInputHoverColor;
            }
        }
    }
}
