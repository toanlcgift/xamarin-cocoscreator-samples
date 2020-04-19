using CocosCreator.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GameStoreAndHotUpdateSample
{
    public class GamePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand NativeCallCommand { get; set; }
        public GamePageViewModel()
        {
            NativeCallCommand = new Command(OnExecuteNativeCallCommand);
        }

        private void OnExecuteNativeCallCommand(object obj)
        {
            var title = (obj as JSMessage).Title;
            var content = (obj as JSMessage).Content;
            System.Diagnostics.Debug.WriteLine(content);
            switch (title)
            {
                case "done":
                    JSBrigde.EvaluateScript(((App.Current.MainPage as NavigationPage).CurrentPage as GamePage).myScript);
                    break;
                default:
                    break;
            }

        }
    }
}
