using System;
using System.Linq;
using Android.Content.Res;
using Org.Tensorflow.Contrib.Android;


namespace ContosoTides.Droid
{
	public class TensorflowInferencePredictor : ITidePredictor
	{
		const string MODEL_FILE_URL = "file:///android_asset/TF_LSTM_Inference.pb";
		const string INPUT_ARGUMENT_NAME = "lstm_1_input";
		const string OUTPUT_VARIABLE_NAME = "output_node0";
		const int OUTPUT_SIZE = 100;

		TensorFlowInferenceInterface inferenceInterface;

		public TensorflowInferencePredictor(AssetManager assetManager)
		{
			inferenceInterface = new TensorFlowInferenceInterface(assetManager, MODEL_FILE_URL);
		}

		public float[] Predict(float[] inputSeaLevels)
		{
			inferenceInterface.Feed(INPUT_ARGUMENT_NAME, inputSeaLevels, inputSeaLevels.Length, 1, 1);
			inferenceInterface.Run(new string[] { OUTPUT_VARIABLE_NAME });
			float[] predictions = new float[OUTPUT_SIZE];
			inferenceInterface.Fetch(OUTPUT_VARIABLE_NAME, predictions);
			return predictions;
		}
	}
}
