using Xamarin.Forms;
using Microcharts;
using SkiaSharp;
using System;
using System.Linq;

namespace ContosoTides
{
	public partial class ContosoTides4Page : ContentPage
	{
		ITidePredictor predictor;
		event EventHandler<EventArgsT<float[]>> TidePredictionUpdated;

		float[][] DataSet = new[]
		{
			// Test data
			new[] {
				4.92F, 2.022F, -0.206F, 2.355F, 4.08F, 1.828F, -0.005F, 2.83F,
				4.966F, 2.715F, -0.073F, 1.69F, 3.958F, 2.5F, 0.201F, 2.075F, 4.754F, 3.475F,
				0.345F, 0.954F, 3.665F, 3.165F, 0.562F, 1.285F, 4.415F, 4.083F, 0.83F, 0.327F,
				3.304F, 3.589F, 0.976F, 0.707F, 3.989F, 4.37F, 1.375F, 0.039F, 2.863F, 3.715F,
				1.525F, 0.507F, 3.403F, 4.38F, 2.06F, 0.097F, 2.251F, 3.671F, 2.223F, 0.579F,
				2.605F, 4.25F, 2.791F, 0.306F, 1.489F, 3.575F, 2.907F, 0.759F, 1.741F, 4.018F,
				3.381F, 0.589F, 0.796F, 3.382F, 3.441F, 1.103F, 1.099F, 3.566F, 3.75F, 1.09F,
				0.393F, 2.906F, 3.831F, 1.77F, 0.816F, 2.739F, 3.922F, 1.891F, 0.285F, 2.02F,
				4.083F, 2.727F, 0.833F, 1.593F, 3.863F, 2.824F, 0.408F, 0.906F, 4.071F, 3.738F,
				1.149F, 0.511F, 3.431F, 3.626F, 0.863F, -0.004F, 3.57F, 4.568F, 1.906F,
				-0.079F, 2.489F, 4.115F, 1.777F, -0.396F, 2.456F, 5.046F, 3.106F, -0.04F,
				1.113F, 4.168F, 2.998F, -0.246F, 0.941F, 5.009F, 4.425F, 0.516F, -0.267F,
				3.695F, 4.092F, 0.405F, -0.384F, 4.32F, 5.429F, 1.541F, -1.036F, 2.628F,
				4.711F, 1.479F, -0.977F, 3.043F, 5.82F, 2.856F, -0.932F, 1.249F, 4.713F,
				2.828F, -0.734F, 1.438F, 5.546F, 4.182F, -0.159F, -0.109F, 4.137F, 4.03F,
				0.09F, 0.042F, 4.693F, 5.075F, 0.921F, -0.925F, 3.14F, 4.67F, 1.227F, -0.599F,
				3.458F, 5.274F, 2.089F, -0.893F, 1.945F, 4.618F, 2.489F, -0.336F, 2.106F,
				4.828F, 3.178F, -0.192F, 0.816F, 4.026F, 3.61F, 0.491F, 0.961F, 3.972F, 3.93F,
				0.717F, 0.057F, 3.155F, 4.273F, 1.457F, 0.369F, 2.96F, 4.147F, 1.559F, -0.088F,
				2.226F, 4.349F, 2.398F, 0.485F, 1.964F, 3.861F, 2.329F, 0.335F, 1.357F, 3.972F,
				3.286F, 1.092F, 1.083F, 3.277F, 3.011F, 0.981F, 0.632F, 3.375F, 3.987F
			},
			// First date is June 1, 2018
			new float[] {
				2.522F, 4.158F, 2.478F, 0.759F, 3.172F, 4.9F, 3.155F, 0.496F, 1.839F, 3.872F,
				3.171F, 1.07F, 2.407F, 4.566F, 3.83F, 0.941F, 1.206F, 3.523F, 3.685F, 1.424F,
				1.774F, 4.162F, 4.22F, 1.414F, 0.831F, 3.128F, 3.915F, 1.889F, 1.467F, 3.634F,
				4.317F, 2.026F, 0.789F, 2.591F, 3.948F, 2.551F, 1.457F, 2.892F, 4.237F, 2.765F,
				0.933F, 1.866F, 3.903F, 3.298F, 1.586F, 2.017F, 4.035F, 3.445F, 1.157F, 1.119F,
				3.758F, 3.95F, 1.872F, 1.286F, 3.623F, 3.923F, 1.586F, 0.616F, 3.345F, 4.443F,
				2.48F, 0.925F, 2.848F, 4.174F, 2.341F, 0.453F, 2.511F, 4.768F, 3.426F, 0.931F,
				1.717F, 4.175F, 3.292F, 0.57F, 1.375F, 4.828F, 4.475F, 1.254F, 0.565F, 3.82F,
				4.144F, 1.002F, 0.355F, 4.432F, 5.348F, 1.98F, -0.138F, 2.988F, 4.678F, 1.859F,
				-0.154F, 3.443F, 5.866F, 3.128F, -0.173F, 1.714F, 4.785F, 3.038F, -0.072F,
				1.996F, 5.9F, 4.428F, 0.357F, 0.339F, 4.407F, 4.162F, 0.499F, 0.586F, 5.356F,
				5.451F, 1.294F, -0.574F, 3.543F, 4.854F, 1.495F, -0.2F, 4.241F, 5.903F, 2.533F,
				-0.66F, 2.304F, 4.952F, 2.793F, -0.139F, 2.748F, 5.717F, 3.838F, -0.034F,
				0.982F, 4.493F, 4.047F, 0.558F, 1.319F, 4.978F, 4.815F, 0.935F, 0.037F, 3.619F,
				4.842F, 1.588F, 0.495F, 3.856F, 5.168F, 2.0F, -0.158F, 2.517F, 4.987F, 2.778F,
				0.546F, 2.57F, 4.888F, 3.046F, 0.356F, 1.394F, 4.572F, 3.932F, 1.243F, 1.385F,
				4.167F, 3.884F, 1.176F, 0.511F, 3.816F, 4.758F, 2.155F, 0.632F, 3.231F, 4.292F,
				1.975F, 0.152F, 2.924F, 5.055F, 3.064F, 0.552F, 2.248F, 4.214F, 2.728F, 0.399F,
				2.016F, 4.864F, 3.956F, 1.056F, 1.313F, 3.783F, 3.458F, 1.0F, 1.177F, 4.373F,
				4.741F, 1.782F, 0.553F, 3.175F, 4.027F, 1.627F, 0.57F, 3.746F, 5.226F, 2.481F,
				0.181F, 2.503F
			},
			// Garbage in... 
			new[] {
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
				0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F,
			}
		};

		public ContosoTides4Page(ITidePredictor platformPredictor)
		{
			this.predictor = platformPredictor;

			InitializeComponent();

			// Configure tide chart
			var chart = new LineChart();
			chart.LineMode = LineMode.Straight;
			tideChart.Chart = chart;

			TidePredictionUpdated += (s, e) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					UpdateTideGraph(e.Value);
					UpdateTideNumbers(e.Value);
					EnableButtons(true);
				});
			};
			KickoffPredict();
		}

		void UpdateTideGraph(float[] sealevels)
		{
			var oceanColor = SKColor.Parse("#3e85d6");
			var entries = sealevels.Take(20).Select(s => new Microcharts.Entry((float) s) { Label = s.ToString("n1"), Color = oceanColor});
			tideChart.Chart.Entries = entries;
		}

		void UpdateTideNumbers(float[] sealevels)
		{
			var shortStrings = sealevels.Select(s => s.ToString("n1"));
			var output = String.Join(", ", shortStrings);
			tidePredictions.Text = output;
		}

		void KickoffPredict()
		{
			Predict(DataSet[0]);
		}

		void EnableButtons(bool enabled)
		{
			Button1.IsEnabled = enabled;
			Button2.IsEnabled = enabled;
			Button3.IsEnabled = enabled;
		}


		void Predict(float[] inputs)
		{
			EnableButtons(false);
			var tides = predictor.Predict(inputs);
			TidePredictionUpdated(this, new EventArgsT<float[]>(tides));
		}

		void OnButtonClicked(object sender, EventArgs e)
		{
			var btn = (sender as Button);
			if (btn.Equals(Button1))
			{
				Predict(DataSet[0]);
			}
			else if (btn.Equals(Button2))
			{
				Predict(DataSet[1]);
			}
			else if (btn.Equals(Button3))
			{
				Predict(DataSet[2]);
			}
		}
	}
}
