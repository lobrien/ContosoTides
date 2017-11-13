using System;
using System.Linq;
using CoreML;
using Foundation;
using ContosoTides;

namespace ContosoTides.iOS
{
	class TideInput : NSObject, IMLFeatureProvider
	{
		MLFeatureValue readings;
		MLMultiArray lstm_1_h_in, lstm_1_c_in;

		const int INPUT_SIZE = 200;
		const int MIDDLE_SIZE = 128;

		public NSSet<NSString> FeatureNames
		{
			get
			{
				return new NSSet<NSString>(
					new NSString("readings"),
					new NSString("lstm_1_h_in"),
					new NSString("lstm_1_c_in")
				);
			}
		}

		public MLFeatureValue GetFeatureValue(string featureName)
		{
			switch (featureName)
			{
				case "readings": return readings;
				case "lstm_1_h_in": return MLFeatureValue.Create(lstm_1_h_in);
				case "lstm_1_c_in": return MLFeatureValue.Create(lstm_1_c_in);
				default: throw new ArgumentOutOfRangeException();
			}
		}

		public TideInput(float[] tideInputData)
		{
			//200 elements, 1 batch, 1 feature
			NSError mlErr;
			var ma = new MLMultiArray(new nint[] { INPUT_SIZE, 1, 1 }, MLMultiArrayDataType.Double, out mlErr);
			for (int i = 0; i < tideInputData.Length; i++)
			{
				ma[i] = tideInputData[i];
			}
			readings = MLFeatureValue.Create(ma);
			lstm_1_h_in = new MLMultiArray(new nint[] { MIDDLE_SIZE }, MLMultiArrayDataType.Double, out mlErr);
			lstm_1_c_in = new MLMultiArray(new nint[] { MIDDLE_SIZE }, MLMultiArrayDataType.Double, out mlErr);
			for (int i = 0; i < MIDDLE_SIZE; i++)
			{
				lstm_1_h_in[i] = lstm_1_c_in[i] = new NSNumber(0.0);
			}
		}
		public TideInput(double[] tideInputData, MLMultiArray h, MLMultiArray c)
		{
			//200 elements, 1 batch, 1 feature
			NSError mlErr;
			var ma = new MLMultiArray(new nint[] { INPUT_SIZE, 1, 1 }, MLMultiArrayDataType.Double, out mlErr);
			if (mlErr != null)
			{
				throw new Exception(mlErr.ToString());
			}
			for (int i = 0; i < INPUT_SIZE; i++)
			{
				ma[i] = tideInputData[i];
			}
			readings = MLFeatureValue.Create(ma);
			lstm_1_h_in = h;
			lstm_1_c_in = c;
		}
	}

	public class CoreMLTidePredictor : NSObject, ITidePredictor
	{
		public event EventHandler<EventArgsT<double[]>> PredictionUpdated = delegate { };
		public event EventHandler<EventArgsT<String>> ErrorOccurred = delegate { };
		public event EventHandler<EventArgsT<String>> MessageUpdated = delegate { };

		const int OUTPUT_SIZE = 100;
		const string OUTPUT_NAME = "predicted_tide_ft";

		MLModel model;

		public CoreMLTidePredictor()
		{
			// Load the ML model
			var bundle = NSBundle.MainBundle;
			var assetPath = bundle.GetUrlForResource("LSTM_TidePrediction", "mlmodelc");
			NSError mlErr;
			model = MLModel.Create(assetPath, out mlErr);
			if (mlErr != null)
			{
				ErrorOccurred(this, new EventArgsT<string>(mlErr.ToString()));
			}
		}

		public float[] Predict(float[] seaLevelInputs)
		{
			var inputs = new TideInput(seaLevelInputs);
			NSError mlErr;
			var prediction = model.GetPrediction(inputs, out mlErr);
			if(mlErr != null){
				ErrorOccurred(this, new EventArgsT<string>(mlErr.ToString()));
			};

			var predictionMultiArray = prediction.GetFeatureValue(OUTPUT_NAME).MultiArrayValue;
			var predictedLevels = new float[OUTPUT_SIZE];
			for (int i = 0; i < OUTPUT_SIZE; i++)
			{
				predictedLevels[i] = predictionMultiArray[i].FloatValue;
			}
			return predictedLevels;
		}
	}
}
