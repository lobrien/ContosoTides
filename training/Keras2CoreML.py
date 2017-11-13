import coremltools

# Convert to CoreML
coreml_model = coremltools.converters.keras.convert("keras_model.hdf5", ["readings"], ["predicted_tide_ft"])
coreml_model.author = 'Larry O\'Brien'
coreml_model.license = 'MIT'
coreml_model.save('LSTM_TidePrediction.mlmodel')
