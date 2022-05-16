using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Loadout_Creator
{
    public sealed partial class PromptDialog : ContentDialog
    {
        public event EventHandler<String> ConfirmedEvent;
        public string Result=null;
        private string t, c="";
        public delegate Tuple<bool, string> ValidationDelegate(string text);
        public ValidationDelegate Validation;
        public string BlacklistedCharacters = "";
        public bool MultiLine=false;
        public bool canBeEmpty = false;
        public PromptDialog(string title, string Current = "")
        {
            t = title;
            c = Current;

            this.InitializeComponent();
        }
        private bool Validate()
        {
            if (Box.Text.Length == 0 && !canBeEmpty)
            {
                ErrorBox.Text = "Input Cannot Be Empty";
                return false;
            }
            if (BlacklistedCharacters.Any(e => Box.Text.Contains(e)))
            {
                ErrorBox.Text = "Input cannot contain [" + string.Join(", ", BlacklistedCharacters.ToCharArray()) + "]";
                return false;
            }

            if (Validation != null)
            {
                var res = Validation(Box.Text);
                if (!res.Item1)
                {
                    ErrorBox.Text = res.Item2;
                    return false;
                }
            }
            return true;
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (Validate())
            {
                var txt = Box.Text;
                Result = txt;
                Hide();
                ConfirmedEvent?.Invoke(this, txt);
            }
            else
                args.Cancel = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
                Hide();
            else if (e.Key.Equals(Windows.System.VirtualKey.Enter))
            {
                if (Validate())
                {
                    var txt = Box.Text;
                    Result = txt;
                    Hide();
                    ConfirmedEvent?.Invoke(this, Box.Text);
                }
            }

        }

        private void Box_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel=args.NewText.Any(e => BlacklistedCharacters.Contains(e));
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Title = t;
            Box.AcceptsReturn = MultiLine;
            Box.SelectedText = c;
        }
    }
}
