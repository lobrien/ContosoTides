using System;
using System.Linq;
using Xamarin.Forms;

namespace ContosoTides
{
	public partial class App : Application
	{

		public App(ITidePredictor platformTidePredictor)
		{
			InitializeComponent();

			MainPage = new ContosoTides4Page(platformTidePredictor);
		}

		protected override void OnStart()
		{
			// Handle when your app starts


		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
