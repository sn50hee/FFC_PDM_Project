import joblib
import json
import sys
import os

script_dir = os.path.dirname(os.path.abspath(__file__))
model_path = os.path.join(script_dir, "XGBoost_model.pkl")
loaded_model = joblib.load(model_path)

new_data=input()
# new_data="[[158.5692601, 390.7888887, 91.69100281, 42.63577924, 14, 0, 2, 18]]"
new_data_list = json.loads(new_data)
result = loaded_model.predict(new_data_list)


# sys.stdout.write(str(result))
# sys.stdout.flush()
print(result)