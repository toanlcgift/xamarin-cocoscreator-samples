using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameStoreAndHotUpdateSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        public string myScript = string.Empty;

        public GamePage(string script)
        {
            myScript = script;
            InitializeComponent();
        }
    }
}