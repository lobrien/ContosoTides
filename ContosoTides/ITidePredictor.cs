using System;
namespace ContosoTides
{
	public interface ITidePredictor
	{
		/// <summary>
		/// From 200 input sea leavels, in feet, taken every 3 hours, predict 100 new sea levels (next 300 hours)
		/// </summary>
		/// <returns>100 levels,in feet, representing the predicted tide from the final input time to + i * 3 hours</returns>
		/// <param name="sealevelInputs">200 water levels, measured in feet, taken every 3 hours</param>
		float[] Predict(float[] seaLevelInputs);

	}
}
