using Xamarin.Forms;

namespace Translator {
    public class App : Application {
        public App() {
            // The root page of your application
            MainPage = new MyPage();
        }

        protected override void OnStart() {
            // Handle when your app starts
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }
    }

    internal class MyPage : ContentPage {

        private readonly Translator _translator = new Translator();

        public MyPage() {
            var editor = new Editor {
                HeightRequest = 200,
                Text = "Hello world"
            };
            if (Device.OS == TargetPlatform.WinPhone) {
                editor.BackgroundColor = Color.Teal;
            }

            var label = new Label();
            var button = new Button {
                Text = "Translate",
                Command = new Command(async () => {
                    label.Text = await _translator.Get(editor.Text);
                })
            };

            Content = new StackLayout() {
                Padding = new Thickness(10, Device.OnPlatform(30, 10, 10), 10, 0),
                Children = {editor, button, label}
            };

        }
    }
}