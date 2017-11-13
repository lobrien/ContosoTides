import numpy as np
import pandas as pd
from keras.models import Sequential
from keras.layers.core import Activation, Dense, Dropout
from keras.layers.recurrent import LSTM
from keras.callbacks import Callback, LambdaCallback

import sys
import time

# Prepare and format data for training
def dataframe_to_matrices (dataframe, input_count, output_count) :
    data = dataframe.values
    sample_count = len(data) - input_count - output_count

    input_list = [np.expand_dims(np.atleast_2d(data[i:input_count + i, :]), axis=0) for i in xrange(sample_count)]
    input_matrix = np.concatenate(input_list, axis=0)

    # use the tail of the series as the test data
    test_data = pd.DataFrame(dataframe[-input_count:]).values
    test_input_list = [np.expand_dims(np.atleast_2d(test_data[len(test_data) - input_count:len(test_data), :]), axis=0) for i
                       in xrange(1)]
    test_input_matrix = np.concatenate(test_input_list, axis=0)

    # target is the first (and only) column in the input frame
    target_list = [np.atleast_2d(data[i + input_count:input_count + i + output_count, 0]) for i in xrange(sample_count)]
    target_matrix = np.concatenate(target_list, axis=0)
    return (input_matrix, target_matrix, test_input_matrix)

# Basic model does fine : input => LSTM => Dense => output
def build_model(lookback_length, input_feature_count, lstm_layer_output_dimensions, prediction_length, dropout_density):
    model = Sequential()
    model.add(LSTM(lstm_layer_output_dimensions, input_shape=(lookback_length, input_feature_count)))
    model.add(Dropout(dropout_density))
    model.add(Dense(prediction_length))
    model.add(Activation('linear'))
    model.compile(loss='mse', optimizer='rmsprop')
    return model

def output_results (predicted, history) :
    print("Predictions")
    for i in range(output_count):
        sys.stdout.write(str(predicted[0][i]) + ", ")

    # Loss
    print ("")
    print ("Loss")
    map(lambda x: sys.stdout.write(str(x) + ", "), history.history['loss'])

    # Validation accuracy
    print("")
    print("Validation_Loss")
    map(lambda x: sys.stdout.write(str(x) + ", "), history.history['val_loss'])


# Called-back-to every epoch. Stops training if the validation error is below a threshold
class ThresholdStop(Callback) :
    def __init__(self, threshold = 0.1):
        super(Callback, self).__init__()
        self.threshold = threshold

    def on_epoch_end(self, epoch, logs=None):
        if logs is not None :
            if logs['val_loss'] is not None :
                self.model.stop_training = logs['val_loss'] < self.threshold


start_time = time.time()

# Trains, using Keras, an LSTM that looks back 200 samples to predict next 100 samples

# Load data from "Contoso Harbor" [2017-01-01, 2018-01-11], sampled every 3 hours
# This is noisy data in feet -- truth + random Gaussian mu = 0 sigma = 1.50 (inches)
data_frame = pd.read_csv("contoso_noisy.txt", names = ["level"])

input_count = 200 # How far to look back
output_count = 100 # How many steps forward to predict
lstm_layer_output_dimensions = 128 # Size of LSTM output
dropout_density = 0.2 # Dropout density to avoid over-fitting

(training_inputs, training_targets, test_input) = dataframe_to_matrices(data_frame, input_count, output_count)
# How many input features? In this case, 1, but changes from model-to-model
features = training_inputs.shape[2]
model = build_model(input_count, features, lstm_layer_output_dimensions, output_count, dropout_density)

# Train (Experimentally, 0.03 seems to be an "elbow" -- lower ThresholdStop to gain accuracy by spending training time)
training_history = model.fit(training_inputs, training_targets, epochs=2500, batch_size=100, validation_split=0.15, callbacks=[ThresholdStop(0.03)])

# Predict and output results, using input data held back from training
predicted = model.predict(test_input)

output_results(predicted, training_history)

# Save the Keras model
model.save("keras_noisy_lstm.hdf5")

print("--- Total runtime: %s seconds ---" % round(time.time() - start_time, 2))