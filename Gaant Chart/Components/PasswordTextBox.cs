using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gaant_Chart.Components
{
    public class PasswordTextBox
    {
        public TextBox textbox { get; set; }
        public String password = "";
        private Boolean addingStars = false;

        public PasswordTextBox()
        {
            textbox = new TextBox();
            textbox.VerticalContentAlignment = VerticalAlignment.Center;
            textbox.TextChanged += Textbox_TextChanged;
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (addingStars) return;

            int n = textbox.Text.Length;
            if (n == 0)
            {
                password = "";
                return;
            }

            char c = textbox.Text[n - 1];

            if(n > password.Length)
            {
                password += c;
            }
            else
            {
                int i = password.Length;
                while(i > n)
                {
                    password = password.Remove(password.Length - 1);
                    i--;
                }
            }

            String displayText = "";

            for(int i = 0; i < n - 1; i++)
            {
                displayText += "*";
            }

            displayText += c;

            addingStars = true;
            textbox.Text = displayText;
            addingStars = false;

            textbox.SelectionStart = n;
            textbox.SelectionLength = 0;
        }
    }

}
