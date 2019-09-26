# Autonomous Exploration Obstacle Avoidance Agent
>TODO

## Brief

TODO

## Prerequisites

* Unity 2019 or higher
* mlagents (Unity's Machine Learning Toolkit)
* Python 3.6

## Installation
```sh
$ git clone https://github.com/dodicin/autonomous-exploration-agent
$ cd autonomous-exploration-agent
$ virtualenv .venv -p C:/Python/Python36/python.exe 
$ .venv/Script/activate.bat
$ pip install mlagents
```

If pip gives some errors for the bleach and pillow packages:
```sh
$ pip uninstall pillow==6.1.0 // Uninstall the uncompatible version
$ pip install pillow==5.4.1

$ pip uninstall bleach==3.1.0
$ pip install bleach==1.5.0
```
## Training
* Open the project
* Tick the Control checkboxes in the Broadcast Hub for the Brains that are going to be trained
* Remove the model from the Brain objects under `./Assets/Brains`
* Launch a command prompt:
```sh
$ tensorboard --logdir=summaries
$ mlagents-learn MLConfig\trainer_config.yaml --run-id=TEST --train
```
* Stop the training when the cumulative reward reaches a satisfying level
* Models are saved under `./models`

# Testing
* Open the project
* Unick the Control checkboxes in the Broadcast
* Add the models to the Brain objects under `./Assets/Brains`. The models are found under `./models`
* Press the play button to see the trained model in action

## Authors

* **Federico Bottoni** (xxxxxx) - 
* **Nassim Habbash** (808292) - [dodicin](https://github.com/dodicin)
